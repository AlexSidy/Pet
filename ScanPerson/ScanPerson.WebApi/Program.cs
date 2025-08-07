using System.Diagnostics;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using ScanPerson.BusinessLogic;
using ScanPerson.Common.Resources;
using ScanPerson.DAL;
using ScanPerson.Models.Contracts.Auth;

using Serilog;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ScanPersonDb") ??
	throw new InvalidOperationException("Connection string 'ScanPersonDb' not found.");
var jwtOptins = builder.Configuration.GetSection(JwtOptions.AppSettingsSection).Get<JwtOptions>()
	?? throw new InvalidOperationException(string.Format(Messages.SectionNotFound, JwtOptions.AppSettingsSection));

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
	.WriteTo.Graylog(new GraylogSinkOptions
	{
		HostnameOrAddress = builder.Configuration.GetSection("Graylog").GetValue<string>("Host") ?? "graylog", // адрес вашего Graylog сервера
		Port = builder.Configuration.GetSection("Graylog").GetValue<int?>("Port") ?? 12201, // порт GELF UDP/TCP (обычно 12201)
		Facility = ProjectName, // название вашего приложения
		MinimumLogEventLevel = Serilog.Events.LogEventLevel.Information,
		// Можно добавить дополнительные настройки, например, пароли или протоколы
		// Можно добавить обработчик ошибок
		TransportType = TransportType.Tcp // или Udp, в зависимости от настроек
	})
	.CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
#region [Addition services]
builder.Services.AddDalServices(connectionString);
builder.Services.AddBusinessLogicServices();

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
builder.Services.AddAuthorization();
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

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowLocalhost");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
#endregion [Use services]

app.Run();

partial class Program
{
	public const string WebApi = "webApi";
	public const string ProjectName = "ScanPerson.WebApi";
}