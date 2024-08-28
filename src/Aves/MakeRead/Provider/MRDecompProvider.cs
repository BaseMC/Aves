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
            var decompTask = Task.Run(() => RunDecompiler(config, variantConfig));
            decompTask.Wait();

            return decompTask.IsCompletedSuccessfully;
         }
      }

      private void RunDecompiler(Configuration config, VariantConfig variant)
      {
         Log.Info($"Decompiler starting for '{variant.Name}'");

         DirUtil.EnsureCreatedAndClean(variant.OutputFilesDirFolder);

         var formattedBaseCommand = config.BaseDecompilerCommand
            .Replace("{Decompiler}", config.Decompiler)
            .Replace("{SrcFile}", variant.DeObfuscatedFile)
            .Replace("{TargetDir}", Path.GetFullPath(variant.OutputFilesDirFolder));

         var command = $"{formattedBaseCommand}";

         if (!ProcessUtil.RunProcess(config.JavaExePath, command, config.DecompilerTimeout))
            throw new TaskCanceledException();

         Log.Info("Decompiler finished");
      }
   }
}
