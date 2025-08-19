using FluentMigrator;

namespace ScanPerson.DAL2.Migrations._2024_12
{
	[Migration(202408122050)]
	public class CreatePerson : Migration
	{
		public const string TableName = "Person";

		public override void Up()
		{
			Create.Table(TableName)
				.InSchema(InitialSheme.WebAppSchema)
				.WithColumn("Id").AsInt64().PrimaryKey().Identity()
				.WithColumn("Name").AsString()
				.WithColumn("Mail").AsString();
		}

		public override void Down()
		{
			Delete.Table(TableName).InSchema(InitialSheme.WebAppSchema);
		}
	}
}
