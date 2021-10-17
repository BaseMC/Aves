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
   public class MakeReadableVariant
   {
      public class MakeReadableVariantConfig
      {
         public Configuration Config { get; set; }

         public VariantConfig VariantConfig { get; set; }

         public List<IMRProvider> Providers { get; set; } = new List<IMRProvider>();
      }

      private MakeReadableVariantConfig MRConfig {get; set;}

      public MakeReadableVariant(MakeReadableVariantConfig mrconfig)
      {
         MRConfig = mrconfig;
      }

      public void Run()
      {
         Log.Info($"Start running variant '{MRConfig.VariantConfig.Name}'");

         foreach(var provider in MRConfig.Providers)
         {
            if(!provider.Run(MRConfig.Config, MRConfig.VariantConfig))
            {
               Log.Error($"'{MRConfig.VariantConfig.Name}': '{provider.GetType().Name}' failed!");
               break;
            }
         }

         Log.Info($"Finished variant '{MRConfig.VariantConfig.Name}'");
      }


   }
}
