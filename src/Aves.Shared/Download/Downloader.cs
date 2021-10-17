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
   public static class Downloader
   {
      public static void Download(string srcURL, string targetPath)
      {
         Log.Info($"Starting download: '{srcURL}'->'{targetPath}'");

         var sw = Stopwatch.StartNew();

         //Webclient overrides existing files
         using (var webclient = new WebClient())
            webclient.DownloadFile(srcURL, targetPath);

         sw.Stop();
         Log.Info($"Downloading from '{srcURL}' took {sw.ElapsedMilliseconds}ms");
      }

   }
}
