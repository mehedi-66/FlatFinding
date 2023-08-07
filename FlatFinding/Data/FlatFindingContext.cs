using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlatFinding.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FlatFinding.Data
{
    public class FlatFindingContext : IdentityDbContext<ApplicationUser>
    {
        public FlatFindingContext (DbContextOptions<FlatFindingContext> options)
            : base(options)
        {
        }

        public DbSet<FlatFinding.Models.UserModel> UserModel { get; set; } = default!;
        public DbSet<FlatFinding.Models.Flat> Flats { get; set; } = default!;
        public DbSet<FlatFinding.Models.Notice> Notices { get; set; } = default!;
    }
}
