using System;
using System.Collections.Generic;
using System.Text;

namespace ADB.Java
{
   /// <seealso cref="https://docs.microsoft.com/de-de/dotnet/core/rid-catalog"/>
   public class ApiArch
   {
      public static readonly ApiArch X64 = new ApiArch("x64", s => "x64".Equals(s));
      public static readonly ApiArch AARCH64 = new ApiArch("aarch64", s => "arm".Equals(s) || "arm64".Equals(s));

      public string ApiPar { get; private set; }

      public Func<string, bool> Matcher { get; private set; }

      public ApiArch(string apiPar, Func<string, bool> matcher)
      {
         ApiPar = apiPar;
         Matcher = matcher;
      }
   }
}
