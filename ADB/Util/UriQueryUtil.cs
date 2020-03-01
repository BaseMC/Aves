﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace ADB.Util
{
   public static class UriQueryUtil
   {
      public static Uri AddQuery(this Uri uri, string name, string value)
      {
         var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

         httpValueCollection.Remove(name);
         httpValueCollection.Add(name, value);

         var ub = new UriBuilder(uri);

         // this code block is taken from httpValueCollection.ToString() method
         // and modified so it encodes strings with HttpUtility.UrlEncode
         if (httpValueCollection.Count == 0)
         {
            ub.Query = string.Empty;
            return ub.Uri;
         }
         
         var sb = new StringBuilder();

         for (int i = 0; i < httpValueCollection.Count; i++)
         {
            string text = HttpUtility.UrlEncode(httpValueCollection.GetKey(i));

            string val = text != null ? 
               text + "=" : 
               string.Empty;

            string[] vals = httpValueCollection.GetValues(i);

            if (sb.Length > 0)
               sb.Append('&');

            if (vals == null || vals.Length == 0)
               sb.Append(val);
            else if (vals.Length == 1)
            {
               sb.Append(val);
               sb.Append(HttpUtility.UrlEncode(vals[0]));
            }
            else
            {
               for (int j = 0; j < vals.Length; j++)
               {
                  if (j > 0)
                     sb.Append('&');

                  sb.Append(val);
                  sb.Append(HttpUtility.UrlEncode(vals[j]));
               }
            }
            

         }

         ub.Query = sb.ToString();
         
         return ub.Uri;
      }
   }
}
