﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using TicketingSys.Models;

namespace TicketingSys.Settings
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<ResponseAttachment> ResponseAttachments { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; }
        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<UserDepartmentAccess> UserDepartmentAccess { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions)
       : base(dbContextOptions)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.userId); // Index on Id column 


            // Department
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.Id);

                // Required name
                entity.Property(d => d.Name).IsRequired();

                // Unique index on name (case-sensitive)
                entity.HasIndex(d => d.Name).IsUnique();
            });
            

            // Response
            modelBuilder.Entity<Response>(entity =>
            {
                entity.HasKey(r => r.Id);

                // Relationship: Response -> Ticket
                entity.HasOne(r => r.Ticket)
                      .WithMany(t => t.Responses) 
                      .HasForeignKey(r => r.TicketId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relationship: Response -> User (sender)
                entity.HasOne(r => r.User)
                      .WithMany() 
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure properties
                entity.Property(r => r.Message).IsRequired();
                
            });


            // ResponseAttachment
            modelBuilder.Entity<ResponseAttachment>(entity =>
            {
                entity.HasKey(ra => ra.Id);

                // Relationship: ResponseAttachment -> Response
                entity.HasOne(ra => ra.Response)
                      .WithMany(r => r.Attachments)
                      .HasForeignKey(ra => ra.ResponseId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Require path to attachment
                entity.Property(ra => ra.Path).IsRequired();

            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(ra => ra.Id);

                // Mark Title as required
                entity.Property(t => t.Title).IsRequired();

                // Mark Title as required
                entity.Property(t => t.Urgency).IsRequired();

                entity.HasOne(t => t.SubmittedBy)
                  .WithMany() 
                  .HasForeignKey(t => t.SubmittedById)
                  .OnDelete(DeleteBehavior.Restrict);

                // Relationship: Ticket -> AssignedTo (user to which ticket is assigned)
                entity.HasOne(t => t.AssignedTo)
                      .WithMany() 
                      .HasForeignKey(t => t.AssignedToId)
                      .OnDelete(DeleteBehavior.SetNull);

                // Relationship: Ticket -> TicketCategory
                entity.HasOne(t => t.Category)
                      .WithMany() 
                      .HasForeignKey(t => t.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);

                // Relationship: Ticket -> Department
                entity.HasOne(t => t.Department)
                      .WithMany() 
                      .HasForeignKey(t => t.DepartmentId)
                      .OnDelete(DeleteBehavior.Cascade);


                // TicketAttachment
                modelBuilder.Entity<TicketAttachment>(entity =>
                {
                    // Map to a specific table
                    entity.ToTable("TicketAttachments");

                    entity.HasKey(ta => ta.Id);

                    // Relationship: TicketAttachment -> Ticket
                    entity.HasOne(ta => ta.Ticket)
                          .WithMany(t => t.Attachments)
                          .HasForeignKey(ta => ta.TicketId)
                          .OnDelete(DeleteBehavior.Cascade);

                    // Required Path property
                    entity.Property(ta => ta.Path).IsRequired();
                });


                modelBuilder.Entity<TicketCategory>(entity =>
                {
                    entity.HasKey(tc => tc.Id);

                    entity.Property(tc => tc.Name).IsRequired();

                    entity.HasIndex(tc => tc.Name).IsUnique();

                    // Relationship: TicketCategory -> Department
                    entity.HasOne(tc => tc.Department)
                          .WithMany()
                          .HasForeignKey(tc => tc.DepartmentId)
                          .OnDelete(DeleteBehavior.Cascade);
                });

                modelBuilder.Entity<UserDepartmentAccess>(entity =>
                {
                    entity.HasKey(uda => uda.Id);

                    entity.HasOne(uda => uda.User)
                          .WithMany(u => u.DepartmentAccesses)
                          .HasForeignKey(uda => uda.UserId)
                          .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(uda => uda.Department)
                          .WithMany()
                          .HasForeignKey(uda => uda.DepartmentId)
                          .OnDelete(DeleteBehavior.Cascade);

                    entity.HasIndex(uda => new  // add index to userid query and userid + deptid query
                        { uda.UserId, uda.DepartmentId }).IsUnique(); // prevent duplicate access
                });



            });
            
        }
    }
}
