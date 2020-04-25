using ADB.CMD;
using CommandLine;
using CoreFrameworkBase.Crash;
using CoreFrameworkBase.Logging;
using System;
using System.Reflection;
using System.Linq;
using CoreFrameworkBase.Logging.Initalizer;
using CoreFrameworkBase.Logging.Initalizer.Impl;

/// <summary>
/// Aves Dependency Builder
/// </summary>
namespace ADB
{
   public static class Program
   {
      public static void Main(string[] args)
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
                        if(opt.ShowVersion)
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
