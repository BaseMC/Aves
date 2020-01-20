using CoreFrameworkBase.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFrameworkBase.Crash
{
   public class CrashDetector
   {
      public static CrashDetector Current { get; set; } = new CrashDetector();

      private CrashDetector() { }

      public void Init()
      {
         AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      }

      private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs ev)
      {
         try
         {
            LoggerInitializer.Current.InitLogger();

            Log.Fatal("Detected UnhandledException");
            if (ev.ExceptionObject is Exception ex)
               Log.Fatal("Run into unhandled error", ex);
            else
               Log.Fatal($"Run into unhandled error: {ev.ExceptionObject}");
         }
         catch(Exception ex)
         {
            Console.Error.WriteLine(ex);
         }

      }

   }
}
