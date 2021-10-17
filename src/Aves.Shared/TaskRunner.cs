using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Aves.Shared
{
   public static class TaskRunner
   {
      public static void RunTasks(params Action[] actions)
      {
         RunTasks(actions.Select(act => Task.Run(act)).ToArray());
      }

      public static void RunTasks(params Task[] tasks)
      {
         var tasklist = new List<Task>();

         foreach (var task in tasks)
            tasklist.Add(task);

         var mastertask = Task.WhenAll(tasklist);
         mastertask.Wait();

         if (!mastertask.IsCompletedSuccessfully)
            throw new InvalidOperationException("One or more tasks failed");
      }
   }
}
