using System;
using System.Collections.Generic;
using System.Text;

namespace ADB.Java
{
   /// <seealso cref="https://docs.microsoft.com/de-de/dotnet/core/rid-catalog"/>
   public class ApiArch
   {
      public static readonly ApiArch X64 = new ApiArch("x64", s => "x64".Equals(s));
      public static readonly ApiArch X86 = new ApiArch("x86", s => "x86".Equals(s));
      public static readonly ApiArch ARM = new ApiArch("arm", s => "arm".Equals(s) || "arm64".Equals(s));

      public string ApiPar { get; private set; }

      public Func<string, bool> Matcher { get; private set; }

      public ApiArch(string apiPar, Func<string, bool> matcher)
      {
         ApiPar = apiPar;
         Matcher = matcher;
      }
   }
}
