using System.Data;

using FluentMigrator;

namespace ScanPerson.Auth.Api.Migrations._2024_12
{
	[Migration(202412120250)]
	public class InitiaIdentity : Migration
	{
		public const string AuthSchema = "AuthApp";
		public const string AspNetRolesTableName = "AspNetRoles";
		public const string AspNetUsersTableName = "AspNetUsers";
		public const string AspNetRoleClaimsTableName = "AspNetRoleClaims";
		public const string AspNetUserClaimsTableName = "AspNetUserClaims";
		public const string AspNetUserLoginsTableName = "AspNetUserLogins";
		public const string AspNetUserRolesTableName = "AspNetUserRoles";
		public const string AspNetUserTokensTableName = "AspNetUserTokens";

		public override void Up()
		{
			Create.Schema(AuthSchema);

			Create.Table(AspNetRolesTableName)
				.InSchema(AuthSchema)
				.WithColumn("Id").AsInt64().PrimaryKey("PK_AspNetRoles").Identity()
				.WithColumn("Name").AsString(256).Nullable()
				.WithColumn("NormalizedName").AsString(256).Nullable().Unique("RoleNameIndex")
				.WithColumn("ConcurrencyStamp").AsString().Nullable();

			Create.Table(AspNetUsersTableName)
				.InSchema(AuthSchema)
				.WithColumn("Id").AsInt64().PrimaryKey("PK_AspNetUsers").Identity()
				.WithColumn("UserName").AsString(256).Nullable()
				.WithColumn("NormalizedUserName").AsString(256).Nullable().Unique("UserNameIndex")
				.WithColumn("Email").AsString(256).Nullable()
				.WithColumn("NormalizedEmail").AsString(256).Nullable().Unique("EmailIndex")
				.WithColumn("EmailConfirmed").AsBoolean().WithDefaultValue(false)
				.WithColumn("PasswordHash").AsString(128).Nullable()
				.WithColumn("SecurityStamp").AsString(128).Nullable()
				.WithColumn("ConcurrencyStamp").AsString(128).Nullable()
				.WithColumn("PhoneNumber").AsString(128).Nullable()
				.WithColumn("PhoneNumberConfirmed").AsBoolean().WithDefaultValue(false)
				.WithColumn("TwoFactorEnabled").AsBoolean().WithDefaultValue(false)
				.WithColumn("LockoutEnd").AsDateTimeOffset().Nullable()
				.WithColumn("LockoutEnabled").AsBoolean()
				.WithColumn("AccessFailedCount").AsInt32();

			Create.Table(AspNetRoleClaimsTableName)
				.InSchema(AuthSchema)
				.WithColumn("Id").AsInt64().PrimaryKey("PK_AspNetRoleClaims").Identity()
				.WithColumn("RoleId").AsInt64()
					.ForeignKey("FK_AspNetRoleClaims_AspNetRoles_RoleId", AuthSchema, AspNetRolesTableName, "Id")
					.OnDelete(Rule.Cascade)
				.Indexed("IX_AspNetRoleClaims_RoleId")
				.WithColumn("ClaimType").AsString(256).Nullable()
				.WithColumn("ClaimValue").AsString(256).Nullable();

			Create.Table(AspNetUserClaimsTableName)
				.InSchema(AuthSchema)
				.WithColumn("Id").AsInt64().PrimaryKey("PK_AspNetUserClaims").Identity()
				.WithColumn("UserId").AsInt64()
					.ForeignKey("FK_AspNetUserClaims_AspNetUsers_UserId", AuthSchema, AspNetUsersTableName, "Id")
					.OnDelete(Rule.Cascade)
					.Indexed("IX_AspNetUserClaims_UserId")
				.WithColumn("ClaimType").AsString(256).Nullable()
				.WithColumn("ClaimValue").AsString(256).Nullable();

			Create.Table(AspNetUserLoginsTableName)
				.InSchema(AuthSchema)
				.WithColumn("LoginProvider").AsString(128)
				.WithColumn("ProviderKey").AsString(128)
				.WithColumn("ProviderDisplayName").AsString(128).Nullable()
				.WithColumn("UserId").AsInt64()
					.ForeignKey("FK_AspNetUserLogins_AspNetUsers_UserId", AuthSchema, AspNetUsersTableName, "Id")
					.OnDelete(Rule.Cascade)
				.Indexed("IX_AspNetUserLogins_UserId");

			Create.Table(AspNetUserRolesTableName)
				.InSchema(AuthSchema)
				.WithColumn("UserId").AsInt64()
					.ForeignKey("FK_AspNetUserRoles_AspNetUsers_UserId", AuthSchema, AspNetUsersTableName, "Id")
					.OnDelete(Rule.Cascade)
				.WithColumn("RoleId").AsInt64()
					.ForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId", AuthSchema, AspNetRolesTableName, "Id")
					.OnDelete(Rule.Cascade)
					.Indexed("IX_AspNetUserRoles_RoleId");

			Create.PrimaryKey("PK_AspNetUserRoles")
				.OnTable(AspNetUserRolesTableName)
				.WithSchema(AuthSchema)
				.Columns("UserId", "RoleId");

			Create.Table(AspNetUserTokensTableName)
				.InSchema(AuthSchema)
				.WithColumn("UserId").AsInt64()
					.ForeignKey("FK_AspNetUserTokens_AspNetUsers_UserId", AuthSchema, AspNetUsersTableName, "Id")
					.OnDelete(Rule.Cascade)
				.WithColumn("LoginProvider").AsString(128)
				.WithColumn("Name").AsString(128)
				.WithColumn("Value").AsString(256).Nullable();
		}

		public override void Down()
		{
			Delete.Table(AspNetRolesTableName);
			Delete.Table(AspNetUsersTableName);
			Delete.Table(AspNetRoleClaimsTableName);
			Delete.Table(AspNetUserClaimsTableName);
			Delete.Table(AspNetUserLoginsTableName);
			Delete.Table(AspNetUserRolesTableName);
			Delete.Table(AspNetUserTokensTableName);
			Delete.Schema(AuthSchema);
		}
	}
}
