using Microsoft.EntityFrameworkCore;

using ScanPerson.DAL2.Migrations._2024_12;

using ScanPerson.Models.Entities;

namespace ScanPerson.DAL2.Contexts
{
	public class ScanPersonDbContext : DbContext
	{
		public ScanPersonDbContext() { }
		public ScanPersonDbContext(DbContextOptions<ScanPersonDbContext> options) : base(options)
		{
		}

		public DbSet<PersonEntity>? Persons { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema(InitialSheme.WebAppSchema);
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<PersonEntity>().ToTable(CreatePerson.TableName);
		}
	}
}
