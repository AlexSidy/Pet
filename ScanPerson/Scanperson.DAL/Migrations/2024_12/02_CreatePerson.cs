using FluentMigrator;

namespace Scanperson.DAL.Migrations._2024_12
{
	[Migration(202408122050)]
	public class CreatePerson : Migration
	{
		public const string Tablename = "Person";

		public override void Up()
		{
			Create.Table(Tablename)
				.InSchema(InitialSheme.WebAppSchema)
				.WithColumn("Id").AsInt64().PrimaryKey().Identity()
				.WithColumn("Name").AsString()
				.WithColumn("Mail").AsString();
		}
		
		public override void Down()
		{
			Delete.Table(Tablename).InSchema(InitialSheme.WebAppSchema);
		}
	}
}
