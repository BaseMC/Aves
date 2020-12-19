using Aves.Config;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Linq;
using System.Diagnostics.Contracts;
using Aves.Util;
using Aves.Shared;
using Aves.Shared.Download;

namespace Aves.Download
{
   public abstract class DownloadTask
   {
      protected Configuration Config { get; set; }


      protected DownloadTask(Configuration config)
      {
         Config = config;
      }


      /// <summary>
      /// Downloads the manifest from <see cref="Configuration.RemoteManifestJsonURL"/>
      /// </summary>
      /// <param name="forceDownload">Forces the download</param>
      /// <returns>true = downloaed</returns>
      protected bool DownloadMainfest(bool forceDownload = false)
      {
         if (!File.Exists(Config.ManifestJsonFilePath) || Config.SuppressManifestDownload.TotalMilliseconds <= 0 || forceDownload)
         {
            Downloader.Download(Config.RemoteManifestJsonURL, Config.ManifestJsonFilePath);
            return true;
         }

         Log.Info($"{nameof(Config.SuppressManifestDownload)}={Config.SuppressManifestDownload}");

         var fileInfo = new FileInfo(Config.ManifestJsonFilePath);

         if(fileInfo.LastAccessTimeUtc > DateTime.UtcNow)
         {
            Log.Warn($"File was written in the future: {fileInfo.LastAccessTimeUtc}");
            Downloader.Download(Config.RemoteManifestJsonURL, Config.ManifestJsonFilePath);
            return true;
         }

         var timespanLastWriteTimeToNow = DateTime.UtcNow - fileInfo.LastWriteTimeUtc;

         Log.Info($"{nameof(fileInfo.LastWriteTimeUtc)}={fileInfo.LastWriteTimeUtc} -> {timespanLastWriteTimeToNow}");
         if (timespanLastWriteTimeToNow <= Config.SuppressManifestDownload)
            return false;

         Downloader.Download(Config.RemoteManifestJsonURL, Config.ManifestJsonFilePath);
         return true;
      }

      protected Task ReadFromJsonAndDownload(JToken downloadInfo, string targetPath)
      {
         return CheckSumDownloader.SHA1.DownloadAsync(
            downloadInfo["url"].ToObject<string>(),
            targetPath,
            downloadInfo["size"].ToObject<long>(),
            Config.TryKeepExisting,
            downloadInfo["sha1"].ToObject<string>()
            );
      }

   }
}
