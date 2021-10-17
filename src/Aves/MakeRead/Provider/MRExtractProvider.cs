using Aves.Config;
using Aves.Shared;
using Aves.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Aves.MakeRead.Provider
{
   /// <summary>
   /// Extracts a variant
   /// </summary>
   /// <remarks>
   /// parallel
   /// </remarks>
   public class MRExtractProvider : IMRProvider
   {
      public bool Run(Configuration config, VariantConfig variantConfig)
      {
         Log.Info($"Extracting Zip starting for '{variantConfig.Name}'");

         DirUtil.EnsureCreatedAndClean(variantConfig.OutputFilesDirFolder);

         ZipFile.ExtractToDirectory(variantConfig.DecompiledFile, variantConfig.OutputFilesDirFolder);

         Log.Info("Extracting Zip finished");

         return true;
      }

   }
}
