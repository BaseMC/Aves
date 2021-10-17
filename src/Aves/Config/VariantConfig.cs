using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Aves.Config
{
   /// <summary>
   /// Configuration for variants (also known as profiles; e.g: client and server)
   /// </summary>
   public class VariantConfig
   {
      /// <summary>
      /// If false, variant won't get processed
      /// </summary>
      public bool Enabled { get; set; } = true;

      /// <summary>
      /// Name; used for filling the other attributes
      /// </summary>
#pragma warning disable S2376 // Write-only properties should not be used
      public string SetName
      {
#pragma warning restore S2376 // Write-only properties should not be used
         set
         {
            if (string.IsNullOrWhiteSpace(value))
               throw new ArgumentException("Invalid Name!");

            var name = value.Trim().ToLower();

            Name = name;
            Key = name;
            OutputFilesDirFolder = name;

            SrcJar = name + ".jar";

            MappingKey = name + "_mappings";
            MappingFile = name + ".txt";
            PatchFile = name + ".txt";

            DeObfuscatedFile = name + ".jar";
            DecompiledFile = DeObfuscatedFile;
         }
      }

      /// <summary>
      /// General name, e.g. client or server
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// Key in <see cref="Configuration.ManifestJsonFilePath"/>
      /// </summary>
      public string Key { get; set; }

      /// <summary>
      /// Name of the .jar file
      /// </summary>
      public string SrcJar { get; set; }

      /// <summary>
      /// Mapping Key in <see cref="Configuration.ManifestJsonFilePath"/>
      /// </summary>
      public string MappingKey { get; set; }

      /// <summary>
      /// Mapping File
      /// </summary>
      public string MappingFile { get; set; }

      /// <summary>
      /// Patch File (processed <see cref="MappingFile"/>)
      /// </summary>
      public string PatchFile { get; set; }

      public List<string> ExcludedComponents { get; set; } = new List<string>() { 
         // Workaround (since 1.15-dev): there seem to be test classes inside the jar, which aren't correctly compiled
         "net/minecraft/gametest/"
      };

      /// <summary>
      /// DeObfuscatedFile
      /// </summary>
      public string DeObfuscatedFile { get; set; }

      /// <summary>
      /// DecompiledFile
      /// </summary>
      /// <remarks>
      /// Fernflower only takes the output directory
      /// this file get's predicted by <see cref="DeObfuscatedFile"/>
      /// </remarks>
      public string DecompiledFile { get; set; }

      /// <summary>
      /// OutputFiles Directory Sub Folder
      /// </summary>
      public string OutputFilesDirFolder { get; set; }

      /// <summary>
      /// Client-Variant
      /// </summary>
      public static readonly VariantConfig CLIENT = new VariantConfig()
      {
         SetName = "client"
      };

      /// <summary>
      /// Server-Variant
      /// </summary>
      /// <remarks>
      /// Enabled = false
      /// </remarks>
      public static readonly VariantConfig SERVER = GetServerVariant();
      protected static VariantConfig GetServerVariant()
      {
         var server = new VariantConfig()
         {
            SetName = "server",
            Enabled = false,
         };

         //Exclude compiled third party libs
         server.ExcludedComponents.AddRange(new string[]
            {
               //guava + gson
               "com/google/",
               //netty
               "io/netty",
               //fastutil
               "it/unimi/",
               //jsr305
               "javax/",
               //joptsimple
               "joptsimple/",
               //commons + log4j
               "org/apache",
            });

         return server;
      }

      /// <summary>
      /// Default Variants, that are processed
      /// </summary>
      /// <remarks>
      /// Server is included but not active
      /// </remarks>
      public static readonly ImmutableHashSet<VariantConfig> DEFAULT_VARIANTS = ImmutableHashSet.Create(CLIENT, SERVER);

      public override bool Equals(object obj)
      {
         return obj is VariantConfig config &&
                Name == config.Name;
      }

      public override int GetHashCode()
      {
         return HashCode.Combine(Name);
      }
   }
}
