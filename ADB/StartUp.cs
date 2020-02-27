using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Linq;
using static CoreFrameworkBase.Config.JsonConfig;
using ADB.CMD;
using ADB.Config;
using ADB.Util;

namespace ADB
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

            FillSampleData();

            WriteJsonConfig();
            return;
         }

         Log.Info("MODE: Normal start");
         if (CmdOption.ConfigPath != null)
            ReadJsonConfig();

         ReadCMDConfig();

         ReadEnvConfig();

         DoStart();
      }

      protected void FillSampleData()
      {
         if (string.IsNullOrWhiteSpace(Config.DestinationDirPattern))
            Config.DestinationDirPattern = "bin/{BuildConfiguration}/netcoreapp3.1/{RID}/publish";

         if (Config.GitHubConfig.ProjectConfigs.Count == 0)
            Config.GitHubConfig.ProjectConfigs.Add(new ADB.Config.Github.GitHubProjectConfig()
            {
               Owner = "ghost",
               Repo = "YOUR_REPO_HERE"
            });
      }

      protected void WriteJsonConfig()
      {
         Log.Info("Writing json config");

         if (!string.IsNullOrWhiteSpace(CmdOption.ConfigGenerationPath))
            Config.Config.SavePath = CmdOption.ConfigGenerationPath;

         Log.Info($"Saving '{Config.Config.SavePath}'");
         Config.Save();

         Log.Info($"Saving: success");
      }

      protected void ReadJsonConfig()
      {
         Log.Info("Reading json config");

         if (!string.IsNullOrWhiteSpace(CmdOption.ConfigPath))
            Config.Config.SavePath = CmdOption.ConfigPath;

         Log.Info($"Loading '{Config.Config.SavePath}'");
         Config.Load(LoadFileNotFoundAction.THROW_EX);

         Log.Info($"Loading: success");
      }

      protected void ReadCMDConfig()
      {
         Log.Info("Doing config over commandline-args");

         if (!string.IsNullOrWhiteSpace(CmdOption.GITHUB_TOKEN))
         {
            Log.Info($"SetInp: {nameof(Config.GitHubConfig.GitHubToken)}='****'[SHA={HashUtil.GenerateSHA2(CmdOption.GITHUB_TOKEN)}]");
            Config.GitHubConfig.GitHubToken = CmdOption.GITHUB_TOKEN;
         }

         if (!string.IsNullOrWhiteSpace(CmdOption.DestinationDirPattern))
         {
            Log.Info($"SetInp: {nameof(Config.DestinationDirPattern)}='{CmdOption.DestinationDirPattern}'");
            Config.DestinationDirPattern = CmdOption.DestinationDirPattern;
         }

         if (!string.IsNullOrWhiteSpace(CmdOption.DestinationDir))
         {
            Log.Info($"SetInp: {nameof(Config.DestinationDir)}='{CmdOption.DestinationDir}'");
            Config.DestinationDir = CmdOption.DestinationDir;
         }

         if (!string.IsNullOrWhiteSpace(CmdOption.BuildConfiguration))
         {
            Log.Info($"SetInp: {nameof(Config.BuildConfiguration)}='{CmdOption.BuildConfiguration}'");
            Config.BuildConfiguration = CmdOption.BuildConfiguration;
         }

         if (!string.IsNullOrWhiteSpace(CmdOption.RID))
         {
            Log.Info($"SetInp: {nameof(Config.RID)}='{CmdOption.RID}'");
            Config.RID = CmdOption.RID;
         }
      }

      protected void ReadEnvConfig()
      {
         Log.Info("Reading environment config");

         var envGithubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
         if (!string.IsNullOrWhiteSpace(envGithubToken))
         {
            Log.Info($"SetInp: {nameof(Config.GitHubConfig.GitHubToken)}='****'[SHA={HashUtil.GenerateSHA2(CmdOption.GITHUB_TOKEN)}]");
            Config.GitHubConfig.GitHubToken = envGithubToken;
         }
      }

      protected void DoStart()
      {
         Log.Info("Starting");
         new Runner(Config).Run();
         Log.Info("Done");
      }
   }
}
