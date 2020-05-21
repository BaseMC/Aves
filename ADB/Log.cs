using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ADB
{
   /// <summary>
   /// Adapter for CoreFramework
   /// </summary>
   internal class Log
   {
      protected Log()
      {
         //No instances pls
      }

      public static void Verbose(
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Verbose(message, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Verbose(
         string message,
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Verbose(message, ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Verbose(
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Debug(ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Debug(
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Debug(message, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Debug(
         string message, Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Debug(message, ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Debug(
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Debug(ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Info(
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Info(message, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Info(
         string message,
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Info(message, ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Info(
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Info(ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Warn(
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Warn(message, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Warn(
         string message,
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Warn(message, ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Warn(
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Warn(ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Error(
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Error(message, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Error(
         string message,
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Error(message, ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Error(
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Error(ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Fatal(
         string message,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Error(message, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Fatal(
         string message,
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Error(message, ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Fatal(
         Exception ex,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFramework.Log.Error(ex, memberName, sourceFilePath, sourceLineNumber);
      }
   }
}
