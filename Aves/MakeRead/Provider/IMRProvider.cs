using Aves.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aves.MakeRead.Provider
{
   /// <summary>
   /// Interface for an runnable "extension" to make the the code readable
   /// </summary>
   public interface IMRProvider
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="config"></param>
      /// <param name="variantConfig"></param>
      /// <returns>true = success</returns>
      bool Run(Configuration config, VariantConfig variantConfig);
   }
}
