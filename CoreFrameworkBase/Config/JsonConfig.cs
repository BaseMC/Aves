using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFrameworkBase.Config
{
   /// <summary>
   /// Extend this class to create a custom Config
   /// </summary>
   public abstract class JsonConfig
   {
      /// <summary>
      /// Configuration
      /// </summary>
      [JsonIgnore]
      public virtual Configurator Config { get; set; } = new Configurator();

      protected JsonConfig()
      { }

      /// <summary>
      /// Loads from file
      /// </summary>
      /// <param name="autosaveifnotexists"><see cref="Save(bool)"/></param>
      [Obsolete("Use Load(LoadModeIfFileNotFound)")]
      public void Load(bool autosaveifnotexists = true)
      {
         Load(LoadFileNotFoundAction.GENERATE_FILE);
      }

      /// <summary>
      /// Loads from file
      /// </summary>
      /// <param name="fileNotFoundAction">Determine what to do if the file doesn't exists</param>
      public void Load(LoadFileNotFoundAction fileNotFoundAction = LoadFileNotFoundAction.THROW_EX)
      {
         Contract.Assert(Config != null);
         Contract.Assert(Config.SavePath != null);

         if (File.Exists(Config.SavePath))
            JsonConvert.PopulateObject(File.ReadAllText(Config.SavePath), this, Config.Settings);
         else
         {
            if (fileNotFoundAction == LoadFileNotFoundAction.THROW_EX)
               throw new FileNotFoundException($"Could not find file '{Config.SavePath}'");
            else if (fileNotFoundAction == LoadFileNotFoundAction.GENERATE_FILE)
               Save();
         }
      }

      public enum LoadFileNotFoundAction
      {
         NONE,
         THROW_EX,
         GENERATE_FILE
      }

      /// <summary>
      /// Saves to file
      /// </summary>
      /// <param name="createifnotexists">creates the file if it doesn't exists</param>
      public void Save(bool createifnotexists = true)
      {
         Contract.Assert(Config != null);
         Contract.Assert(Config.SavePath != null);

         if (createifnotexists)
         {
            string dir = Path.GetDirectoryName(Config.SavePath);
            if (!string.IsNullOrWhiteSpace(dir))
               Directory.CreateDirectory(dir);
         }
         File.WriteAllText(Config.SavePath, GetSerialized());
      }

      public string GetSerialized()
      {
         return JsonConvert.SerializeObject(this, Formatting.Indented, Config.Settings);
      }

      /// <summary>
      /// Outsourced class for Configuration
      /// </summary>
      public class Configurator
      {
         public const string DEFAULT_SAVEPATH = "config.json";

         public Configurator()
         {

         }

         public Configurator(string savePath)
         {
            Contract.Requires(savePath != null);
            SavePath = savePath;
         }


         /// <summary>
         /// The Path where the file is saved; by default <see cref="DEFAULT_SAVEPATH"/> 
         /// </summary>
         /// <remarks>You shouldn't change it at runtime!</remarks>
         public string SavePath { get; set; } = DEFAULT_SAVEPATH;

         /// <summary>
         /// Settings that are used, null = Default
         /// </summary>
         public JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings()
         {
            ObjectCreationHandling = ObjectCreationHandling.Replace
         };
      }

   }

}
