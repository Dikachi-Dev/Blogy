using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Blogy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Blogy.Models.Comments;

namespace Blogy.Data
{
    public class BlogyContext : IdentityDbContext
    {
        public BlogyContext (DbContextOptions<BlogyContext> options)
            : base(options)
        {
        }


        public DbSet<Post> Posts { get; set; }
        public DbSet<MainComment> MainComments { get; set; }
        public DbSet<SubComment> SubComments { get; set; }
    }
    /**
    public class BlogyUserContext : IdentityDbContext<BlogyUser>
    {
        public BlogyUserContext(DbContextOptions<BlogyUserContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
    **/
}
