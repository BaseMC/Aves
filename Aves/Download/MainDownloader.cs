using Aves.Config;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Linq;
using System.Diagnostics.Contracts;
using Aves.Util;
using Aves.Shared;

namespace Aves.Download
{
   /// <summary>
   /// Downloads the main parts, mainly the jar, optional also the libarys and the logging configuration
   /// </summary>
   public class MainDownloader : DownloadTask
   {
      public MainDownloader(Configuration config) : base(config)
      {
      }

      public void Run()
      {
         Log.Info("Starting downloader");

         if (Config.ResolveOverNetwork)
            ProcessMainfest();

         ProcessVersion();

         Log.Info("Finished downloader");
      }

      private void ProcessMainfest()
      {
         bool downloaded = false;
         try
         {
            downloaded = DownloadMainfest();
         }
         catch (Exception ex)
         {
            Log.Warn("Failed to download mainfest", ex);
         }

         try
         {
            ProcessAndDownloadVersionMainfest();
         }
         catch (Exception ex)
         {
            if (downloaded)
               throw;

            Log.Warn($"Failed to execute {nameof(ProcessAndDownloadVersionMainfest)}; Mainfest was already downloaded, downloading it again, so it's up-to-date", ex);
            DownloadMainfest(true);

            ProcessAndDownloadVersionMainfest();
         }
         
      }

      private void ProcessAndDownloadVersionMainfest()
      {
         JObject jsonFile = null;
         try
         {
            jsonFile = JObject.Parse(File.ReadAllText(Config.ManifestJsonFilePath));
         }
         catch (Exception ex)
         {
            Log.Error($"Failed to read '{Config.ManifestJsonFilePath}'", ex);
            throw;
         }


         var versionDownloadUrl = ((JArray)jsonFile["versions"]).Children().FirstOrDefault(entry => entry["id"].ToObject<string>() == Config.Version)["url"].ToObject<string>();

         if (versionDownloadUrl == null)
            throw new InvalidOperationException($"No version[='{Config.Version}'] found in Mainfest/LauncherMeta");

         Downloader.Download(versionDownloadUrl, Config.VersionSrcJson);
      }

      private void ProcessVersion()
      {
         JObject jsonFile;
         try
         {
            jsonFile = JObject.Parse(File.ReadAllText(Config.VersionSrcJson));
         }
         catch (Exception ex)
         {
            Log.Error($"Failed to read '{Config.VersionSrcJson}'", ex);
            throw;
         }

         if (Config.ResolveOverNetwork)
         {
            if (Config.NetworkIncludeClientLibs)
               ProcessLibarys(jsonFile);

            if (Config.NetworkIncludeLogging)
               ProcessLogger(jsonFile);
         }


         ProcessVersionPayload(jsonFile);
      }

      private void ProcessVersionPayload(JObject jsonFile)
      {
         var downloads = jsonFile["downloads"];

         var tasklist = new List<Task>();

         foreach (var variant in Config.VariantConfigs.Where(v => v.Enabled))
         {
            tasklist.Add(ReadFromJsonAndDownload(downloads[variant.Key], variant.SrcJar));

            tasklist.Add(ReadFromJsonAndDownload(downloads[variant.MappingKey], variant.MappingFile));
         }

         var mastertask = Task.WhenAll(tasklist);
         mastertask.Wait();

         Log.Info("All tasks finished!");

         if (!mastertask.IsCompletedSuccessfully)
            throw new InvalidOperationException("One or more tasks failed");

         Log.Info("All tasks successful!");

      }

      private void ProcessLibarys(JObject jsonFile)
      {
         Log.Info("Starting downloading libs");

         DirUtil.EnsureCreated(Config.OutputDirLibs);

         var infoFilePath = Path.Combine(Config.OutputDirLibs, "_info.txt");
         Log.Info($"InfoFile is '{infoFilePath}'");

         if (File.Exists(infoFilePath))
            File.Delete(infoFilePath);

         foreach (var lib in jsonFile["libraries"])
         {
            var infoLines = new List<string>();

            Log.Info($"Processing {lib["name"]}");

            infoLines.Add($"=== {lib["name"]} ===");

            var downloads = lib["downloads"];

            var artifact = downloads["artifact"];
            
            var artifactFileNameWithExt = Path.GetFileName(artifact["path"].ToObject<string>());

            infoLines.Add($" - Artifact: {artifact["path"]} -> {artifactFileNameWithExt} ");

            var artifactPath = Path.Combine(Config.OutputDirLibs, artifactFileNameWithExt);

            DirUtil.EnsureCreated(Directory.GetParent(artifactPath).FullName);

            ReadFromJsonAndDownload(artifact, artifactPath).Wait();


            var classifNatives = downloads["classifiers"];
            if (classifNatives != null)
               foreach (JProperty classifier in classifNatives.Children<JProperty>())
               {
                  var classifierValue = classifier.Value;

                  var classifierFileNameWithExt = Path.GetFileName(classifierValue["path"].ToObject<string>());

                  infoLines.Add($" - Classifiers: {classifier.Name} -> {classifierFileNameWithExt}");

                  var classifierPath = Path.Combine(Config.OutputDirLibs, classifierFileNameWithExt);
                  DirUtil.EnsureCreated(Directory.GetParent(classifierPath).FullName);

                  ReadFromJsonAndDownload(classifierValue, classifierPath).Wait();
               }

            var rules = lib["rules"];
            if (rules != null)
               infoLines.Add($" - Rules: {rules.ToString()}");

            var natives = lib["natives"];
            if (natives != null)
               infoLines.Add($" - Natives: {natives.ToString()}");

            infoLines.Add("");

            File.AppendAllLines(infoFilePath, infoLines);
         }

         Log.Info("Finished downloading libs");
      }

      private void ProcessLogger(JObject jsonFile)
      {
         Log.Info("Starting downloading logging");

         DirUtil.EnsureCreated(Config.OutputDirLogging);

         foreach (var variantProp in jsonFile["logging"].Children<JProperty>())
         {
            var variantName = variantProp.Name;
            var variant = variantProp.Value;

            var currentDir = Path.Combine(Config.OutputDirLogging, variantName);

            Log.Info($"Processing '{variantName}' in '{currentDir}'");

            DirUtil.EnsureCreated(currentDir);

            var loggingMetaFilePath = Path.Combine(currentDir, "meta.txt");
            Log.Info($"InfoFile is '{loggingMetaFilePath}'");

            if (File.Exists(loggingMetaFilePath))
               File.Delete(loggingMetaFilePath);

            File.WriteAllLines(loggingMetaFilePath, new string[] {
               $"argument='{variant["argument"].ToObject<string>()}'",
               $"type='{variant["type"].ToObject<string>()}'",
            });

            var fileDownlaodInfo = variant["file"];

            ReadFromJsonAndDownload(fileDownlaodInfo, Path.Combine(currentDir, fileDownlaodInfo["id"].ToObject<string>()));
         }

         Log.Info("Finished downloading logging");
      }
   }
}
