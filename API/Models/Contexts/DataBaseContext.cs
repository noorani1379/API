using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.Contexts 
{
    public class DataBaseContext:DbContext
    {
        public DataBaseContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDo>().HasQueryFilter(p => !p.IsRemoved);
        }
    }
}
