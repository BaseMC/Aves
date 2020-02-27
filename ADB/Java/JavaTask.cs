using ADB.Config;
using Aves.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADB.Java
{
   public class JavaTask
   {
      private Configuration Config { get; set; }

      public JavaTask(Configuration config)
      {
         Config = config;
      }

      public void Run()
      {
         Config.JavaConfig.Destination = PathBuilder.BuildPath(nameof(Config.JavaConfig.Destination), Config.JavaConfig.Destination, Config.DestinationDir);
         Log.Info($"Set {nameof(Config.JavaConfig.Destination)}='{Config.JavaConfig.Destination}'");

         DirUtil.EnsureCreated(Config.JavaConfig.Destination);
      }
   }
}
