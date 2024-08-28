using CoreFramework.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Aves.Config
{
   public class Configuration : JsonConfig
   {
      /// <summary>
      /// Trys to download the jars over the network; if false you have to add the files manually
      /// </summary>
      public bool ResolveOverNetwork { get; set; } = true;

      /// <summary>
      /// Remote URL to download the mainfest (includes all existing versions and the version foreach remote)
      /// </summary>
      public string RemoteManifestJsonURL { get; set; } = "https://launchermeta.mojang.com/mc/game/version_manifest.json";

      /// <summary>
      /// Suppresses the download from the <see cref="RemoteManifestJsonURL"/>,
      /// if the <see cref="ManifestJsonFilePath"/> exists and 
      /// the file is not older the given duration
      /// </summary>
      public TimeSpan SuppressManifestDownload { get; set; } = TimeSpan.FromMinutes(5);

      /// <summary>
      /// Local Manifest-Json-File; if <see cref="ResolveOverNetwork"/> is active and the remoteUrl is not avaible it will fallback to this (if it was saved previously)
      /// </summary>
      public string ManifestJsonFilePath { get; set; } = "version_manifest.json";

      /// <summary>
      /// Trys to keep the same files; Uses hashes
      /// </summary>
      public bool TryKeepExisting { get; set; } = true;

      /// <summary>
      /// Download client libarys into <see cref="Configuration.OutputDirLibs"/>
      /// </summary>
      public bool NetworkIncludeClientLibs { get; set; } = false;

      /// <summary>
      /// Download Logger Config into <see cref="Configuration.OutputDirLogging"/>
      /// </summary>
      public bool NetworkIncludeLogging { get; set; } = false;


      /// <summary>
      /// The version of the game that should be used, e.g. 19w36a
      /// </summary>
      /// <remarks>
      /// Only versions above 19w36a are supported! (and 1.14.4)
      /// </remarks>
      public string Version { get; set; }

      /// <summary>
      /// Name of the .json file; relative to <see cref="RawDirectory"/>
      /// </summary>
      /// <example>
      /// 19w36a.json
      /// </example>
      public string VersionSrcJson { get; set; } = "package.json";

      /// <summary>
      /// Variants
      /// </summary>
      public HashSet<VariantConfig> VariantConfigs { get; set; } = new HashSet<VariantConfig>(VariantConfig.DEFAULT_VARIANTS);

      /// <summary>
      /// PatchFiles: mapping files contain content, that creates malformed Java-Code when decompiling it; true = fix this
      /// </summary>
      /// <see cref="PatchFile.PatchFileMaker.RenameNameOfPatchClass(string)"/>
      public bool MakeJavaCompatible { get; set; } = true;

      /// <summary>
      /// Java Installation Dir
      /// <para/>
      /// if not set autodetect; default is provided JRE under jre/bin/java.exe (win) or jre/bin/java (linux)
      /// <para/>
      /// if not absloute: relative to <see cref="AppDomain.CurrentDomain.BaseDirectory"/>
      /// </summary>
      /// <example>
      /// "C:\Program Files\Java\21.0_XXX\bin\java.exe"
      /// </example>
      public string JavaExePath { get; set; } = Path.Combine("jre", "bin", RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "java.exe" : "java");

      /// <summary>
      /// Deobfuscator; if not absloute: relative to <see cref="System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName"/>
      /// </summary>
      public string Deobfuscator { get; set; } = "javgent-standalone.jar";

      /// <summary>
      /// Timeout for Deobfuscator, null = no timeout
      /// </summary>
      public TimeSpan? DeobfuscatorTimeout { get; set; } = TimeSpan.FromMinutes(5);

      /// <summary>
      /// Basic Deobfuscator command
      /// </summary>
      public string BaseDeobfuscatorCommand { get; set; } = "-jar \"{Deobfuscator}\" -s \"{SrcJar}\" -m \"{PatchFile}\" -o \"{DeObfuscatedFile}\"";

      /// <summary>
      /// Decompiler; if not absloute: relative to <see cref="System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName"/>
      /// </summary>
      public string Decompiler { get; set; } = "vineflower.jar";

      /// <summary>
      /// Timeout for Decompiler, null = no timeout
      /// </summary>
      public TimeSpan? DecompilerTimeout { get; set; } = TimeSpan.FromMinutes(30);

      /// <summary>
      /// Basic Decompiler command
      /// </summary>
      /// <remarks>
      /// <list type="bullet">
      ///   <item>dgs = decompile generic signatures</item>
      ///   <item>rsy = hide synthetic class members</item>
      ///   <item>lit = output numeric literals "as-is"</item>
      ///   <item>mpm = maximum allowed processing time per decompiled method, in seconds. 0 means no upper limit</item>
      ///   <item>ren = rename ambiguous (resp. obfuscated) classes and class elements 
      ///      <b>IMPORTANT: DON'T USE IT</b> 
      ///      - it will cause decompilation failures like NPE in ClassesProcessor$ClassNode, 
      ///      because the short methodnames like "or" are getting renamed to "method_123"
      ///  </item>
      /// </list>
      /// </remarks>
      /// <seealso cref="https://github.com/BaseMC/avesflower"/>
      public string BaseDecompilerCommand { get; set; } = "-jar \"{Decompiler}\" -dgs=1 -rsy=1 -lit=1 -mpm=60 \"{SrcFile}\" \"{TargetDir}\"";

#region Workdir

      /// <summary>
      /// Working Directory
      /// </summary>
      public string WorkingDirectory { get; set; } = "workingDir";

      /// <summary>
      /// Working Directory for a version; if not set it's based on the <see cref="Version"/>; if not absloute: relative to <see cref="WorkingDirectory"/>
      /// </summary>
      /// <example>
      /// 19w36a
      /// </example>
      public string VersionWorkingDirectory { get; set; }

      /// <summary>
      /// Input files; if not absloute: relative to <see cref="VersionWorkingDirectory"/> <para/>
      /// (may) holds
      /// <see cref="VersionSrcJson"/>,
      /// <see cref="VersionSrcJarClient"/>,
      /// <see cref="VersionSrcJarServer"/>
      /// </summary>
      public string RawDirectory { get; set; } = "raw";

      /// <summary>
      /// Directory for MappingFiles; if not absloute: relative to <see cref="VersionWorkingDirectory"/>
      /// </summary>
      public string MappingFilesDir { get; set; } = "mappings";

      /// <summary>
      /// Directory for PatchFiles; if not absloute: relative to <see cref="VersionWorkingDirectory"/>
      /// </summary>
      public string PatchFilesDir { get; set; } = "patch";

      /// <summary>
      /// Output Directory for deobfuscated files (.jar); if not absloute: relative to <see cref="VersionWorkingDirectory"/>
      /// </summary>
      public string DeObfuscatedDirectory { get; set; } = "deof";

      /// <summary>
      /// Output Directory for decompiled files (.jar); if not absloute: relative to <see cref="VersionWorkingDirectory"/>
      /// </summary>
      public string DecompiledDirectory { get; set; } = "dec";

      /// <summary>
      /// Output Directory; if not absloute: relative to <see cref="VersionWorkingDirectory"/>
      /// </summary>
      public string OutputDirectory { get; set; } = "output";

      /// <summary>
      /// Output Directory for non source code files; if not absloute: relative to <see cref="VersionWorkingDirectory"/>
      /// </summary>
      public string OutputDirectoryMetaFiles { get; set; } = "output-meta";

      /// <summary>
      /// Output Directory for (currently client only) libs; if not absloute: relative to <see cref="OutputDirectoryMetaFiles"/>
      /// </summary>
      public string OutputDirLibs { get; set; } = "libs";

      /// <summary>
      /// Output Directory for logging configurations; if not absloute: relative to <see cref="OutputDirectoryMetaFiles"/>
      /// </summary>
      public string OutputDirLogging { get; set; } = "logging";

      #endregion Workdir
   }
}
