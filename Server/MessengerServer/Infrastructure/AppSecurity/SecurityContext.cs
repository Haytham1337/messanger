using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.AppSecurity
{
    public class SecurityContext : IdentityDbContext<SecurityUser, IdentityRole<int>, int>
    {
        public SecurityContext(DbContextOptions<SecurityContext> options) : base(options)
        {

        }
    }
}
