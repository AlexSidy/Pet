using FluentMigrator;

namespace ScanPerson.DAL.Migrations._2024_12
{
	[Migration(202408122040)]
	public class InitialSheme : Migration
	{
		public const string WebAppSchema = "WebApp";

		public override void Up()
		{
			Create.Schema(WebAppSchema);
		}

		public override void Down()
		{
			Delete.Schema(WebAppSchema);
		}
	}
}
