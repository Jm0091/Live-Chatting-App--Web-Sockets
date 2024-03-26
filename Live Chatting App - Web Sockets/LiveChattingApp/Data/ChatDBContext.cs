using LiveChattingApp.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveChattingApp.Data
{
    public class ChatDBContext : DbContext
    {
        public ChatDBContext(DbContextOptions<ChatDBContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<ConnectivityTime> ConnectivityTime { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }

    }
}
