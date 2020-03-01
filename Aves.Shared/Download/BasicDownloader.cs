using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aves.Shared.Download
{
   public class BasicDownloader
   {
      public Task DownloadAsync(string srcURL, string targetPath, long size, bool trykeepExisting, int retrys = 3)
      {
         if (trykeepExisting && File.Exists(targetPath))
         {
            Log.Debug($"Trying to keep existing file[='{targetPath}']");

            if (!CheckFile(targetPath, size))
            {
               Log.Info($"Existing file[='{targetPath}'] is faulty! Downloading new one...");
               return RunDownloadAsync(srcURL, targetPath, size, retrys);
            }

            Log.Info($"File[='{targetPath}']'s hash and size are ok! No Download required!");
            return Task.FromResult(0);
         }

         return RunDownloadAsync(srcURL, targetPath, size, retrys);

      }

      public Task RunDownloadAsync(string srcURL, string targetPath, long size, int retrys = 3)
      {
         return Task.Run(() =>
         {
            bool isok = true;
            do
            {
               Downloader.Download(srcURL, targetPath);

               isok = CheckFile(targetPath, size);
               retrys--;

               if (!isok)
                  Log.Warn($"'{targetPath}'[URL='{srcURL}'] was faulty! Trys left: {retrys}");

            } while (!isok && retrys > 0);

            if (!isok)
               throw new InvalidOperationException($"Download of '{targetPath}'[URL='{srcURL}'] was faulty");
         });
      }

      public bool CheckFile(string targetPath, long size)
      {
         var exists = File.Exists(targetPath);
         Log.Debug($"'{targetPath}' EXISTS={exists}");

         if (!exists)
            return false;

         var fileInfo = new FileInfo(targetPath);
         Log.Debug($"'{targetPath}' SIZE[EXP='{size}';ACT='{fileInfo.Length}']");

         if (fileInfo.Length != size)
            return false;

         return true;
      }

      public static BasicDownloader Default => new BasicDownloader();


   }
}
