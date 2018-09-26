/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.AspNetCore.Identity.Models
{
    public class RoleListModel
    {
        public class ListItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int UserCount { get; set; }
        }

        public IList<ListItem> Roles { get; set; }

        public RoleListModel()
        {
            Roles = new List<ListItem>();
        }

        public static RoleListModel Get(IDb db)
        {
            var model = new RoleListModel();

            model.Roles = db.Roles
                .OrderBy(r => r.Name)
                .Select(r => new ListItem
                {
                    Id = r.Id,
                    Name = r.Name
                }).ToList();

            foreach (var role in model.Roles)
            {
                role.UserCount = db.UserRoles
                    .Where(r => r.RoleId == role.Id)
                    .Count();
            }

            return model;
        }
    }
}