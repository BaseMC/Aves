using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aves.Util
{
   public class EmbeddedResExtracter
   {
      private string ManifestResourceName { get; set; }

      private string Exportpath { get; set; }

      public EmbeddedResExtracter(string manifestResourceName, string exportpath)
      {
         Contract.Requires(manifestResourceName != null);
         Contract.Requires(exportpath != null);

         ManifestResourceName = manifestResourceName;
         Exportpath = exportpath;
      }

      /// <summary>
      /// Checks if the external resource equals the embedded resource
      /// </summary>
      /// <returns></returns>
      public bool ExternalValid()
      {
         Log.Debug($"Checking if hash of EMB='{ManifestResourceName}' equals EXT='{Exportpath}'");
         if (!File.Exists(Exportpath))
         {
            Log.Debug($"External file does not exist: {Exportpath}");
            return false;
         }

         var embeddedHashTask = Task.Run(HashOfEmbedded);
         var externalHashTask = Task.Run(HashOfExternal);

         embeddedHashTask.Wait();
         externalHashTask.Wait();

         var embeddedHash = embeddedHashTask.Result;
         var externalHash = externalHashTask.Result;

         var result = embeddedHash != null && externalHash != null && embeddedHash == externalHash;
         Log.Debug($"EMB={embeddedHash} {(result ? "==" : "!=")} EXT={externalHash}");

         return result;

      }

      private string HashOfEmbedded()
      {
         HashAlgorithm hashAlgorithm = new SHA1CryptoServiceProvider();

         var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ManifestResourceName);
         if (stream == null)
            throw new ArgumentException("Unable to locate embedded file");

         return BitConverter
                    .ToString(hashAlgorithm.ComputeHash(stream))
                    .Replace("-", "")
                    .ToLower();
      }

      private string HashOfExternal()
      {
         if (!File.Exists(Exportpath))
            return null;

         using var fileStream = File.OpenRead(Exportpath);
         using var cryptoProvider = new SHA1CryptoServiceProvider();
         return BitConverter
            .ToString(cryptoProvider.ComputeHash(fileStream))
            .Replace("-", "")
            .ToLower();
      }

      /// <summary>
      /// Exports an embedded resource
      /// </summary>
      public void ExtractResource()
      {
         var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ManifestResourceName);
         if (stream == null)
            throw new ArgumentException("Unable to locate embedded file");

         Log.Info($"Extracting '{ManifestResourceName}'->'{Exportpath}'");

         using (var fileStream = File.Create(Exportpath))
         {
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fileStream);
         }

         Log.Info($"Extraction to '{Exportpath}' successful");
      }
   }
}
