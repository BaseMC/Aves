using Aves.Config;
using Aves.MakeRead.Provider;
using Aves.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aves.MakeRead
{
   public class MakeReadable
   {
      private Configuration Config { get; set; }

      public MakeReadable(Configuration config)
      {
         Config = config;
      }

      private readonly List<IMRProvider> providers = new List<IMRProvider>()
      {
         new MRDeobfProvider(),
         new MRDecompProvider(),
         new MRExtractProvider()
      };

      public void Run()
      {
         Log.Info("Starting making readable");

         var tasklist = new List<Task>();
         foreach (var variant in Config.VariantConfigs.Where(v => v.Enabled))
         {
            tasklist.Add(
               Task.Run(() =>
               {
                  new MakeReadableVariant(new MakeReadableVariant.MakeReadableVariantConfig()
                  {
                     Config = Config,
                     VariantConfig = variant,
                     Providers = providers
                  })
                  .Run();
               })
            );
         }
         Log.Info("Waiting for subtasks");
         Task.WaitAll(tasklist.ToArray());

         Log.Info("Finished making readable");
      }


   }
}
