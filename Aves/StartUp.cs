using Aves.CMD;
using Aves.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Linq;

namespace Aves
{
   public class StartUp
   {
      private CmdOption CmdOption { get; set; }

      private Configuration Config { get; set; } = new Configuration();

      public StartUp(CmdOption cmdOption)
      {
         CmdOption = cmdOption;
      }

      public void Start()
      {
         Contract.Requires(CmdOption != null);
         Log.Info($"Current directory is '{Directory.GetCurrentDirectory()}'");

         if (CmdOption.ConfigGenerationPath != null)
         {
            Log.Info("MODE: Write JSON Config");

            ReadCMDConfig();

            WriteJsonConfig();
            return;
         }

         Log.Info("MODE: Normal start");
         if (CmdOption.ConfigPath != null)
            ReadJsonConfig();

         ReadCMDConfig();

         DoStart();
      }

      public void WriteJsonConfig()
      {
         Log.Info("Writing json config");

         if (!string.IsNullOrWhiteSpace(CmdOption.ConfigGenerationPath))
            Config.Config.SavePath = CmdOption.ConfigGenerationPath;

         Log.Info($"Saving '{Config.Config.SavePath}'");
         Config.Save();

         Log.Info($"Saving: success");
      }

      public void ReadJsonConfig()
      {
         Log.Info("Reading json config");

         if (!string.IsNullOrWhiteSpace(CmdOption.ConfigPath))
            Config.Config.SavePath = CmdOption.ConfigPath;

         Log.Info($"Loading '{Config.Config.SavePath}'");
         Config.Load();

         Log.Info($"Loading: success");
      }

      public void ReadCMDConfig()
      {
         Log.Info("Doing config over commandline-args");

         if (!string.IsNullOrWhiteSpace(CmdOption.Version))
         {
            Log.Info($"SetInp: {nameof(Config.Version)}='{CmdOption.Version}'");
            Config.Version = CmdOption.Version;
         }

         if (!string.IsNullOrWhiteSpace(CmdOption.JavaExePath) && File.Exists(CmdOption.JavaExePath))
         {
            Log.Info($"SetInp: {nameof(Config.JavaExePath)}='{CmdOption.JavaExePath}'");
            Config.JavaExePath = CmdOption.JavaExePath;
         }

         if (CmdOption.Profiles != null && CmdOption.Profiles.Any())
         {
            var varConfigs = Config.VariantConfigs
               .Where(c => CmdOption.Profiles.Contains(c.Name))
               .Select(c =>
               {
                  c.Enabled = true;
                  return c;
               })
               .ToHashSet();

            Log.Info($"SetInp: {nameof(Config.VariantConfigs)}=[{string.Join(", ", varConfigs.Select(c => c.Name))}]<-[{string.Join(", ", CmdOption.Profiles)}]");
            Config.VariantConfigs = varConfigs;
         }
      }

      private void DoStart()
      {
         Log.Info("Starting");
         new Runner(Config).Run();
         Log.Info("Done");
      }
   }
}
