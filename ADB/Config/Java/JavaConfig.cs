using System;
using System.Collections.Generic;
using System.Text;

namespace ADB.Config.Java
{
   /// <seealso cref="https://api.adoptopenjdk.net/swagger-ui/#/Assets/get_v3_assets_feature_releases__feature_version___release_type_"/>
   public class JavaConfig
   {
      /// <summary>
      /// Destination Folder; relative to <see cref="Configuration.DestinationDir"/>
      /// </summary>
      public string DestinationSubDir { get; set; } = "jre";

      /// <summary>
      /// Remote URL
      /// </summary>
      public string RemoteBaseURL { get; set; } = "https://api.adoptopenjdk.net/v3/assets/feature_releases/{feature_version}/{release_type}";

      /// <summary>
      /// Version
      /// </summary>
      /// <remarks>
      /// feature_version
      /// </remarks>
      /// <example>
      /// 8, ... 11, 12, ...
      /// </example>
      public int FeatureVersion { get; set; } = 11;

      /// <summary>
      /// ReleaseType
      /// </summary>
      /// <remarks>
      /// release_type 
      /// </remarks>
      /// <example>
      /// ea, ga
      /// </example>
      public string ReleaseType { get; set; } = "ga";

      /// <summary>
      /// Architecture; if null: automatically detected from <see cref="Configuration.RID"/>
      /// </summary>
      /// <remarks>
      /// architecture
      /// </remarks>
      /// <example>
      /// x64, arm, x86
      /// </example>
      public string Architecture { get; set; } = null;

      //"heap_size" is not required and skipped

      /// <summary>
      /// ImageType
      /// </summary>
      /// <remarks>
      /// image_type
      /// </remarks>
      /// <example>
      /// jdk, jre, testimage
      /// </example>
      public string ImageType { get; set; } = "jre";

      /// <summary>
      /// JVM Implementation
      /// </summary>
      /// <remarks>
      /// jvm_impl
      /// </remarks>
      /// <example>
      /// hotspot, openj9
      /// </example>
      public string JVMImpl { get; set; } = "hotspot";

      /// <summary>
      /// Operating System; if null: automatically detected from <see cref="Configuration.RID"/>
      /// </summary>
      /// <remarks>
      /// os
      /// </remarks>
      /// <example>
      /// windows, mac, linux, ...
      /// </example>
      public string OS { get; set; } = null;

      /// <summary>
      /// Pagination page size (number of results)
      /// </summary>
      /// <remarks>
      /// page_size
      /// </remarks>
      public int PageSize { get; set; } = 1;

      /// <summary>
      /// Project
      /// </summary>
      /// <remarks>
      /// project
      /// </remarks>
      /// <example>
      /// jdk, valhalla, ...
      /// </example>
      public string Project { get; set; } = "jdk";

      /// <summary>
      /// Sort Order; Use latest with DESC
      /// </summary>
      /// <remarks>
      /// sort_order
      /// </remarks>
      /// <example>
      /// ASC, DESC
      /// </example>
      public string SortOrder { get; set; } = "DESC";

      /// <summary>
      /// Vendor
      /// </summary>
      /// <remarks>
      /// vendor
      /// </remarks>
      /// <example>
      /// jdk, valhalla, ...
      /// </example>
      public string Vendor { get; set; } = "adoptopenjdk";
   }
}
