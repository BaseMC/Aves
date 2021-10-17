using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADB.Config.Github
{
   public class GitHubConfig
   {
      /// <summary>
      /// GITHUB_TOKEN
      /// <para/>
      /// Config possible over:
      ///  - Commandline
      ///  - Environment
      /// </summary>
      /// <seealso cref="https://help.github.com/en/actions/configuring-and-managing-workflows/authenticating-with-the-github_token"/>
      [JsonIgnore]
      public string GitHubToken { get; set; } = null;

      public List<GitHubProjectConfig> ProjectConfigs { get; set; } = new List<GitHubProjectConfig>();
   }
}
