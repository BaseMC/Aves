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
   }
}
