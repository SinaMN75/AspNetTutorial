namespace AspNetTutorial;

using Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions options) : DbContext(options) {
	
	public DbSet<UserEntity> Users { get; set; } = null!;
	public DbSet<SchoolEntity> School { get; set; } = null!;
	public DbSet<ClassEntity> Class { get; set; } = null!;
	
}