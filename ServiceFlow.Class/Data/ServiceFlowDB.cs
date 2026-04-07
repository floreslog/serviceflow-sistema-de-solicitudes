using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServiceFlow.Class.Models;

namespace ServiceFlow.Class.Data
{
    public class ServiceFlowDB:IdentityDbContext<ApplicationUser>
    {
        public ServiceFlowDB(DbContextOptions<ServiceFlowDB> options) : base(options) { }

        public DbSet<CategoryModel> Category { get; set; }
        public DbSet<CommentModel> Comment {  get; set; }
        public DbSet<RequestModel> Request { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
          base.OnModelCreating(builder);

          builder.Entity<CommentModel>()
            .HasOne(c => c.Request)
            .WithMany()
            .HasForeignKey(c => c.RequestId)
            .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
