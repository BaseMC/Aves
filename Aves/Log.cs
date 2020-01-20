using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Aves
{
   /// <summary>
   /// Adapter for CoreFrameworkbase
   /// </summary>
   /// <lastUpdatedAt>2019-08-29</lastUpdatedAt>
   internal static class Log
   {
      public static void Verbose(string message, [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Verbose(message, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Verbose(string message, Exception ex, [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Verbose(message, ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Debug(string message, [CallerMemberName] string memberName = "",
         [CallerFilePath] string sourceFilePath = "",
         [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Debug(message, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Debug(string message, Exception ex, [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Debug(message, ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Info(string message, [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Info(message, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Info(string message, Exception ex, [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Info(message, ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Warn(string message, [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Warn(message, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Warn(string message, Exception ex, [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Warn(message, ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Error(string message, [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Error(message, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Error(string message, Exception ex, [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Error(message, ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Error(Exception ex, [CallerMemberName]
      string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Error(ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Fatal(string message, [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Error(message, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Fatal(string message, Exception ex, [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Error(message, ex, memberName, sourceFilePath, sourceLineNumber);
      }

      public static void Fatal(Exception ex, [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
      {
         CoreFrameworkBase.Log.Error(ex, memberName, sourceFilePath, sourceLineNumber);
      }
   }
}
