using System.Diagnostics.CodeAnalysis;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using ScanPerson.BusinessLogic;
using ScanPerson.Common.Helpers;
using ScanPerson.Common.Resources;
using ScanPerson.DAL;
using ScanPerson.Models.Options.Auth;
using ScanPerson.WebApi.Extensions;
using ScanPerson.WebApi.Middlewares.Exceptions;

using Serilog;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

var builder = WebApplication.CreateBuilder(args);
var environmentName = builder.Environment.EnvironmentName;
var configPath = Path.Combine(AppContext.BaseDirectory, $"appsettings.{environmentName}.json");
builder.Configuration
	.AddJsonFile(configPath)
	.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString(DbSection) ??
	throw new InvalidOperationException(string.Format(Messages.SectionNotFound, DbSection));
var jwtOptins = builder.Configuration.GetSection(JwtOptions.AppSettingsSection).Get<JwtOptions>()
	?? throw new InvalidOperationException(string.Format(Messages.SectionNotFound, JwtOptions.AppSettingsSection));
jwtOptins.SecretKey = EnviromentHelper.GetViriableByName("JWT_OPTIONS_SECRET_KEY");

// Setup Serilog
Log.Logger = new LoggerConfiguration()
	.WriteTo.Graylog(new GraylogSinkOptions
	{
		HostnameOrAddress = builder.Configuration.GetSection("Graylog").GetValue<string>("Host") ?? "graylog",
		Port = builder.Configuration.GetSection("Graylog").GetValue<int?>("Port") ?? 12201,
		Facility = ProjectName,
		MinimumLogEventLevel = Serilog.Events.LogEventLevel.Information,
		TransportType = TransportType.Tcp
	})
	.CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
#region [Addition services]
// If is not testing
if (!builder.Environment.IsStaging())
{
	builder.Services.AddDalServices(connectionString);
}
builder.Services.AddBusinessLogicServices(builder.Configuration);

var allowedHosts = builder.Configuration.GetValue<string>("ALLOWED_HOSTS")?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries) ?? [];
builder.Services.AddCors(options =>
{
	options.AddPolicy(CorsPolicy, builder =>
	{
		builder.WithOrigins(allowedHosts)
			   .AllowAnyHeader()
			   .AllowAnyMethod();
	});
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
builder.Services.AddAuthorizationBuilder();
builder.Services.AddHttpClient();
builder.Services.AddScanPersonAutoMapper();
#endregion [Add services]

var app = builder.Build();

#region [Usage services]
// Configure middleware pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseExceptionHandlerMiddleware();
app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
#endregion [Use services]

await app.RunAsync();

[SuppressMessage("SonarQube", "RSPEC-1118:Method should be static", Justification = "Public modificator needs for integration tests")]
[SuppressMessage("SonarQube", "S1118:Method should be static", Justification = "Public modificator needs for integration tests")]
#pragma warning disable 1118
#pragma warning disable S1118
public partial class Program
{
	public const string WebApi = "webApi";
	public const string ProjectName = "ScanPerson.WebApi";
	public const string DbSection = "ScanPersonDb";
	public const string CorsPolicy = "MyTrustedHosts";
}
#pragma warning restore S1118
#pragma warning restore 1118