using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using ScanPerson.Auth.Api;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Contracts.Auth;

using Serilog;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString(DbSection)
	?? throw new InvalidOperationException(string.Format(Messages.SectionNotFound, DbSection));
var jwtOptins = builder.Configuration.GetSection(JwtOptions.AppSettingsSection).Get<JwtOptions>()
	?? throw new InvalidOperationException(string.Format(Messages.SectionNotFound, JwtOptions.AppSettingsSection));

// Setup Serilog
Log.Logger = new LoggerConfiguration()
	.WriteTo.Graylog(new GraylogSinkOptions
	{
		HostnameOrAddress = builder.Configuration.GetSection("Graylog").GetValue<string>("Host") ?? "graylog", // graylog`s hostname
		Port = builder.Configuration.GetSection("Graylog").GetValue<int?>("Port") ?? 12201, // port GELF UDP/TCP (ussualy 12201)
		Facility = ProjectName, // project name
		MinimumLogEventLevel = Serilog.Events.LogEventLevel.Information,
		TransportType = TransportType.Tcp
	})
	.CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
#region [Addition services]
builder.Services.AddSingleton(jwtOptins);
builder.Services.AddScanPersonAuth(connectionString);
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowLocalhost", builder =>
	{
		builder.WithOrigins(["http://localhost:4200", "https://localhost:4200", "http://scanperson.ui:4200", "https://scanperson.ui:4200"])
			   .AllowAnyHeader() // Разрешаем любые заголовки
			   .AllowAnyMethod(); // Разрешаем любые методы
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
			ValidIssuer = jwtOptins.Issuer,
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

app.UseCors("AllowLocalhost");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
#endregion [Use services]

app.Run();

partial class Program
{
	public const string DbSection = "AuthDb";
	public const string AuthApi = "authApi";
	public const string ProjectName = "ScanPerson.Auth.Api";
}