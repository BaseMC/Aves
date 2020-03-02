using ADB.Config.Github;
using ADB.Config.Java;
using CoreFrameworkBase.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ADB.Config
{
   public class Configuration : JsonConfig
   {
      /// <summary>
      /// Base-Directory where the files should be put; 
      /// <para/>
      /// if null: built automatically from <see cref="BuildConfiguration"/> and <see cref="RID"/>
      /// </summary>
      public string DestinationDir { get; set; } = null;

      /// <summary>
      /// With this pattern (if not null) the <see cref="DestinationDir"/> is built
      /// </summary>
      /// <remarks>
      /// / are replaced with <see cref="Path.DirectorySeparatorChar"/>
      /// </remarks>
      /// <example>
      /// Aves/bin/{BuildConfiguration}/netcoreapp3.1/{RID}/publish
      /// </example>
      public string DestinationDirPattern { get; set; }

      /// <summary>
      /// Build Configuration;
      /// only used to set the path of <see cref="DestinationDir"/>
      /// </summary>
      /// <example>
      /// Release or Debug
      /// </example>
      public string BuildConfiguration { get; set; }

      /// <summary>
      /// REQUIRED! Runtime Identifier
      /// </summary>
      /// <seealso cref="https://docs.microsoft.com/de-de/dotnet/core/rid-catalog"/>
      public string RID { get; set; }


      public GitHubConfig GitHubConfig { get; set; } = new GitHubConfig();

      public JavaConfig JavaConfig { get; set; } = new JavaConfig();
   }
}
