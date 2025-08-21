using System.Net;
using System.Net.Http.Json;
using System.Text;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Newtonsoft.Json;

using ScanPerson.BusinessLogic.Services;
using ScanPerson.DAL.Contexts;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;
using ScanPerson.WebApi.Controllers;

namespace ScanPerson.Integration.Tests
{
	[TestClass]
	public class ScanPersonWebApiTests : Xunit.IClassFixture<WebApplicationFactory<Program>>
	{
		private const string PersonControllerName = "Person";
		private readonly HttpClient _httpClient;
		private readonly WebApplicationFactory<Program> _factory;
		private readonly Mock<IPersonService> _personService;
		private readonly Mock<ILogger<PersonController>> _logger;

		public ScanPersonWebApiTests()
		{
			_personService = new Mock<IPersonService>();
			_logger = new Mock<ILogger<PersonController>>();
			_factory = new WebApplicationFactory<Program>()
				.WithWebHostBuilder(builder =>
				{
					builder.UseEnvironment("Staging");
					builder.ConfigureServices(services =>
					{
						// Удаляем реальный сервис
						var personServiceDescriptor = services.SingleOrDefault(
							d => d.ServiceType == typeof(IPersonService));
						if (personServiceDescriptor != null)
						{
							services.Remove(personServiceDescriptor);
						}

						var loggerDescriptor = services.SingleOrDefault(
							d => d.ServiceType == typeof(ILogger<PersonController>));
						if (loggerDescriptor != null)
						{
							services.Remove(loggerDescriptor);
						}

						var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ScanPersonDbContext));
						if (dbContextDescriptor != null)
						{
							services.Remove(dbContextDescriptor);
						}

						// Регистрируем InMemory базу данных
						services.AddDbContext<ScanPersonDbContext>(options =>
						{
							options.UseInMemoryDatabase("ScanPersonDb");
						});

						// Подменяем на мок сервис
						services.AddTransient(_ => _personService.Object);
						services.AddTransient(_ => _logger.Object);
					});
				});
			using (var scope = _factory.Services.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<ScanPersonDbContext>();
				context.Database.EnsureCreated();
			}
			_httpClient = _factory.CreateDefaultClient();
		}

		[TestMethod]
		public async Task GetPersonAsync_CorrectRequest_ReturnSuccessResult()
		{
			// Arrange
			var data = JsonConvert.SerializeObject(new PersonRequest
			{
				Email = "email",
				Login = "login",
				Password = "password"
			}); ;
			var personResponse = new ScanPersonResultResponse<PersonItem>(new PersonItem(1, "Test", "Mail"));
			var taskResponse = Task.FromResult(personResponse);
			var content = new StringContent(data, Encoding.UTF8, "application/json");
			_personService.Setup(x => x.FindAsync(It.IsAny<PersonRequest>())).Returns(taskResponse!);

			// Act
			try
			{
				var response = await _httpClient.PostAsync($"{Program.WebApi}/{PersonControllerName}/{nameof(PersonController.GetPersonAsync)}", content);

				// Assert
				Assert.IsNotNull(response);
				response.EnsureSuccessStatusCode();
				Assert.IsNotNull(response?.Content?.Headers?.ContentType);
				Assert.AreEqual("application/json; charset=utf-8", response?.Content?.Headers?.ContentType?.ToString());
				var result = await response!.Content.ReadFromJsonAsync<ScanPersonResultResponse<PersonItem?>?>();
				Assert.IsNotNull(result);
				Assert.IsTrue(result.IsSuccess);
				Assert.IsNull(result.Error);
				Assert.IsNotNull(result.Result);
				Assert.AreEqual(personResponse!.Result!.Name, result.Result.Name);
				Assert.AreEqual(personResponse.Result.Id, result.Result.Id);
				Assert.AreEqual(personResponse.Result.Mail, result.Result.Mail);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
		}

		[TestMethod]
		public async Task GetPersonAsync_PersonServiceThrowEception_ReturnFailResult()
		{
			// Arrange
			var data = JsonConvert.SerializeObject(new PersonRequest
			{
				Email = "email",
				Login = "login",
				Password = "password"
			});
			var content = new StringContent(data, Encoding.UTF8, "application/json");
			_personService.Setup(x => x.FindAsync(It.IsAny<PersonRequest>())).Throws<InvalidOperationException>();

			// Act
			try
			{
				var response = await _httpClient.PostAsync($"{Program.WebApi}/{PersonControllerName}/{nameof(PersonController.GetPersonAsync)}", content);

				// Assert
				Assert.IsNotNull(response);
				Assert.IsFalse(response.IsSuccessStatusCode);
				Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
		}
	}
}
