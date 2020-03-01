using ADB.CMD;
using CommandLine;
using CoreFrameworkBase.Crash;
using CoreFrameworkBase.Logging;
using System;
using System.Reflection;
using System.Linq;

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

            CrashDetector.Current.Init();
#endif
            Parser.Default.ParseArguments<CmdOption>(args)
                     .WithParsed((opt) =>
                     {
                        if(opt.ShowVersion)
                        {
                           Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name} {Assembly.GetExecutingAssembly().GetName().Version}");
                           return;
                        }

                        LoggerInitializer.Current.InitLogger(opt.LogToFile);

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

                        LoggerInitializer.Current.InitLogger();
                        foreach (var error in ex)
                           Log.Error($"Failed to parse: {error.Tag}");
                     });
#if !DEBUG
         }
         catch (Exception ex)
         {

            LoggerInitializer.Current.InitLogger(true);
            Log.Fatal(ex);

         }
#endif
      }
   }
}
