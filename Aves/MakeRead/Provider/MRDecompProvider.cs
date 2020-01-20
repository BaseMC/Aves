using Aves.Config;
using Aves.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Aves.MakeRead.Provider
{
   /// <summary>
   /// Decompiles a variant
   /// </summary>
   /// <remarks>
   /// not parallel
   /// </remarks>
   public class MRDecompProvider : IMRProvider
   {
      private readonly object _lockObject = new object();

      public bool Run(Configuration config, VariantConfig variantConfig)
      {
         lock (_lockObject)
         {
            var decompTask = Task.Run(() => RunDecompile(config, variantConfig));
            decompTask.Wait();

            return decompTask.IsCompletedSuccessfully;
         }
      }

      private void RunDecompile(Configuration config, VariantConfig variant)
      {
         if (config.Decompiler == null)
         {
            var res = new EmbeddedResExtracter($"{nameof(Aves)}.{Configuration.EMBEDDED_Decompiler}", Configuration.EMBEDDED_Decompiler);
            if (!res.ExternalValid())
               res.ExtractResource();
         }

         Log.Info($"Decompiler starting for '{variant.Name}'");

         var parent = Directory.GetParent(variant.DecompiledFile).ToString();
         DirUtil.EnsureCreatedAndClean(parent);

         var command = string.Format(config.BaseDecompileCommand,
            config.Decompiler ?? Configuration.EMBEDDED_Decompiler,
            variant.DeObfuscatedFile,
            parent);

         if (!ProcessUtil.RunProcess(config.JavaExePath, command, config.DecompilerTimeout))
            throw new TaskCanceledException();

         Log.Info("Decompiler finished");
      }
   }
}
