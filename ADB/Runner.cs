using ADB.Config;
using ADB.GitHub;
using ADB.Java;
using ADB.Util;
using Aves.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ADB
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
#if !DEBUG
         if (string.IsNullOrWhiteSpace(Config.GitHubConfig.GitHubToken))
            throw new ArgumentException($"{nameof(Config.GitHubConfig.GitHubToken)}[='****'] is invalid");
#endif

         if (string.IsNullOrWhiteSpace(Config.BuildConfiguration))
            throw new ArgumentException($"{nameof(Config.BuildConfiguration)}[='{Config.BuildConfiguration}'] is invalid");

         if (string.IsNullOrWhiteSpace(Config.RID))
            throw new ArgumentException($"{nameof(Config.RID)}[='{Config.RID}'] is invalid");

         if (string.IsNullOrWhiteSpace(Config.DestinationDir))
         {
            if (string.IsNullOrWhiteSpace(Config.DestinationDirPattern))
               throw new ArgumentException($"{nameof(Config.DestinationDirPattern)}[='{Config.DestinationDirPattern}'] is invalid");
            var destDir = Config.DestinationDirPattern.Replace('/', Path.DirectorySeparatorChar)
               .Replace($"{{{nameof(Config.BuildConfiguration)}}}", Config.BuildConfiguration)
               .Replace($"{{{nameof(Config.RID)}}}", Config.RID);

            Log.Info($"SetInp: {nameof(Config.DestinationDir)}='{destDir}'");
            Config.DestinationDir = destDir;
         }

         DirUtil.EnsureCreated(Config.DestinationDir);

      }

      #endregion Init

      public void Run()
      {
         Log.Info("Starting run");

         TaskRunner.RunTasks(
            () => new GithubTask(Config).Run(),
            () => new JavaTask(Config).Run());
         Log.Info("All tasks successfully finished!");

         Log.Info("Finished run");
      }


   }
}
