using Aves.Config;
using Aves.Shared;
using Aves.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Aves.MakeRead.Provider
{
   /// <summary>
   /// Deobfuscates a variant
   /// </summary>
   /// <remarks>
   /// not parallel
   /// </remarks>
   public class MRDeobfProvider : IMRProvider
   {
      private readonly object _lockObject = new object();

      public bool Run(Configuration config, VariantConfig variantConfig)
      {
         lock (_lockObject)
         {
            var deobfTask = Task.Run(() => RunDeobfuscation(config, variantConfig));
            deobfTask.Wait();

            return deobfTask.IsCompletedSuccessfully;
         }
      }

      private void RunDeobfuscation(Configuration config, VariantConfig variant)
      {
         Log.Info($"Deobfusctor starting for '{variant.Name}'");

         DirUtil.EnsureCreatedAndClean(Directory.GetParent(variant.DeObfuscatedFile).ToString());

         var formattedBaseCommand = config.BaseDeobfuscatorCommand
            .Replace("{Deobfuscator}", config.Deobfuscator)
            .Replace("{SrcJar}", variant.SrcJar)
            .Replace("{PatchFile}", variant.PatchFile)
            .Replace("{DeObfuscatedFile}", variant.DeObfuscatedFile);

         var command = $"{formattedBaseCommand}{(variant.ExcludedComponents.Count > 0 ? $" -ec {string.Join(",",variant.ExcludedComponents)}" : "")}";

         if (!ProcessUtil.RunProcess(config.JavaExePath, command, config.DeobfuscatorTimeout))
            throw new TaskCanceledException();

         Log.Info("Deobfusctor finished");
      }

   }
}
