using System;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence {
    public class DataContext : IdentityDbContext<AppUser> {
        public DataContext(DbContextOptions options) : base(options) {

        }

        public DbSet<Value> Values { get; set; }
        public DbSet<Activity> Activities { get; set; }

        //Many to many Table
        public DbSet<UserActivity> UserActivities { get; set; }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<UserFollowing> Followings { get; set; }
        protected override void OnModelCreating(ModelBuilder builder) {
            //gives AppUser a primary key of a string - will error without it
            base.OnModelCreating(builder);
            builder.Entity<Value>()
                .HasData(
                    new Value { Id = 1, Name = "Value 101" },
                    new Value { Id = 2, Name = "Value 102" },
                    new Value { Id = 3, Name = "Value 103" }
                );

            //configure to use id from user and activity to make the relationship
            builder.Entity<UserActivity>(x => x.HasKey(ua => new { ua.AppUserId, ua.ActivityId }));

            //Define the relationship
            //User has many activities
            builder.Entity<UserActivity>()
                .HasOne(u => u.AppUser)
                .WithMany(a => a.UserActivities)
                .HasForeignKey(u => u.AppUserId);

            //activity has many users
            builder.Entity<UserActivity>()
                .HasOne(a => a.Activity)
                .WithMany(u => u.UserActivities)
                .HasForeignKey(a => a.ActivityId);

            //Many to Many
            builder.Entity<UserFollowing>(b => {
                //Key is the combo of obvId and TargetId
                b.HasKey(k => new { k.ObserverId, k.TargetId });

                // User is following many people
                b.HasOne(o => o.Observer)
                    .WithMany(f => f.Followings)
                    .HasForeignKey(o => o.ObserverId)
                    .OnDelete(DeleteBehavior.Restrict);

                // User has many followers
                b.HasOne(o => o.Target)
                    .WithMany(f => f.Followers)
                    .HasForeignKey(o => o.TargetId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}