using CommandLine;
using Aves.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Aves.CMD
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

      #region SetableProperties
      [Option('v', "version", HelpText = "version that should be used; e.g. 1.15")]
      public string Version { get; set; } = null;

      [Option('j', "java", HelpText = "path to java.exe (requires Java 11+)")]
      public string JavaExePath { get; set; } = null;

      [Option('p', "profiles", HelpText = "name of the profiles/variants which should get executed (they are also executed if they are disabled); Default: client only")]
      public IEnumerable<string> Profiles { get; set; } = null;
      #endregion SetableProperties

   }
}
