using CommandLine;
using CoreFrameworkBase.Logging;
using Aves.CMD;
using Aves.Config;
using System;
using System.Collections.Generic;
using CoreFrameworkBase.Crash;
using System.Linq;
using System.Reflection;
using CoreFrameworkBase.Logging.Initalizer;
using CoreFrameworkBase.Logging.Initalizer.Impl;

namespace Aves
{
   /// <summary>
   /// Main entry point
   /// </summary>
   public static class Program
   {
      static void Main(string[] args)
      {
         Run(args);
      }

      public static void Run(string[] args)
      {
#if !DEBUG
         try
         {

            new CrashDetector()
            {
               LoggerInitializer = GetLoggerInitializer(true)
            }.Init();
#endif
         Parser.Default.ParseArguments<CmdOption>(args)
                  .WithParsed((opt) =>
                  {
                     if (opt.ShowVersion)
                     {
                        Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name} {Assembly.GetExecutingAssembly().GetName().Version}");
                        return;
                     }

                     CurrentLoggerInitializer.InitializeWith(GetLoggerInitializer(opt.LogToFile));

                     var starter = new StartUp(opt);
                     starter.Start();
                  })
                  .WithNotParsed((ex) =>
                  {
                     if (ex.All(err =>
                             new ErrorType[]
                             {
                                 ErrorType.HelpRequestedError,
                                 ErrorType.HelpVerbRequestedError,
                                 ErrorType.VersionRequestedError
                             }.Contains(err.Tag))
                       )
                        return;

                     CurrentLoggerInitializer.InitializeWith(GetLoggerInitializer());
                     foreach (var error in ex)
                        Log.Error($"Failed to parse: {error.Tag}");
                  });
#if !DEBUG
         }
         catch (Exception ex)
         {
            CurrentLoggerInitializer.InitializeWith(GetLoggerInitializer(true));
            Log.Fatal(ex);

         }
#endif
      }

      static ILoggerInitializer GetLoggerInitializer(bool writeFile = false) => new DefaultLoggerInitializer() { WriteFile = writeFile };
   }
}
