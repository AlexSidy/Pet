using Microsoft.EntityFrameworkCore;

using ScanPerson.DAL.Migrations._2024_12;

using ScanPerson.Models.Entities;

namespace ScanPerson.DAL.Contexts
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
