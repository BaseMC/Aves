using ADB.Config;
using ADB.Config.Java;
using Aves.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using Aves.Shared.Download;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using ADB.Util;
using System.Net.Http;

namespace ADB.Java
{
   public class JavaTask
   {
      private Configuration BaseConfig { get; set; }

      private JavaConfig Config => BaseConfig.JavaConfig;

      public JavaTask(Configuration config)
      {
         BaseConfig = config;
      }

      public void Run()
      {
         Log.Info("Starting JavaTask");

         DetermineOsAndArchFromRID();

         Init();

         var downloadLocation = Download();

         ExtractToTarget(downloadLocation);

         Log.Info("JavaTask finished");
      }

      private void DetermineOsAndArchFromRID()
      {
         var rid = BaseConfig.RID;

         string ridOs = rid;
         string ridArch = null;

         int idx = rid.LastIndexOf('-');
         if (idx != -1)
         {
            ridOs = rid.Substring(0, idx);
            ridArch = rid.Substring(idx + 1);
         }

         if (ridOs == null || ridArch == null)
            throw new ArgumentException($"Unable to determine Os and Arch from RID: {rid}");

         Config.OS = new ApiOS[] {
            ApiOS.WIN,
            ApiOS.LINUX,
            ApiOS.MAC
         }.FirstOrDefault(os => os.Matcher.Invoke(ridOs))?.ApiPar;

         Config.Architecture = new ApiArch[] {
            ApiArch.X64,
            ApiArch.AARCH64
         }.FirstOrDefault(os => os.Matcher.Invoke(ridArch))?.ApiPar;

         Log.Info($"RID='{rid}' -> OS='{Config.OS}'; Arch='{Config.Architecture}'");
      }

      private void Init()
      {
         NonNullPar(nameof(Config.DestinationSubDir), Config.DestinationSubDir);
         NonNullPar(nameof(Config.Architecture), Config.Architecture);
         NonNullPar(nameof(Config.OS), Config.OS);
         NonNullPar(nameof(Config.ReleaseType), Config.ReleaseType);

         Config.DestinationSubDir = PathBuilder.BuildPath(nameof(Config.DestinationSubDir), Config.DestinationSubDir, BaseConfig.DestinationDir);
         Log.Info($"Set {nameof(Config.DestinationSubDir)}='{Config.DestinationSubDir}'");

         DirUtil.EnsureCreatedAndClean(Config.DestinationSubDir);
      }

      private void NonNullPar(string name, string value)
      {
         if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{name}[='{value}'] is invalid");

      }

      private string Download()
      {
         using var wc = new HttpClient();

         Uri downloadUri = GetDownloadURI();

         Log.Info($"Downloading from '{downloadUri}'");

         var metaJSON = wc.GetStringAsync(downloadUri).Result;

         Log.Info($"Got this: {metaJSON}");

         var results = JArray.Parse(metaJSON);
         if (!results.Any())
            throw new InvalidOperationException("No results");
         else if (results.Count() > 1)
            Log.Warn("Found more than one result in meta-json; taking first");

         var binaries = results.First()["binaries"].Children();
         if (!binaries.Any())
            throw new InvalidOperationException("No binary");
         else if (binaries.Count() > 1)
            Log.Warn("Found more than one binary in meta-json; taking first");

         var downloadPack = binaries.First()["package"];

         var link = downloadPack["link"].ToObject<string>();
         var name = downloadPack["name"].ToObject<string>();
         var size = downloadPack["size"].ToObject<int>();
         var sha256checksum = downloadPack["checksum"].ToObject<string>();

         var downloadLocation = Path.Combine(Path.GetTempPath(), name);

         CheckSumDownloader.SHA256.RunDownloadAsync(link, downloadLocation, size, sha256checksum).Wait();

         return downloadLocation;
      }

      private Uri GetDownloadURI()
      {
         var uri = new Uri(
         Config.RemoteBaseURL
            .Replace("{feature_version}", Config.FeatureVersion.ToString())
            .Replace("{release_type}", Config.ReleaseType)
         );

         if (!string.IsNullOrWhiteSpace(Config.Architecture))
            uri = uri.AddQuery("architecture", Config.Architecture);

         if (!string.IsNullOrWhiteSpace(Config.ImageType))
            uri = uri.AddQuery("image_type", Config.ImageType);

         if (!string.IsNullOrWhiteSpace(Config.JVMImpl))
            uri = uri.AddQuery("jvm_impl", Config.JVMImpl);

         if (!string.IsNullOrWhiteSpace(Config.OS))
            uri = uri.AddQuery("os", Config.OS);

         if (Config.PageSize >= 1)
            uri = uri.AddQuery("page_size", Config.PageSize.ToString());

         if (!string.IsNullOrWhiteSpace(Config.Project))
            uri = uri.AddQuery("project", Config.Project);

         if (!string.IsNullOrWhiteSpace(Config.SortOrder))
            uri = uri.AddQuery("sort_order", Config.SortOrder);

         if (!string.IsNullOrWhiteSpace(Config.Vendor))
            uri = uri.AddQuery("vendor", Config.Vendor);

         return uri;
      }

