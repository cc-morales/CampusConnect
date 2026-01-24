using CamCon.Domain.Enitity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Xml;

namespace WebAPI.ApplicationDBContextService;
public class AppDbContext : IdentityDbContext<ApplicationUserModel>
{
    public DbSet<TokenInfoModel> TokenInfos { get; set; }
    public override DbSet<ApplicationUserModel> Users { get; set; }
    public DbSet<ProfileInfo> ProfileInformations { get; set; }
    public DbSet<MyOrganizationModel> MyOrganizations { get; set; }
    public DbSet<DepartmentModel> Departments { get; set; }
    public DbSet<NewsFeedModel> NewsFeeds { get; set; }
    public DbSet<NewsFeedImageModel> NewsFeedImages { get; set; }
    public DbSet<NewsFeedCommentModel> NewsFeedComments { get; set; }
    public DbSet<AdminPageRequestModel> RequestPages { get; set; }
    public DbSet<NotifyModel> Notifications { get; set; }
    public DbSet<PageRequestImageModel> PageRequestImages { get; set; }
    public DbSet<OrganizationDepartmentModel> OrganizationDepartments { get; set; }
    public DbSet<LikeModel> Likes { get; set; }

    public DbSet<SentimentModel> Sentiments { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ProfileInfo).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(MyOrganizationModel).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(DepartmentModel).Assembly);

        // Rename Identity tables
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

        // Rename your own table
        builder.Entity<ApplicationUserModel>().ToTable("Users");
        builder.Entity<TokenInfoModel>().ToTable("TokenInfos");
        builder.Entity<ProfileInfo>().ToTable("ProfileInformations");
        builder.Entity<MyOrganizationModel>().ToTable("MyOrganizations");
        builder.Entity<DepartmentModel>().ToTable("Departments");
        builder.Entity<NewsFeedModel>().ToTable("NewsFeeds");
        builder.Entity<NewsFeedImageModel>().ToTable("NewsFeedImages");
        builder.Entity<NewsFeedCommentModel>().ToTable("NewsFeedComments");
        builder.Entity<NotifyModel>().ToTable("Notifications");
        builder.Entity<AdminPageRequestModel>().ToTable("RequestPages");
        builder.Entity<PageRequestImageModel>().ToTable("PageRequestImages");
        builder.Entity<OrganizationDepartmentModel>().ToTable("OrganizationDepartments");
        builder.Entity<LikeModel>().ToTable("Likes");
        builder.Entity<SentimentModel>().ToTable("Sentiments");


        // Global Filter
        builder.Entity<NewsFeedCommentModel>().HasQueryFilter(e => !e.IsDeleted);
    }
}