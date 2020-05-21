using CommandLine;
using Aves.CMD;
using Aves.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreFramework.Logging.Initalizer;
using CoreFramework.CrashLogging;
using CoreFramework.Logging.Initalizer.Impl;

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
         CurrentLoggerInitializer.Set(new DefaultLoggerInitializer(new DefaultLoggerInitializerConfig()));

#if !DEBUG
         try
         {
            new CrashDetector()
            {
               SupplyLoggerInitalizer = () => {
                  InitLog(li => li.Config.WriteFile = true);
                  return CurrentLoggerInitializer.Current;
               }
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

                        InitLog(li => li.Config.WriteFile = opt.LogToFile);

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

                        InitLog();
                        foreach (var error in ex)
                           Log.Error($"Failed to parse: {error.Tag}");
                     });
#if !DEBUG
         }
         catch (Exception ex)
         {
            InitLog(li => li.Config.WriteFile = true);
            Log.Fatal(ex);
         }
#endif
      }

      static void InitLog(Action<DefaultLoggerInitializer> initAction = null)
      {
         CurrentLoggerInitializer.InitLogging(il => initAction?.Invoke((DefaultLoggerInitializer)il));
      }
   }
}
