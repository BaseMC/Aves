using System;
using System.Diagnostics;

namespace Aves.Util
{
   public static class CommandLineSupport
   {
      public static string Bash(this string cmd)
      {
         var escapedArgs = cmd.Replace("\"", "\\\"");

         using var process = new Process()
         {
            StartInfo = new ProcessStartInfo
            {
               FileName = "/bin/bash",
               Arguments = $"-c \"{escapedArgs}\"",
               RedirectStandardOutput = true,
               UseShellExecute = false,
               CreateNoWindow = true,
            }
         };

         process.Start();

         string result = process.StandardOutput.ReadToEnd();

         process.WaitForExit();

         return result;
      }

      public static string CMD(this string cmd)
      {
         var escapedArgs = cmd.Replace("\"", "\\\"");

         using var process = new Process()
         {
            StartInfo = new ProcessStartInfo
            {
               FileName = @"C:\Windows\System32\cmd.exe",
               Arguments = $"/C \"{escapedArgs}\"",
               RedirectStandardOutput = true,
               UseShellExecute = false,
               CreateNoWindow = true,
            }
         };

         process.Start();

         string result = process.StandardOutput.ReadToEnd();

         process.WaitForExit();

         return result;
      }
   }
}