      private void ExtractToTarget(string path)
      {
         Log.Info("Extracting...");

         var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
         if(File.Exists(tempFile))
            File.Delete(tempFile);

         Directory.CreateDirectory(tempFile);
         var tempDir = tempFile;


         Log.Info($"Extracting '{path}'->'{tempDir}'");
         if (path.EndsWith(".zip"))
         {
            Log.Info("ExtractionMethod: ZIP");
            ZipFile.ExtractToDirectory(path, tempDir);
         }
         else if (path.EndsWith(".tar.gz"))
         {
            Log.Info("ExtractionMethod: TAR/GZ");
            Tar.ExtractTarGz(path, tempDir);
         }
         else
            throw new InvalidOperationException("Unknown compression type");

         List<Task> deleteTasks = new List<Task>
         {
            Task.Run(() =>
               {
                  Log.Info($"Deleting input/tempfile '{path}'");
                  File.Delete(path);
                  Log.Info($"Deleted '{path}'");
               })
         };

         var targetInfo = new DirectoryInfo(Config.DestinationSubDir);
         foreach (var dir in new DirectoryInfo(tempDir).GetDirectories())
         {
            Log.Info($"Moving internal (layer-1) directory '{dir}' into '{targetInfo}'");
            CopyFilesRecursively(dir, new DirectoryInfo(Config.DestinationSubDir));

            deleteTasks.Add(
               Task.Run(() =>
               {
                  Log.Info($"Deleting internal folder '{dir.FullName}'");
                  Directory.Delete(dir.FullName, true);
                  Log.Info($"Deleted '{dir.FullName}'");
               })
               );
         }

         Log.Info("Waiting for DeleteTasks");
         Task.WaitAll(deleteTasks.ToArray());
         Log.Info("All DeleteTasks finished");

         Log.Info($"Deleting tempfolder '{tempDir}'");
         Directory.Delete(tempDir, true);
         Log.Info($"Deleted '{tempDir}'");

         Log.Info("Extracting finished");
      }

      public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
      {
         foreach (DirectoryInfo dir in source.GetDirectories())
            CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
         foreach (FileInfo file in source.GetFiles())
            file.CopyTo(Path.Combine(target.FullName, file.Name));
      }

      /// <seealso cref="https://gist.github.com/ForeverZer0/a2cd292bd2f3b5e114956c00bb6e872b"/>
      public static class Tar
      {
         /// <summary>
         /// Extracts a <i>.tar.gz</i> archive to the specified directory.
         /// </summary>
         /// <param name="filename">The <i>.tar.gz</i> to decompress and extract.</param>
         /// <param name="outputDir">Output directory to write the files.</param>
         public static void ExtractTarGz(string filename, string outputDir)
         {
            using var stream = File.OpenRead(filename);

            ExtractTarGz(stream, outputDir);
         }

         /// <summary>
         /// Extracts a <i>.tar.gz</i> archive stream to the specified directory.
         /// </summary>
         /// <param name="stream">The <i>.tar.gz</i> to decompress and extract.</param>
         /// <param name="outputDir">Output directory to write the files.</param>
         public static void ExtractTarGz(Stream stream, string outputDir)
         {
            // A GZipStream is not seekable, so copy it first to a MemoryStream
            using var gzip = new GZipStream(stream, CompressionMode.Decompress);

            const int chunk = 4096;

            using var memStr = new MemoryStream();
            int read;
            var buffer = new byte[chunk];
            while ((read = gzip.Read(buffer, 0, buffer.Length)) > 0)
            {
               memStr.Write(buffer, 0, read);
            }

            memStr.Seek(0, SeekOrigin.Begin);
            ExtractTar(memStr, outputDir);
         }

         /// <summary>
         /// Extractes a <c>tar</c> archive to the specified directory.
         /// </summary>
         /// <param name="stream">The <i>.tar</i> to extract.</param>
         /// <param name="outputDir">Output directory to write the files.</param>
         public static void ExtractTar(Stream stream, string outputDir)
         {
            var buffer = new byte[100];
            while (true)
            {
               stream.Read(buffer, 0, 100);
               var name = Encoding.ASCII.GetString(buffer).Trim('\0');
               if (String.IsNullOrWhiteSpace(name))
                  break;
               stream.Seek(24, SeekOrigin.Current);
               stream.Read(buffer, 0, 12);
               var size = Convert.ToInt64(Encoding.UTF8.GetString(buffer, 0, 12).Trim('\0').Trim(), 8);


               stream.Seek(376L, SeekOrigin.Current);

               var output = Path.Combine(outputDir, name);
               if (!Directory.Exists(Path.GetDirectoryName(output)))
                  Directory.CreateDirectory(Path.GetDirectoryName(output));
               if (!name.EndsWith('/'))
               {
                  using var str = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write);

                  var buf = new byte[size];
                  stream.Read(buf, 0, buf.Length);
                  str.Write(buf, 0, buf.Length);
               }

               var pos = stream.Position;

               var offset = 512 - (pos % 512);
               if (offset == 512)
                  offset = 0;

               stream.Seek(offset, SeekOrigin.Current);
            }
         }
      }
   }
}

