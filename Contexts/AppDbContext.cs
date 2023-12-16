using EduHome.Models;
using EduHome.Models.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EduHome.Models.Identity;

namespace EduHome.Contexts;

public class AppDbContext : IdentityDbContext<AppUser>
{
    private readonly IHttpContextAccessor _contextAccessor;
    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor contextAccessor) : base(options)
    {
        _contextAccessor = contextAccessor;
    }

    public DbSet<Blog> Blogs { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<CourseCategory> courseCategories { get; set; } = null!;
    public DbSet<Category> Categories { get;set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<EventSpeaker> eventSpeakers { get; set; } = null!;
    public DbSet<Speaker> speakers { get; set; } = null!;
    public DbSet<Teacher> teachers { get; set; } = null!;
    public DbSet<TeacherSkill> teachersSkills { get; set; } = null!;
    public DbSet<Skill> skills { get; set; } = null!;
    public DbSet<SocialMedia> socialMedias { get; set; } = null!;
    public DbSet<Slider> Sliders { get; set; } = null!;
    public DbSet<Subscriber> Subscribers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>().HasQueryFilter(c => c.IsDeleted == false);
        modelBuilder.Entity<Category>().HasQueryFilter(c => c.IsDeleted == false);
        modelBuilder.Entity<Blog>().HasQueryFilter(c => c.IsDeleted == false);
        modelBuilder.Entity<Event>().HasQueryFilter(c => c.IsDeleted == false);
        modelBuilder.Entity<Teacher>().HasQueryFilter(c => c.IsDeleted == false);
        modelBuilder.Entity<Slider>().HasQueryFilter(c => c.IsDeleted == false);
        modelBuilder.Entity<Skill>().HasQueryFilter(c => c.IsDeleted == false);
        modelBuilder.Entity<Speaker>().HasQueryFilter(c => c.IsDeleted == false);
        modelBuilder.Entity<SocialMedia>().HasQueryFilter(c => c.IsDeleted == false);

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        string? name = "Admin";
        var identity = _contextAccessor?.HttpContext?.User.Identity;
        if (identity is not null)
        {
            name = identity.IsAuthenticated ? identity.Name : "Admin";
        }

        var entries = ChangeTracker.Entries<BaseSectionEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedTime = DateTime.UtcNow;
                    entry.Entity.CreatedBy = name;
                    entry.Entity.UpdatedTime = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = name;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedTime = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = name;
                    break;
                default:
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
