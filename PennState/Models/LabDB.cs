using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    public class LabDB : DbContext
    {
        public LabDB() : base("LabDB")
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<LabDBContext, PennState.Migrations.Configuration>());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Photos> Photos { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SubLocation> SubLocations { get; set; }
        public DbSet<CatagoryLocation> CategoryLocations { get; set; }
        public DbSet<CatagoryVendor> CategoryVendors { get; set; }
        public DbSet<CatagoryType> CategoryTypes { get; set; }
        public DbSet<CatagoryOwner> CategoryOwners { get; set; }

    }
}