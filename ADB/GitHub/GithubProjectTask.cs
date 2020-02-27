using ADB.Config;
using ADB.Config.Github;
using Aves.Shared;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace ADB.GitHub
{
   public class GithubProjectTask
   {
      private GitHubClient Client { get; set; }

      private Configuration Configuration { get; set; }

      private GitHubProjectConfig GithubProjectConfig { get; set; }

      public GithubProjectTask(
         GitHubClient client,
         Configuration config,
         GitHubProjectConfig gitHubProjectConfig)
      {
         Client = client;
         Configuration = config;
         GithubProjectConfig = gitHubProjectConfig;
      }

      public void Run()
      {
         // "most recent non-prerelease, non-draft"
         var release = Client.Repository.Release.GetLatest(GithubProjectConfig.Owner, GithubProjectConfig.Repo).Result;

         TaskRunner.RunTasks(
           release.Assets
              .Select(asset =>
                  Downloader.RunDownloadAsync(
                     asset.BrowserDownloadUrl,
                     Path.Combine(Configuration.DestinationDir, asset.Name),
                     asset.Size)
                 )
              .ToArray()
           );
      }
   }
}
