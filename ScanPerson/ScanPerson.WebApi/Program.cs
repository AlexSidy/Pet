using ScanPerson.DAL;
using ScanPerson.BusinessLogic;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ScanPersonDb") ??
    throw new InvalidOperationException("Connection string 'ScanPersonDb' not found.");

// Add services to the container.
#region [Add services]
builder.Services.AddDalServices(connectionString);
builder.Services.AddBusinessLogicServices();
builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion [Add services]

var app = builder.Build();

#region [Use services]
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors(builder => builder.WithOrigins("http://localhost:4200"));
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
#endregion [Use services]

app.Run();
