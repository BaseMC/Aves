using ADB.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Aves.Shared;
using Octokit;

namespace ADB.GitHub
{
   public class GithubTask
   {
      private GitHubClient Client { get; set; }

      private Configuration Config { get; set; }

      public GithubTask(Configuration config)
      {
         Config = config;
      }

      public void Run()
      {
         Log.Info("GithubTask starting");

         Client = new GitHubClient(new ProductHeaderValue(nameof(ADB)))
         {
#if !DEBUG
            Credentials = new Credentials(Config.GitHubConfig.GitHubToken, AuthenticationType.Bearer)
#endif
         };

         TaskRunner.RunTasks(
            Config.GitHubConfig.ProjectConfigs
               .Select(pconf => 
                  new Action(() => 
                     new GithubProjectTask(Client, Config, pconf).Run())
                  )
               .ToArray()
            );
         Log.Info("All tasks successfully finished!");

         Log.Info("GithubTask finished");
      }
   }
}
