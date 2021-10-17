using System;
using System.Collections.Generic;
using System.Text;

namespace Aves.PatchFile.ComModel
{
   public class ComPatchMethod
   {
      public string Name { get; set; }
      public string ObfName { get; set; }


      public string ReturnType { get; set; }

      public List<ComPatchParameter> Parameters { get; set; } = new List<ComPatchParameter>();

   }
}
