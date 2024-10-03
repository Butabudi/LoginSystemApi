using LoginSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginSystem.Domain;

public class LoginSystemDbContext : DbContext
{
    public LoginSystemDbContext(DbContextOptions<LoginSystemDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoginSystemDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

        UserModelBuilder(modelBuilder);
    }

    private static void UserModelBuilder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(x => x.Id);

        modelBuilder
            .Entity<User>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        modelBuilder
            .Entity<User>()
            .Property<Guid>("Id")
            .ValueGeneratedOnAdd()
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");
    }
}
