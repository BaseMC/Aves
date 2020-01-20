using System;
using System.Collections.Generic;
using System.Text;

namespace Aves.PatchFile.ComModel
{
   public class ComPatchClass
   {
      public string Name { get; set; }
      public string ObfName { get; set; }

      public List<ComPatchField> Fields { get; set; } = new List<ComPatchField>();
      public List<ComPatchMethod> Methods { get; set; } = new List<ComPatchMethod>();

   }
}
