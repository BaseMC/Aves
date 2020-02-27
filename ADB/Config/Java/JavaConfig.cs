using System;
using System.Collections.Generic;
using System.Text;

namespace ADB.Config.Java
{
   public class JavaConfig
   {
      /// <summary>
      /// Destination Folder; relative to <see cref="Configuration.DestinationDir"/>
      /// </summary>
      public string Destination { get; set; } = "jre";
   }
}
