using Aves.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Immutable;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Aves.Download
{
   /// <summary>
   /// Resolves the latest version
   /// </summary>
   public class LatestVersionResolver : Downloader
   {
      public static readonly ImmutableDictionary<string, string[]> IDFSTR = new Dictionary<string, string[]>() {
         {
            "release",
            new string[]
            {
               "LATEST",
               "LATEST_RELEASE",
               "RELEASE"
            }
         },
         {
            "snapshot",
            new string[]
            {
               "LATEST_SNAPSHOT",
               "SNAPSHOT"
            }
         }
      }.ToImmutableDictionary();


      public LatestVersionResolver(Configuration config) : base(config)
      { }

      public string ResolveVersion(string input)
      {
         var key = IDFSTR.FirstOrDefault(x => x.Value.Contains(input.ToUpper())).Key;
         if (key == null)
            return null;


         var downloaded = DownloadMainfest();

         try
         {
            return ResolveVersionFromMainfestFile(key);
         }
         catch (Exception ex)
         {
            Log.Error($"Failed to read '{Config.ManifestJsonFilePath}'", ex);

            if (downloaded)
               throw;

            Log.Warn($"Failed to executed {nameof(ResolveVersionFromMainfestFile)}; Mainfest was already downloaded, downloading it again, so it's up-to-date", ex);
            DownloadMainfest(true);

            return ResolveVersionFromMainfestFile(key);
         }
      }

      private string ResolveVersionFromMainfestFile(string key)
      {
         JObject jsonFile = null;
         try
         {
            jsonFile = JObject.Parse(File.ReadAllText(Config.ManifestJsonFilePath));
         }
         catch (Exception ex)
         {
            Log.Error($"Failed to read '{Config.ManifestJsonFilePath}'", ex);
         }

         var version = jsonFile["latest"].Value<string>(key);

         Log.Info($"Resolved ->'{version}'");

         return version;
      }

   }
}
