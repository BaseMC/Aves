using Aves.Config;
using Aves.Download;
using Aves.MakeRead;
using Aves.PatchFile;
using Aves.Shared;
using Aves.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Aves
{
   public class Runner
   {
      protected Configuration Config { get; set; }

      public Runner(Configuration configuration)
      {
         Config = configuration;


         Init();

      }

      #region Init

      private void Init()
      {
         //Check inputs
         if (string.IsNullOrWhiteSpace(Config.Version))
            throw new ArgumentException($"{nameof(Config.Version)}[='{Config.Version}'] is invalid");

         if (string.IsNullOrWhiteSpace(Config.BaseDeobfuscatorCommand))
            throw new ArgumentException($"{nameof(Config.BaseDeobfuscatorCommand)}[='{Config.BaseDeobfuscatorCommand}'] is invalid");

         if (string.IsNullOrWhiteSpace(Config.Deobfuscator))
            throw new ArgumentException($"{nameof(Config.Deobfuscator)}[='{Config.Deobfuscator}'] is invalid");

         if (string.IsNullOrWhiteSpace(Config.BaseDecompileCommand))
            throw new ArgumentException($"{nameof(Config.BaseDecompileCommand)}[='{Config.BaseDecompileCommand}'] is invalid");

         if (string.IsNullOrWhiteSpace(Config.Decompiler))
            throw new ArgumentException($"{nameof(Config.Decompiler)}[='{Config.Decompiler}'] is invalid");

         if (Config.WorkingDirectory == null)
            throw new ArgumentException($"{nameof(Config.WorkingDirectory)} is not set");

         // Ensure Dependency executables exist
         Config.JavaExePath = TryFindJavaExe();
         if (string.IsNullOrEmpty(Config.JavaExePath))
            throw new ArgumentException($"JavaExePath[='{Config.JavaExePath}'] is invalid");

         Log.Info($"Set {nameof(Config.JavaExePath)}='{Config.JavaExePath}'");
         
         Config.Deobfuscator = BuildPathForExecutableLocation(nameof(Config.Deobfuscator), Config.Deobfuscator);
         Log.Info($"Set {nameof(Config.Deobfuscator)}='{Config.Deobfuscator}'");

         Config.Decompiler = BuildPathForExecutableLocation(nameof(Config.Decompiler), Config.Decompiler);
         Log.Info($"Set {nameof(Config.Decompiler)}='{Config.Decompiler}'");

         //Ensure Directories and files

         DirUtil.EnsureCreated(Config.WorkingDirectory);

         if (Config.ResolveOverNetwork)
         {
            Config.ManifestJsonFilePath = BuildPath(nameof(Config.ManifestJsonFilePath), Config.ManifestJsonFilePath, Config.WorkingDirectory);
            Log.Info($"Set {nameof(Config.ManifestJsonFilePath)}='{Config.ManifestJsonFilePath}'");

            Config.Version = new LatestVersionResolver(Config).ResolveVersion(Config.Version) ?? Config.Version;
         }
         else
            Config.ManifestJsonFilePath = null;


         Config.VersionWorkingDirectory = BuildPath(nameof(Config.VersionWorkingDirectory), Config.VersionWorkingDirectory ?? Config.Version, Config.WorkingDirectory);
         Log.Info($"Set {nameof(Config.VersionWorkingDirectory)}='{Config.VersionWorkingDirectory}'");

         DirUtil.EnsureCreated(Config.VersionWorkingDirectory);


         Config.RawDirectory = BuildPath(nameof(Config.RawDirectory), Config.RawDirectory, Config.VersionWorkingDirectory);
         Log.Info($"Set {nameof(Config.RawDirectory)}='{Config.RawDirectory}'");

         DirUtil.EnsureCreated(Config.RawDirectory);

         Config.VersionSrcJson = BuildPath(nameof(Config.VersionSrcJson), Config.VersionSrcJson, Config.RawDirectory);
         Log.Info($"Set {nameof(Config.VersionSrcJson)}='{Config.VersionSrcJson}'");


         Config.MappingFilesDir = BuildPath(nameof(Config.MappingFilesDir), Config.MappingFilesDir, Config.VersionWorkingDirectory);
         Log.Info($"Set {nameof(Config.MappingFilesDir)}='{Config.MappingFilesDir}'");

         DirUtil.EnsureCreated(Config.MappingFilesDir);


         Config.PatchFilesDir = BuildPath(nameof(Config.PatchFilesDir), Config.PatchFilesDir, Config.VersionWorkingDirectory);
         Log.Info($"Set {nameof(Config.PatchFilesDir)}='{Config.PatchFilesDir}'");

         DirUtil.EnsureCreated(Config.PatchFilesDir);


         Config.DeObfuscatedDirectory = BuildPath(nameof(Config.DeObfuscatedDirectory), Config.DeObfuscatedDirectory, Config.VersionWorkingDirectory);
         Log.Info($"Set {nameof(Config.DeObfuscatedDirectory)}='{Config.DeObfuscatedDirectory}'");

         DirUtil.EnsureCreated(Config.DeObfuscatedDirectory);

         Config.DecompiledDirectory = BuildPath(nameof(Config.DecompiledDirectory), Config.DecompiledDirectory, Config.VersionWorkingDirectory);
         Log.Info($"Set {nameof(Config.DecompiledDirectory)}='{Config.DecompiledDirectory}'");

         DirUtil.EnsureCreated(Config.DecompiledDirectory);


         Config.OutputDirectory = BuildPath(nameof(Config.OutputDirectory), Config.OutputDirectory, Config.VersionWorkingDirectory);
         Log.Info($"Set {nameof(Config.OutputDirectory)}='{Config.OutputDirectory}'");

         DirUtil.EnsureCreated(Config.OutputDirectory);


         foreach (var variant in Config.VariantConfigs)
         {
            if (!variant.Enabled)
            {
               Log.Debug($"Skipping variant {variant.Name}: not enabled");
               continue;
            }

            var basePathRaw = Path.Combine(Config.RawDirectory, variant.Name);
            variant.SrcJar = BuildPath($"{variant.Name}/{nameof(variant.SrcJar)}", variant.SrcJar, basePathRaw);

            DirUtil.EnsureCreated(basePathRaw);


            var basePathMapping = Path.Combine(Config.MappingFilesDir, variant.Name);
            variant.MappingFile = BuildPath($"{variant.Name}/{nameof(variant.MappingFile)}", variant.MappingFile, basePathMapping);

            DirUtil.EnsureCreated(basePathMapping);

            var basePathPatch = Path.Combine(Config.PatchFilesDir, variant.Name);
            variant.PatchFile = BuildPath($"{variant.Name}/{nameof(variant.PatchFile)}", variant.PatchFile, basePathPatch);

            DirUtil.EnsureCreated(basePathPatch);


            var basePathDeobf = Path.Combine(Config.DeObfuscatedDirectory, variant.Name);
            variant.DeObfuscatedFile = BuildPath($"{variant.Name}/{nameof(variant.DeObfuscatedFile)}", variant.DeObfuscatedFile, basePathDeobf);

            DirUtil.EnsureCreated(basePathDeobf);


            var basePathDisas = Path.Combine(Config.DecompiledDirectory, variant.Name);
            variant.DecompiledFile = BuildPath($"{variant.Name}/{nameof(variant.DecompiledFile)}", variant.DecompiledFile, basePathDisas);

            DirUtil.EnsureCreated(basePathDisas);


            variant.OutputFilesDirFolder = BuildPath(nameof(variant.OutputFilesDirFolder), variant.OutputFilesDirFolder, Config.OutputDirectory);

            DirUtil.EnsureCreated(variant.OutputFilesDirFolder);
         }

         if (Config.NetworkIncludeClientLibs || Config.NetworkIncludeLogging)
         {
            Config.OutputDirectoryMetaFiles = BuildPath(nameof(Config.OutputDirectoryMetaFiles), Config.OutputDirectoryMetaFiles, Config.VersionWorkingDirectory);
            Log.Info($"Set {nameof(Config.OutputDirectoryMetaFiles)}='{Config.OutputDirectoryMetaFiles}'");

            DirUtil.EnsureCreated(Config.OutputDirectoryMetaFiles);

            if (Config.NetworkIncludeClientLibs)
            {
               Config.OutputDirLibs = BuildPath(nameof(Config.OutputDirLibs), Config.OutputDirLibs, Config.OutputDirectoryMetaFiles);
               Log.Info($"Set {nameof(Config.OutputDirLibs)}='{Config.OutputDirLibs}'");
            }

            if (Config.NetworkIncludeLogging)
            {
               Config.OutputDirLogging = BuildPath(nameof(Config.OutputDirLogging), Config.OutputDirLogging, Config.OutputDirectoryMetaFiles);
               Log.Info($"Set {nameof(Config.OutputDirLogging)}='{Config.OutputDirLogging}'");
            }
         }

      }

      private string TryFindJavaExe()
      {
         string javaExePath = BuildPathForExecutableLocation(nameof(Config.JavaExePath), Config.JavaExePath);
         if (!string.IsNullOrEmpty(javaExePath))
         {
            if (File.Exists(javaExePath))
            {
               Log.Info("Found valid location of java in configuration");
               return javaExePath;
            }
            else
               Log.Warn($"Found location of java[='{javaExePath}'] in configuration, but was invalid. Trying to find java...");
         }

         javaExePath = Environment.GetEnvironmentVariable("JAVA_HOME");
         if (!string.IsNullOrEmpty(javaExePath))
         {
            javaExePath = Path.Combine(javaExePath, "bin", "java.exe");
            if (File.Exists(javaExePath))
            {
               Log.Info("Found valid JAVA_HOME in EnvironmentVars");
               return javaExePath;
            }
         }

         if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
         {
            Log.Info("Running on commandline[CMD]");
            javaExePath = CommandLineSupport.CMD("where java");
         }
         else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) //Support for OS X is experimental
         {
            Log.Info("Running on commandline[BASH]");
            javaExePath = CommandLineSupport.Bash("where java");
         }

         if (!string.IsNullOrEmpty(javaExePath))
         {
            javaExePath = javaExePath.Replace("\r", "").Replace("\n", "");
            if (File.Exists(javaExePath))
            {
               Log.Info("Found valid location of java by commandline");
               return javaExePath;
            }
         }

         return null;
      }

      private string BuildPath(string name, string path, string relativeParent)
      {
         return PathBuilder.BuildPath(name, path, relativeParent);
      }

      private string BuildPathForExecutableLocation(string name, string path)
      {
         return PathBuilder.BuildPath(name, path, Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
      }

      #endregion Init

      public void Run()
      {
         Log.Info("Starting run");

         var downloader = new MainDownloader(Config);
         downloader.Run();

         var mapper = new PatchFileMaker(Config);
         mapper.Run();

         var dd = new MakeReadable(Config);
         dd.Run();

         Log.Info("Finished run");
      }


   }
}
