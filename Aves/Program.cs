using CommandLine;
using CoreFrameworkBase.Logging;
using Aves.CMD;
using Aves.Config;
using System;
using System.Collections.Generic;
using CoreFrameworkBase.Crash;

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
