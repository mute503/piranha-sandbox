﻿/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Web
{
    public class ArchiveRouter
    {
        /// <summary>
        /// Invokes the router.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="url">The requested url</param>
        /// <param name="siteId">The requested site id</param>
        /// <returns>The piranha response, null if no matching page was found</returns>
        public static IRouteResponse Invoke(IApi api, string url, Guid siteId)
        {
            if (!String.IsNullOrWhiteSpace(url) && url.Length > 1)
            {
                var segments = url.Substring(1).Split(new char[] { '/' });

                if (segments.Length >= 1)
                {
                    var blog = api.Pages.GetBySlug(segments[0], siteId);

                    if (blog != null && blog.ContentType == "Blog")
                    {
                        // First check that this is a valid archive URL
                        if (segments.Length == 2)
                        {
                            try
                            {
                                var number = Convert.ToInt32(segments[1]);
                                if (number < 1900 || number > DateTime.Now.Year)
                                    return null;
                            }
                            catch
                            {
                                return null;
                            }
                        }

                        var route = blog.Route ?? "/archive";

                        int? page = null;
                        int? year = null;
                        int? month = null;
                        Guid? categoryId = null;
                        Guid? tagId = null;
                        bool foundCategory = false;
                        bool foundTag = false;
                        bool foundPage = false;

                        for (var n = 1; n < segments.Length; n++)
                        {
                            if (segments[n] == "category" && !foundPage)
                            {
                                foundCategory = true;
                                continue;
                            }

                            if (segments[n] == "tag" && !foundPage && !foundCategory)
                            {
                                foundTag = true;
                                continue;
                            }

                            if (segments[n] == "page")
                            {
                                foundPage = true;
                                continue;
                            }

                            if (foundCategory)
                            {
                                try
                                {
                                    categoryId = api.Categories.GetBySlug(blog.Id, segments[n])?.Id;

                                    if (!categoryId.HasValue)
                                        categoryId = Guid.Empty;
                                }
                                catch
                                {
                                }
                                finally
                                {
                                    foundCategory = false;
                                }
                            }

                            if (foundTag)
                            {
                                try
                                {
                                    tagId = api.Tags.GetBySlug(blog.Id, segments[n])?.Id;

                                    if (!tagId.HasValue)
                                        tagId = Guid.Empty;
                                }
                                catch
                                {
                                }
                                finally
                                {
                                    foundTag = false;
                                }
                            }

                            if (foundPage)
                            {
                                try
                                {
                                    page = Convert.ToInt32(segments[n]);
                                }
                                catch { }
                                break;
                            }

                            if (!year.HasValue)
                            {
                                try
                                {
                                    year = Convert.ToInt32(segments[n]);

                                    if (year.Value > DateTime.Now.Year)
                                        year = DateTime.Now.Year;
                                }
                                catch { }
                            }
                            else
                            {
                                try
                                {
                                    month = Math.Max(Math.Min(Convert.ToInt32(segments[n]), 12), 1);
                                }
                                catch { }
                            }
                        }

                        return new RouteResponse
                        {
                            PageId = blog.Id,
                            Route = route,
                            QueryString = $"id={blog.Id}&year={year}&month={month}&page={page}&pagenum={page}&category={categoryId}&tag={tagId}&piranha_handled=true",
                            IsPublished = blog.Published.HasValue && blog.Published.Value <= DateTime.Now,
                            CacheInfo = new HttpCacheInfo
                            {
                                EntityTag = Utils.GenerateETag(blog.Id.ToString(), blog.LastModified),
                                LastModified = blog.LastModified
                            }
                        };
                    }
                }
            }
            return null;
        }
    }
}
