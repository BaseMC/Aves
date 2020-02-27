using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ADB.CMD
{
   /// <summary>
   /// Possible options that can be used when calling over commandline
   /// </summary>
   public class CmdOption
   {
      [Option('l', "logfile", Default = false, HelpText = "Logs into ./logs")]
      public bool LogToFile { get; set; } = false;

      #region JSON based Config
      [Option('c', "config", HelpText = "path to the configuration file; if not set: using default internal config")]
      public string ConfigPath { get; set; } = null;

      [Option("genconf", HelpText = "generates default config in mentioned path; if not set: using default internal config")]
      public string ConfigGenerationPath { get; set; } = null;
      #endregion JSON based Config

      #region SetableBuildProperties
      [Option('g', "GITHUB_TOKEN", HelpText = "GITHUB_TOKEN")]
      public string GITHUB_TOKEN { get; set; } = null;

      [Option("bc", HelpText = "Build Configuration; e.g. Release or Debug")]
      public string BuildConfiguration { get; set; } 

      /// <seealso cref="https://docs.microsoft.com/de-de/dotnet/core/rid-catalog"/>
      [Option('r',"rid", HelpText = "Runtime Identifier")]
      public string RID { get; set; }

      [Option("dest", HelpText = "Base-Directory where the files should be put; automatically configured by default")]
      public string DestinationDir { get; set; } = null;

      [Option("destdirpat", HelpText = "Destination Directory Pattern")]
      public string DestinationDirPattern { get; set; } = null;

      #endregion SetableBuildProperties

   }
}
