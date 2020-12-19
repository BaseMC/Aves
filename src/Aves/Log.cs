using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Aves
{
   internal static class Log
   {
      private static string FormatForException(this string message, Exception ex)
      {
         return $"{message}: {(ex != null ? ex.ToString() : "")}";
      }

      private static string FormatForContext(
         this string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         var fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
         var methodName = memberName;

         return $"{fileName} [{methodName}] {message}";
      }

      public static void Verbose(
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         Serilog.Log.Verbose(
            message
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Verbose(
         string message,
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         Serilog.Log.Verbose(
            message
               .FormatForException(ex)
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Verbose(
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {

         Serilog.Log.Verbose(
            (ex != null ? ex.ToString() : "")
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Debug(
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         Serilog.Log.Debug(
            message
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Debug(
         string message,
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         Serilog.Log.Debug(
            message
               .FormatForException(ex)
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Debug(
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {

         Serilog.Log.Debug(
            (ex != null ? ex.ToString() : "")
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Info(
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         Serilog.Log.Information(
            message
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Info(
         string message,
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {

         Serilog.Log.Information(
            message
               .FormatForException(ex)
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Info(
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {

         Serilog.Log.Information(
            (ex != null ? ex.ToString() : "")
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Warn(string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {

         Serilog.Log.Warning(
            message
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Warn(
         string message,
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {

         Serilog.Log.Warning(
            message
               .FormatForException(ex)
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Warn(
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {

         Serilog.Log.Warning(
            (ex != null ? ex.ToString() : "")
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Error(
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {

         Serilog.Log.Error(
            message
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
           );
      }

      public static void Error(
         string message,
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {

         Serilog.Log.Error(
            message
               .FormatForException(ex)
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Error(
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {

         Serilog.Log.Error(
            (ex != null ? ex.ToString() : "")
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Fatal(
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         FatalAction();

         Serilog.Log.Error(
            message
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Fatal(
         string message,
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         FatalAction();

         Serilog.Log.Error(
            message
               .FormatForException(ex)
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      public static void Fatal(
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         FatalAction();

         Serilog.Log.Error(
            (ex != null ? ex.ToString() : "")
               .FormatForContext(memberName, sourceFilePath, sourceLineNumber)
            );
      }

      private static void FatalAction()
      {
         Environment.ExitCode = -1;
      }
   }
}
