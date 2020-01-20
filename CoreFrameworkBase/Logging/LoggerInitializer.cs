using Serilog;
using CoreFrameworkBase.Logging.LoggerExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Serilog.Events;

namespace CoreFrameworkBase.Logging
{
   public class LoggerInitializer
   {
      public static LoggerInitializer Current { get; set; } = new LoggerInitializer();

      private static bool LoggerSet { get; set; } = false;

      private string RelativeLogFileDirectory { get; set; } = "logs";
      private string FileDateTimeFormat { get; set; } = "yyyy-MM-dd-HH-mm-ss";
      private string LogFileExtension { get; set; } = ".log";
      public string LogfilePath { get; private set; }

      public void InitLogger(bool writeFile = false)
      {
         if (LoggerSet)
         {
            Log.Debug("Logger was already initialized");
            return;
         }

         LoggerSet = true;

         var outputTemplateCons = "{Timestamp:HH:mm:ss,fff} {Level:u3} {ThreadId,-2} {Message:lj}{NewLine}{Exception}";
         

         var loggerconf = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.With<Log4NetLevelMapperEnricher>()
            .Enrich.WithThreadId()
            .WriteTo.Console(
               outputTemplate: outputTemplateCons);

         if(writeFile)
         {
            var outputTemplateFile = "{Timestamp:HH:mm:ss,fff} {Log4NetLevel} {ThreadId,-2} {Message:lj}{NewLine}{Exception}";

            LogfilePath = $"{RelativeLogFileDirectory}{Path.DirectorySeparatorChar}{DateTime.Now.ToString(FileDateTimeFormat)}{LogFileExtension}";

            loggerconf.WriteTo.File(
               LogfilePath,
               outputTemplate: outputTemplateFile);
         }

         Serilog.Log.Logger = loggerconf.CreateLogger();

         Log.Info($"****** {Assembly.GetEntryAssembly().GetName().Name} {Assembly.GetEntryAssembly().GetName().Version} ******");
         Log.Info($"****** {Assembly.GetExecutingAssembly().GetName().Name} {Assembly.GetExecutingAssembly().GetName().Version} ******");

         if (writeFile)
            Log.Info($"Logging to file: '{LogfilePath}'");

         AppDomain.CurrentDomain.ProcessExit += (s, ev) =>
         {
            Log.Info("Shutting down logger; Flushing...");
            Serilog.Log.CloseAndFlush();
         };
      }
     
   }
}
