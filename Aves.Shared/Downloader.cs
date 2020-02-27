using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aves.Shared
{
   public static class Downloader
   {
      public static Task DownloadAsync(string srcURL, string targetPath, long size, bool trykeepExisting, string sha1 = null, int retrys = 3)
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

      public static Task RunDownloadAsync(string srcURL, string targetPath, long size, string sha1 = null, int retrys = 3)
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

      public static void Download(string srcURL, string targetPath)
      {
         Log.Info($"Starting download: '{srcURL}'->'{targetPath}'");

         var sw = Stopwatch.StartNew();

         //Webclient overrides existing files
         using (var webclient = new WebClient())
            webclient.DownloadFile(srcURL, targetPath);

         sw.Stop();
         Log.Debug($"Downloading from '{srcURL}' took {sw.ElapsedMilliseconds}ms");
      }

      public static bool CheckFile(string targetPath, long size, string sha1 = null)
      {
         var exists = File.Exists(targetPath);
         Log.Debug($"'{targetPath}' EXISTS={exists}");

         if (!exists)
            return false;

         var fileInfo = new FileInfo(targetPath);
         Log.Debug($"'{targetPath}' SIZE[EXP='{size}';ACT='{fileInfo.Length}']");

         if (fileInfo.Length != size)
            return false;

         if (sha1 != null)
         {
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
         }

         return true;
      }
   }
}
