/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Data
{
    public sealed class SiteField : ContentField, IModel
    {
        /// <summary>
        /// Gets/sets the site id.
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Gets/sets the site.
        /// </summary>
        public Site Site { get; set; }
    }
}
