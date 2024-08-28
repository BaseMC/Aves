using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADB.Config.Github
{
   public class GitHubProjectConfig
   {
      /// <summary>
      /// Owner
      /// </summary>
      /// <example>
      /// BaseMC
      /// </example>
      public string Owner { get; set; }

      /// <summary>
      /// Repository
      /// </summary>
      /// <example>
      /// Aves
      /// </example>
      public string Repo { get; set; }

      /// <summary>
      /// Regex used for matching the asset name
      /// </summary>
      /// <example>
      /// vineflower-[0-9\.]*\.jar
      /// </example>
      public string AssetNameMatcher { get; set; }

      /// <summary>
      /// Used for renaming the asset
      /// </summary>
      /// <example>
      /// vineflower.jar
      /// </example>
      public string AssetNameReplacePattern { get; set; }
   }
}
