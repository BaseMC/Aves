using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Aves.Shared
{
   public class DirUtil
   {
      private DirUtil()
      { }

      public static void EnsureCreated(string path)
      {
         if (Directory.Exists(path))
            return;

         Log.Debug($"Creating '{path}'");
         Directory.CreateDirectory(path);

         if (!Directory.Exists(path))
         {
            Thread.Sleep(50);

            for (int i = 0; i < 10 && !Directory.Exists(path); i++)
               Thread.Sleep(200);


            if (!Directory.Exists(path))
               throw new DirectoryNotFoundException("Failed to create directory");
         }

         Log.Verbose($"Created '{path}'");
      }

      public static void EnsureCreatedAndClean(string path)
      {
         if (Directory.Exists(path))
         {
            Log.Debug($"Deleting'{path}'");
            Directory.Delete(path, true);

            if (Directory.Exists(path))
            {
               Thread.Sleep(50);

               for (int i = 0; i < 10 && Directory.Exists(path); i++)
                  Thread.Sleep(200);


               if (Directory.Exists(path))
                  throw new DirectoryNotFoundException("Failed to clean directory");
            }
            Log.Verbose($"Deleted '{path}'");
         }
         EnsureCreated(path);
      }
   }
}
