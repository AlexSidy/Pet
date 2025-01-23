using ScanPerson.Auth.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ScanPerson.Models.Contracts.Auth;
using ScanPerson.Auth.Api.Resources;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString(DbSection)
	?? throw new InvalidOperationException(string.Format(Messages.SectionNotFound, DbSection));
var jwtOptins = builder.Configuration.GetSection(JwtOptions.AppSettingsSection).Get<JwtOptions>()
	?? throw new InvalidOperationException(string.Format(Messages.SectionNotFound, JwtOptions.AppSettingsSection));

// Add services to the container.
#region [Addition services]
builder.Services.AddSingleton(jwtOptins);
builder.Services.AddScanPersonAuth(connectionString);
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(
		builder =>
		{
			builder
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader();
		});
});
builder.Services.AddControllers();

builder.Services
	.AddEndpointsApiExplorer()
	.AddSwaggerGen()
	.AddAuthorization();

builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = jwtOptins!.Issuer,
			ValidateAudience = true,
			ValidAudience = jwtOptins.Audience,
			ValidateLifetime = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptins.SecretKey)),
			ValidateIssuerSigningKey = true
		};
	});

#endregion [Add services]

var app = builder.Build();

#region [Usage services]
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	app.UseDeveloperExceptionPage();
}

app.UseCors(builder => builder.WithOrigins("http://localhost:4200"));
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
#endregion [Use services]

app.Run();

partial class Program
{
	public const string DbSection = "AuthDb";
}