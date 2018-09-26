/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Models
{
    public interface IDynamicModel
    {
        /// <summary>
        /// Gets/sets the regions.
        /// </summary>
        dynamic Regions { get; set; }
    }
}
