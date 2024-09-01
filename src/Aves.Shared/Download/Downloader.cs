using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aves.Shared.Download
{
   public static class Downloader
   {
      public static void Download(string srcURL, string targetPath)
      {
         Log.Info($"Starting download: '{srcURL}'->'{targetPath}'");
         var sw = Stopwatch.StartNew();

         using (var httpClient = new HttpClient())
            DownloadFileTaskAsync(httpClient, srcURL, targetPath).Wait();

         sw.Stop();
         Log.Info($"Downloading from '{srcURL}' took {sw.ElapsedMilliseconds}ms");
      }

      private static async Task DownloadFileTaskAsync(HttpClient client, string uri, string FileName)
      {
         using var s = await client.GetStreamAsync(uri);
         using var fs = new FileStream(FileName, FileMode.Create);
         await s.CopyToAsync(fs);
      }
   }
}
