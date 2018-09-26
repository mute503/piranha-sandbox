﻿/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Piranha
{
    /// <summary>
    /// Utility methods.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Gets a subset of the given array as a new array.
        /// </summary>
        /// <typeparam name="T">The array type</typeparam>
        /// <param name="arr">The array</param>
        /// <param name="startpos">The startpos</param>
        /// <param name="length">The length</param>
        /// <returns>The new array</returns>
        public static T[] Subset<T>(this T[] arr, int startpos = 0, int length = 0)
        {
            List<T> tmp = new List<T>();

            length = length > 0 ? length : arr.Length - startpos;

            for (var i = 0; i < arr.Length; i++)
            {
                if (i >= startpos && i < (startpos + length))
                    tmp.Add(arr[i]);
            }
            return tmp.ToArray();
        }

        /// <summary>
        /// Generates a slug from the given string.
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>The slug</returns>
        public static string GenerateSlug(string str, bool hierarchical = true)
        {
            if (App.Hooks != null && App.Hooks.OnGenerateSlug != null)
            {
                // Call the registered slug generation
                return App.Hooks.OnGenerateSlug(str);
            }
            else
            {
                // Trim & make lower case
                var slug = str.Trim().ToLower();

                // Convert culture specific characters
                slug = slug
                    .Replace("å", "a")
                    .Replace("ä", "a")
                    .Replace("á", "a")
                    .Replace("à", "a")
                    .Replace("ö", "o")
                    .Replace("ó", "o")
                    .Replace("ò", "o")
                    .Replace("é", "e")
                    .Replace("è", "e")
                    .Replace("í", "i")
                    .Replace("ì", "i");

                // Remove special characters
                slug = Regex.Replace(slug, @"[^a-z0-9-/ ]", "").Replace("--", "-");

                // Remove whitespaces
                slug = Regex.Replace(slug.Replace("-", " "), @"\s+", " ").Replace(" ", "-");

                // Remove slash if non-hierarchical
                if (!hierarchical)
                    slug = slug.Replace("/", "-");

                // Remove multiple dashes
                slug = Regex.Replace(slug, @"[-]+", "-");

                // Remove leading & trailing dashes
                if (slug.EndsWith("-"))
                    slug = slug.Substring(0, slug.LastIndexOf("-"));
                if (slug.StartsWith("-"))
                    slug = slug.Substring(Math.Min(slug.IndexOf("-") + 1, slug.Length));
                return slug;
            }
        }

        /// <summary>
        /// Generates a camel cased internal id from the given string.
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>The internal id</returns>
        public static string GenerateInteralId(string str)
        {
            var ti = new CultureInfo("en-US", false).TextInfo;
            return ti.ToTitleCase(GenerateSlug(str).Replace('-', ' ')).Replace(" ", "");
        }


        /// <summary>
        /// Generates a ETag from the given name and date.
        /// </summary>
        /// <param name="name">The resource name</param>
        /// <param name="date">The modification date</param>
        /// <returns>The etag</returns>
        public static string GenerateETag(string name, DateTime date)
        {
            var encoding = new UTF8Encoding();

            using (var crypto = MD5.Create())
            {
                var str = name + date.ToString("yyyy-MM-dd HH:mm:ss");
                var bytes = crypto.ComputeHash(encoding.GetBytes(str));
                return $"\"{Convert.ToBase64String(bytes)}\"";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatByteSize(double bytes)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB" };

            int index = 0;
            if (bytes > 1023)
            {
                do
                {
                    bytes /= 1024;
                    index++;
                } while (bytes >= 1024 && index < 3);
            }

            return $"{bytes:0.00} {SizeSuffixes[index]}";
        }

        /// <summary>
        /// Gets the first paragraph from the given html string.
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>The first paragraph</returns>
        public static string FirstParagraph(string str)
        {
            Regex reg = new Regex("<p[^>]*>.*?</p>");
            var matches = reg.Matches(str);

            return matches.Count > 0 ? matches[0].Value : "";
        }

        /// <summary>
        /// Gets the first paragraph from the given markdown field.
        /// </summary>
        /// <param name="md">The field</param>
        /// <returns>The first paragraph</returns>
        public static string FirstParagraph(Extend.Fields.MarkdownField md)
        {
            Regex reg = new Regex("<p[^>]*>.*?</p>");
            var matches = reg.Matches(md.ToHtml());

            return matches.Count > 0 ? matches[0].Value : "";
        }

        /// <summary>
        /// Gets the first paragraph from the given html field.
        /// </summary>
        /// <param name="html">The field</param>
        /// <returns>The first paragraph</returns>
        public static string FirstParagraph(Extend.Fields.HtmlField html)
        {
            Regex reg = new Regex("<p[^>]*>.*?</p>");
            var matches = reg.Matches(html.Value);

            return matches.Count > 0 ? matches[0].Value : "";
        }

        /// <summary>
        /// Gets the formatted three digit version number of the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>The version string</returns>
        public static string GetAssemblyVersion(Assembly assembly)
        {
            var version = assembly.GetName().Version;

            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}
