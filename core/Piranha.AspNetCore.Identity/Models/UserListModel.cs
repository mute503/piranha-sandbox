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
    public class UserListModel
    {
        public class ListItem
        {
            public Guid Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public IList<string> Roles { get; set; }

            public ListItem()
            {
                Roles = new List<string>();
            }
        }

        public IList<ListItem> Users { get; set; }

        public UserListModel()
        {
            Users = new List<ListItem>();
        }

        public static UserListModel Get(IDb db)
        {
            var model = new UserListModel();

            model.Users = db.Users
                .OrderBy(u => u.UserName)
                .Select(u => new ListItem
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email
                }).ToList();

            var roles = db.Roles
                .ToList();

            foreach (var user in model.Users)
            {
                var userRoles = db.UserRoles
                    .Where(r => r.UserId == user.Id)
                    .ToList();

                foreach (var userRole in userRoles)
                {
                    var role = roles.FirstOrDefault(r => r.Id == userRole.RoleId);
                    if (role != null)
                        user.Roles.Add(role.Name);
                }
            }
            return model;
        }
    }
}