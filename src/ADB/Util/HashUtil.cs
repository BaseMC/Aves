using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ADB.Util
{
   public static class HashUtil
   {
      public static string GenerateSHA2(string input)
      {
         StringBuilder Sb = new StringBuilder();

         using (var hash = SHA256.Create())
         {
            Encoding enc = Encoding.UTF8;
            byte[] result = hash.ComputeHash(enc.GetBytes(input));

            foreach (byte b in result)
               Sb.Append(b.ToString("x2"));
         }

         return Sb.ToString();
      }
   }
}
