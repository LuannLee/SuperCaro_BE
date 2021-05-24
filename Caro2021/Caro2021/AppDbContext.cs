using Caro2021.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caro2021
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Match> Matches { get; set; }

        public DbSet<MatchDetail> MatchDetails { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }
}
