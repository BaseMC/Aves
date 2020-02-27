using ADB.CMD;
using CommandLine;
using CoreFrameworkBase.Crash;
using CoreFrameworkBase.Logging;
using System;

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
                        LoggerInitializer.Current.InitLogger(opt.LogToFile);

                        var starter = new StartUp(opt);
                        starter.Start();
                     })
                     .WithNotParsed((ex) =>
                     {
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
