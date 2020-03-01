using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aves.Shared
{
   public static class PathBuilder
   {
      public static string BuildPath(string name, string path, string relativeParent)
      {
         if (path == null)
            throw new ArgumentException($"{name} is not set");

         //May throw exception
         if (Path.IsPathRooted(path))
            return path;

         //May throw exception
         if (Path.GetFullPath(path) == null)
            throw new ArgumentException($"Weird stuff happens; The path[='{path}'] couldn't be resolved to a full path (was null)");

         return Path.Combine(relativeParent, path);
      }
   }
}
