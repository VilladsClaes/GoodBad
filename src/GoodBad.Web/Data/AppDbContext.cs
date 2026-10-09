using GoodBad.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Aspect> Aspects => Set<Aspect>();
    public DbSet<CategoryAspect> CategoryAspects => Set<CategoryAspect>();
    public DbSet<ReviewAspect> ReviewAspects => Set<ReviewAspect>();
    public DbSet<ReviewAspectVote> ReviewAspectVotes => Set<ReviewAspectVote>();
    public DbSet<ReviewVote> ReviewVotes => Set<ReviewVote>();
    public DbSet<Fix> Fixes => Set<Fix>();
    public DbSet<FixVote> FixVotes => Set<FixVote>();
    public DbSet<FixSuggestionRule> FixSuggestionRules => Set<FixSuggestionRule>();
    public DbSet<ProductList> ProductLists => Set<ProductList>();
    public DbSet<ProductListItem> ProductListItems => Set<ProductListItem>();
    public DbSet<ProductRecommendation> ProductRecommendations => Set<ProductRecommendation>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // utf8mb4_unicode_ci is the collation Simply.com recommends and it exists
        // on both MySQL 8 and MariaDB (unlike MySQL 8's built-in 0900 collations).
        builder.UseCollation("utf8mb4_unicode_ci");

        builder.Entity<SiteSetting>().HasIndex(s => s.Key).IsUnique();

        builder.Entity<Category>()
            .HasIndex(c => c.Slug).IsUnique();
        builder.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Brand>().HasIndex(b => b.Slug).IsUnique();
        builder.Entity<Product>().HasIndex(p => p.Slug).IsUnique();
        builder.Entity<Aspect>().HasIndex(a => a.Slug).IsUnique();
        builder.Entity<ProductList>().HasIndex(l => l.Slug).IsUnique();

        builder.Entity<CategoryAspect>().HasKey(ca => new { ca.CategoryId, ca.AspectId });
        builder.Entity<CategoryAspect>()
            .HasOne(ca => ca.Category).WithMany(c => c.CategoryAspects)
            .HasForeignKey(ca => ca.CategoryId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CategoryAspect>()
            .HasOne(ca => ca.Aspect).WithMany(a => a.Categories)
            .HasForeignKey(ca => ca.AspectId).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Product>()
            .HasOne(p => p.Brand).WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Product>()
            .HasOne(p => p.Category).WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Product>()
            .HasOne(p => p.CreatedBy).WithMany()
            .HasForeignKey(p => p.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Review>()
            .HasOne(r => r.Product).WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Review>()
            .HasOne(r => r.User).WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ReviewVote>()
            .HasIndex(v => new { v.ReviewId, v.UserId }).IsUnique();
        builder.Entity<ReviewVote>()
            .HasOne(v => v.Review).WithMany(r => r.Votes)
            .HasForeignKey(v => v.ReviewId).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ReviewAspect>()
            .HasOne(ra => ra.Review).WithMany(r => r.Aspects)
            .HasForeignKey(ra => ra.ReviewId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ReviewAspectVote>()
            .HasIndex(v => new { v.ReviewAspectId, v.UserId }).IsUnique();

        builder.Entity<Fix>()
            .HasOne(f => f.Product).WithMany(p => p.Fixes)
            .HasForeignKey(f => f.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Fix>()
            .HasOne(f => f.Review).WithMany(r => r.Fixes)
            .HasForeignKey(f => f.ReviewId).OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Fix>()
            .HasOne(f => f.User).WithMany(u => u.Fixes)
            .HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<FixVote>()
            .HasIndex(v => new { v.FixId, v.UserId }).IsUnique();

        builder.Entity<ProductList>()
            .HasOne(l => l.Owner).WithMany(u => u.Lists)
            .HasForeignKey(l => l.OwnerUserId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<ProductListItem>()
            .HasIndex(i => new { i.ProductListId, i.ProductId }).IsUnique();

        builder.Entity<ProductRecommendation>()
            .HasOne(r => r.SourceProduct).WithMany(p => p.RecommendationsAsSource)
            .HasForeignKey(r => r.SourceProductId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ProductRecommendation>()
            .HasOne(r => r.AlternativeProduct).WithMany(p => p.RecommendationsAsAlternative)
            .HasForeignKey(r => r.AlternativeProductId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ProductRecommendation>()
            .HasIndex(r => new { r.SourceProductId, r.AlternativeProductId }).IsUnique();
    }
}
