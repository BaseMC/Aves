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

namespace Aves.Download
{
   public abstract class Downloader
   {
      protected Configuration Config { get; set; }


      protected Downloader(Configuration config)
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
         if (!File.Exists(Config.ManifestJsonFilePath) || Config.SuppressManifestDownload.TotalMilliseconds == 0 || forceDownload)
         {
            Download(Config.RemoteManifestJsonURL, Config.ManifestJsonFilePath);
            return true;
         }

         Log.Info($"{nameof(Config.SuppressManifestDownload)}={Config.SuppressManifestDownload}");

         var fileInfo = new FileInfo(Config.ManifestJsonFilePath);

         var timespanLastWriteTimeToNow = DateTime.UtcNow - fileInfo.LastWriteTimeUtc;

         Log.Info($"{nameof(fileInfo.LastWriteTimeUtc)}={fileInfo.LastWriteTimeUtc} -> {timespanLastWriteTimeToNow}");
         if (timespanLastWriteTimeToNow <= Config.SuppressManifestDownload)
            return false;

         Download(Config.RemoteManifestJsonURL, Config.ManifestJsonFilePath);
         return true;
      }

      protected Task ReadFromJsonAndDownload(JToken downloadInfo, string targetPath)
      {
         return DownloadAsync(
            downloadInfo["url"].ToObject<string>(),
            targetPath,
            downloadInfo["size"].ToObject<long>(),
            downloadInfo["sha1"].ToObject<string>(),
            Config.TryKeepExisting);
      }

      protected Task DownloadAsync(string srcURL, string targetPath, long size, string sha1, bool trykeepExisting, int retrys = 3)
      {
         if (trykeepExisting && File.Exists(targetPath))
         {
            Log.Debug($"Trying to keep existing file[='{targetPath}']");

            if (!CheckFile(targetPath, size, sha1))
            {
               Log.Info($"Existing file[='{targetPath}'] is faulty! Downloading new one...");
               return RunDownloadAsync(srcURL, targetPath, size, sha1, retrys);
            }

            Log.Info($"File[='{targetPath}']'s hash and size are ok! No Download required!");
            return Task.FromResult(0);
         }

         return RunDownloadAsync(srcURL, targetPath, size, sha1, retrys);

      }

      protected Task RunDownloadAsync(string srcURL, string targetPath, long size, string sha1, int retrys = 3)
      {
         return Task.Run(() =>
         {
            bool isok = true;
            do
            {
               Download(srcURL, targetPath);

               isok = CheckFile(targetPath, size, sha1);
               retrys--;

               if (!isok)
                  Log.Warn($"'{targetPath}'[URL='{srcURL}'] was faulty! Trys left: {retrys}");

            } while (!isok && retrys > 0);

            if (!isok)
               throw new InvalidOperationException($"Download of '{targetPath}'[URL='{srcURL}'] was faulty");
         });
      }

      protected void Download(string srcURL, string targetPath)
      {
         Log.Info($"Starting download: '{srcURL}'->'{targetPath}'");

         var sw = Stopwatch.StartNew();

         using (var webclient = new WebClient())
            webclient.DownloadFile(srcURL, targetPath);

         sw.Stop();
         Log.Debug($"Downloading from '{srcURL}' took {sw.ElapsedMilliseconds}ms");
      }

      protected bool CheckFile(string targetPath, long size, string sha1)
      {
         var exists = File.Exists(targetPath);
         Log.Debug($"'{targetPath}' EXISTS={exists}");

         if (!exists)
            return false;

         var fileInfo = new FileInfo(targetPath);
         Log.Debug($"'{targetPath}' SIZE[EXP='{size}';ACT='{fileInfo.Length}']");

         if (fileInfo.Length != size)
            return false;

         string hash = null;
         using (var fileStream = File.OpenRead(targetPath))
         using (var cryptoProvider = new SHA1CryptoServiceProvider())
         {
            hash = BitConverter
                    .ToString(cryptoProvider.ComputeHash(fileStream)).Replace("-", "").ToLower();
         }

         Log.Debug($"'{targetPath}' SHA1[EXP='{sha1}';ACT='{hash}']");

         if (hash != sha1)
            return false;

         return true;
      }
   }
}
