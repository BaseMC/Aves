using Aves.Config;
using Aves.PatchFile.ComModel;
using Aves.Shared;
using Aves.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aves.PatchFile
{
   public class PatchFileMaker
   {
      private Configuration Config { get; set; }


      public PatchFileMaker(Configuration conf)
      {
         Config = conf;
      }

      public void Run()
      {
         Log.Info("PatchFileMaker starting");

         TaskRunner.RunTasks(
           Config.VariantConfigs.Where(v => v.Enabled)
              .Select(variant =>
                 new Action(() =>
                    GeneratePatchFile(variant))
                 )
              .ToArray()
           );
         Log.Info("All tasks successfully finished!");

         Log.Info("PatchFileMaker finished");
      }

      public void GeneratePatchFile(VariantConfig variant)
      {
         Log.Info($"Starting generation of patchfile for {variant.Name}");
         var sw = Stopwatch.StartNew();

         var patchFiles = ProcessMapFile(variant);

         if (Config.MakeJavaCompatible)
            patchFiles = MakeJavaCompatible(patchFiles, variant);

         var sb = new StringBuilder();
         foreach (var patchFile in patchFiles)
         {
            sb.Append($"{patchFile.Name} -> {patchFile.ObfName}:");
            sb.AppendLine();

            foreach (var field in patchFile.Fields)
            {
               sb.Append($"    {field.Type} {field.Name} -> {field.ObfName}");
               sb.AppendLine();
            }

            foreach (var method in patchFile.Methods)
            {
               sb.Append($"    {method.ReturnType} {method.Name}({string.Join(',', method.Parameters.Select(x => x.Type))}) -> {method.ObfName}");
               sb.AppendLine();
            }
         }
         File.WriteAllText(variant.PatchFile, sb.ToString());

         sw.Stop();
         Log.Info($"Generation of patchFile[='{variant.PatchFile}'] for {variant.Name} finished; took {sw.ElapsedMilliseconds}ms for {patchFiles.Count}x classes");
      }

      private List<ComPatchClass> ProcessMapFile(VariantConfig variant)
      {
         var classModell = new List<ComPatchClass>();

         ComPatchClass last = null;

         foreach (var line in File.ReadAllLines(variant.MappingFile))
         {
            if (line.StartsWith('#'))
            {
               Log.Info("Comment: " + line);
               continue;
            }

            if (!line.StartsWith("    ")) //Class
            {
               last = GetComPatchClassFromLine(line);
               classModell.Add(last);
            }
            else if (last == null)
               throw new ArgumentException("Didn't find a class for line: " + line);
            else  //Sub
               ProcessPayloadOfLine(line, last);
         }

         return classModell;
      }

      private ComPatchClass GetComPatchClassFromLine(string line)
      {
         var comClass = new ComPatchClass();

         var tmps = line.Split(" -> ");

         comClass.Name = tmps[0];
         //Remove :
         comClass.ObfName = tmps[1].Remove(tmps[1].Length - 1);

         return comClass;
      }

      private void ProcessPayloadOfLine(string line, ComPatchClass last)
      {
         var tmp1 = line.Split(" -> ");
         Contract.Requires(tmp1.Length == 2);

         string obfName = tmp1[1];

         //Method with LineNumbers
         if (tmp1[0].Contains(':'))
         {
            var method = new ComPatchMethod()
            {
               ObfName = obfName
            };

            var tmp2 = tmp1[0].Split(':');
            Contract.Requires(tmp2.Length == 3);

            var tmp3 = tmp2[2].Split(' ');
            Contract.Requires(tmp3.Length == 2);

            method.ReturnType = tmp3[0];


            var (name, parameters) = GetNameAndComPatchParameters(tmp3[1]);
            method.Name = name;
            method.Parameters = parameters;

            last.Methods.Add(method);
         }
         else //Field or Method with no line numbers (rare)
         {
            var tmp2 = tmp1[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Contract.Requires(tmp2.Length == 2);

            //Method with no line numbers
            if (tmp2[1].Contains('(') && tmp2[1].Contains(')'))
            {
               var method = new ComPatchMethod()
               {
                  ObfName = obfName,
                  ReturnType = tmp2[0]
               };

               var (name, parameters) = GetNameAndComPatchParameters(tmp2[1]);
               method.Name = name;
               method.Parameters = parameters;

               last.Methods.Add(method);
            }
            else //Field
            {
               var field = new ComPatchField()
               {
                  ObfName = obfName,
                  Type = tmp2[0],
                  Name = tmp2[1]
               };

               last.Fields.Add(field);
            }
         }
      }

      private (string name, List<ComPatchParameter> parameters) GetNameAndComPatchParameters(string nameAndPars)
      {
         var indexOfParsStart = nameAndPars.IndexOf("(");
         var name = nameAndPars.Substring(0, indexOfParsStart);


         var parStr = nameAndPars.Substring(indexOfParsStart + 1);
         parStr = parStr.Substring(0, parStr.IndexOf(")"));

         List<ComPatchParameter> pars = new List<ComPatchParameter>();
         if (parStr != "")
         {
            pars = parStr.Split(',').Select(par => new ComPatchParameter()
            {
               Type = par,
            }).ToList();

         }

         return (name, pars);
      }

      private List<ComPatchClass> MakeJavaCompatible(List<ComPatchClass> comPatchClasses, VariantConfig variant)
      {
         Log.Info($"['{variant.Name}'] Starting making Java compatible");

         var renamedClassNames = new Dictionary<string, string>();

         foreach (ComPatchClass patchClass in comPatchClasses)
         {
            var (renamed, newValue) = RenameNameOfPatchClass(patchClass.Name);
            if (!renamed)
               continue;

            renamedClassNames.Add(patchClass.Name, newValue);

            patchClass.Name = newValue;
         }

         Log.Info($"['{variant.Name}'] Renamed {renamedClassNames.Count}x classes");

         int renOccureCount = 0;
         foreach (ComPatchClass patchClass in comPatchClasses)
         {
            foreach (var method in patchClass.Methods)
            {
               foreach (var methodParameter in method.Parameters)
               {
                  var (isPatchPar, newValuePar) = ShouldPatchClassName(renamedClassNames, methodParameter.Type);
                  if (isPatchPar)
                  {
                     renOccureCount++;
                     methodParameter.Type = newValuePar;
                  }
               }

               var (isPatchRet, newValueRet) = ShouldPatchClassName(renamedClassNames, method.ReturnType);
               if (isPatchRet)
               {
                  renOccureCount++;
                  method.ReturnType = newValueRet;
               }
            }
         }

         Log.Info($"['{variant.Name}'] Renamed {renOccureCount}x occurrences of the classes");


         Log.Info($"['{variant.Name}'] Finished making Java compatible");
         return comPatchClasses;
      }

      private (bool, string) ShouldPatchClassName(Dictionary<string, string> dictonary, string key)
      {
         var isPatch = dictonary.TryGetValue(key, out string result);
         return (isPatch, result);
      }

      /// <summary>
      /// numeric classnames are invalid in Java, e.g. Abc$1$2
      /// - which becomes class 2 { ... } -
      /// must become Abc$Abc_1$Abc_1_2
      /// </summary>
      /// <param name="name"></param>
      /// <returns></returns>
      private (bool, string) RenameNameOfPatchClass(string name)
      {
         if (!name.Contains('$'))
            return (false, name);

         var srcNameParts = name.Split('$');

         var baseName = srcNameParts[0];

         var newNameParts = new List<string>
         {
           baseName
         };

         var baseNameParts = new List<string>
         {
            baseName.Contains('.') ? baseName.Substring(baseName.LastIndexOf('.') + 1) : baseName
         };

         //Skip the first one
         foreach (string part in srcNameParts.Skip(1))
         {
            var partName = part;

            baseNameParts.Add(part);

            //Check if it is a number
            if (int.TryParse(part, out int _))
            {
               partName = string.Join('_', baseNameParts);
            }

            newNameParts.Add(partName);
         }

         var renamedName = string.Join('$', newNameParts);

         var renamed = renamedName != name;
         if (renamed)
         {
            Log.Verbose($"Renamed '{name}'->'{renamedName}'");
         }

         return (renamed, renamedName);
      }
   }
}
