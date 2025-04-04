using ScanPerson.DAL;
using ScanPerson.BusinessLogic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ScanPersonDb") ??
    throw new InvalidOperationException("Connection string 'ScanPersonDb' not found.");

// Add services to the container.
#region [Addition services]
builder.Services.AddDalServices(connectionString);
builder.Services.AddBusinessLogicServices();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowLocalhost", builder =>
	{
		builder.WithOrigins(["http://localhost:4200", "https://localhost:4200",	"http://scanperson.ui:4200","https://scanperson.ui:4200"])
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
			ValidIssuer = AuthOptions.ISSUER,
			ValidateAudience = true,
			ValidAudience = AuthOptions.AUDIENCE,
			ValidateLifetime = true,
			IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
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
}

public class AuthOptions
{
	public const string ISSUER = "MyAuthServer"; // издатель токена
	public const string AUDIENCE = "MyAuthClient"; // потребитель токена
	const string KEY = "mysupersecret_secretsecretsecretkey!123";   // ключ для шифрации
	public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
		new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}
