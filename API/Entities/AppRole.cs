using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppRole : IdentityRole<int>
    {
        //many to many relationship. each roles can have many users, each user can have many roles.
        //navigation property to the join table
        public ICollection<AppUserRole> UserRoles { get; set; }

    }
}