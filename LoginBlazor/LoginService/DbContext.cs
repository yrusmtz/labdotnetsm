using LoginService.Data;
using Microsoft.EntityFrameworkCore;
using LoginShared;
using LoginShared.Security.Entities;

namespace LoginService;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<PantallaEntity> Pantallas { get; set; }
    public DbSet<PantallaRoleEntity> PantallaRoles { get; set; }
    
    public DbSet<PatrocinadorEntity?> Patrocinadores { get; set; }
    public DbSet<PatrocinadorRoleEntity> PatrocinadorRoles { get; set; }
    public DbSet<SucursalEntity> Sucursales { get; set; }
    public DbSet<SucursalRoleEntity> SucursalRoles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>().ToTable("Users");
        modelBuilder.Entity<RoleEntity>().ToTable("Roles");
        modelBuilder.Entity<UserRoleEntity>().ToTable("UserRoles");
        modelBuilder.Entity<PantallaEntity>().ToTable("Pantallas").HasData(PantallaData.pantallas);
        modelBuilder.Entity<PantallaRoleEntity>().ToTable("PantallaRoles");
        modelBuilder.Entity<PatrocinadorEntity>().ToTable("Patrocinadores").HasData(PatrocinadorData.patrocinadores);
        modelBuilder.Entity<PatrocinadorRoleEntity>().ToTable("PatrocinadorRoles");
        modelBuilder.Entity<SucursalEntity>().ToTable("Sucursales").HasData(SucursalData.sucursales);
        modelBuilder.Entity<SucursalRoleEntity>().ToTable("SucursalRoles");
    }
}
