using System;
using System.Collections.Generic;
using System.Text;

namespace ADB.Java
{
   /// <seealso cref="https://docs.microsoft.com/de-de/dotnet/core/rid-catalog"/>
   public class ApiOS
   {
      public static readonly ApiOS WIN = new ApiOS("windows", s => s.StartsWith("win"));
      public static readonly ApiOS LINUX = new ApiOS("linux", s => s.StartsWith("linux") || s.StartsWith("rhel") || s.StartsWith("tizen"));
      public static readonly ApiOS MAC = new ApiOS("mac", s => s.StartsWith("osx"));

      public string ApiPar { get; private set; }

      public Func<string,bool> Matcher { get; private set; }

      public ApiOS(string apiPar, Func<string, bool> matcher)
      {
         ApiPar = apiPar;
         Matcher = matcher;
      }
   }
}
