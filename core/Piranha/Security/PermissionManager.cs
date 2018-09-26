/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System.Collections.Generic;
using System.Linq;

namespace Piranha.Security
{
    /// <summary>
    /// The permission manager.
    /// </summary>
    public class PermissionManager
    {
        private readonly Dictionary<string, IList<PermissionItem>> _modules;

        /// <summary>
        /// Gets the permission items for the given module.
        /// </summary>
        public IList<PermissionItem> this[string module] {
            get {
                if (_modules.TryGetValue(module, out var items))
                    return items;

                _modules[module] = items = new List<PermissionItem>();

                return items;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PermissionManager()
        {
            _modules = new Dictionary<string, IList<PermissionItem>>();
        }

        /// <summary>
        /// Gets the registered permission modules.
        /// </summary>
        /// <returns>The module names</returns>
        public IList<string> GetModules()
        {
            return _modules.Keys.OrderBy(k => k).ToList();
        }

        /// <summary>
        /// Gets the permissions for the given module.
        /// </summary>
        /// <param name="module">The module name</param>
        /// <returns>The available permissions</returns>
        public IList<PermissionItem> GetPermissions(string module)
        {
            return this[module].OrderBy(p => p.Name).ToList();
        }

        /// <summary>
        /// Gets all of the available permissions.
        /// </summary>
        /// <returns>The available permissions</returns>
        public IList<PermissionItem> GetPermissions()
        {
            var all = new Dictionary<string, PermissionItem>();

            foreach (var module in GetModules())
            {
                foreach (var permission in GetPermissions(module))
                {
                    all[permission.Name] = permission;
                }
            }
            return all.Values.OrderBy(k => k.Name).ToList();
        }
    }
}