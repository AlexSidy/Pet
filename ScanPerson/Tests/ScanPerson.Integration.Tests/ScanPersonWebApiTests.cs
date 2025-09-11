using System.Net;
using System.Net.Http.Json;
using System.Text;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Newtonsoft.Json;

using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Tests;
using ScanPerson.DAL.Contexts;
using ScanPerson.Integration.Tests.Base;
using ScanPerson.Integration.Tests.Configure;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;
using ScanPerson.WebApi.Controllers;

namespace ScanPerson.Integration.Tests
{
	[TestClass]
	public class ScanPersonWebApiTests : IntegrationTestsBase
	{
		private readonly Mock<IPersonInfoServicesAggregator> _personInfoServicesAggregator;
		private readonly Mock<ILogger<PersonInfoController>> _logger;

		public ScanPersonWebApiTests()
		{
			_personInfoServicesAggregator = new Mock<IPersonInfoServicesAggregator>();
			_logger = new Mock<ILogger<PersonInfoController>>();
			Factory = new WebApplicationFactory<Program>()
				.WithWebHostBuilder(builder =>
				{
					builder.UseEnvironment("Staging");
					Environment.SetEnvironmentVariable("HTMLWEBRU_API_KEY", "value-does-not-matter");
					Environment.SetEnvironmentVariable("JWT_OPTIONS_SECRET_KEY", "value-does-not-matter");

					builder.ConfigureServices(services =>
					{
						// Удаляем реальный сервис
						var forRemoveDescriptors = new[] {
							typeof(ILogger<PersonInfoController>),
							typeof(ScanPersonDbContext),
							typeof(IPersonInfoServicesAggregator)
						};
						RemoveFromServices(services, forRemoveDescriptors);

						// Регистрируем InMemory базу данных
						services.AddDbContext<ScanPersonDbContext>(options =>
						{
							options.UseInMemoryDatabase("ScanPersonDb");
						});

						// Подменяем на мок сервис
						services.AddTransient(_ => _logger.Object);
						services.AddSingleton(_ => _personInfoServicesAggregator.Object);

						// Удаляем реальную аутентификацию
						services.AddAuthentication("Test")
							.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

						// Заменяем политику авторизации, чтобы всегда пропускать
						services.AddAuthorizationBuilder()
							.AddPolicy("DefaultPolicy", policy =>
							{
								policy.RequireAuthenticatedUser();
							});
					});
				});
			HttpClient = Factory!.CreateDefaultClient();
		}

		[TestMethod]
		public async Task GetPersonAsync_CorrectRequest_ReturnSuccessResult()
		{
			// Arrange
			var data = JsonConvert.SerializeObject(new PersonInfoRequest
			{
				PhoneNumber = "123456789"
			});
			var personResponses = CreationHelper.GetPersonResponse();
			var taskResponse = CreationHelper.GetTaskResponse(personResponses);
			var content = new StringContent(data, Encoding.UTF8, "application/json");
			_personInfoServicesAggregator.Setup(x => x.GetScanPersonInfoAsync(It.IsAny<PersonInfoRequest>())).Returns(taskResponse!);

			// Act
			try
			{
				var response = await HttpClient!.PostAsync(
					$"{Program.WebApi}/{PersonInfoControllerName}/{nameof(PersonInfoController.GetScanPersonInfoAsync)}",
					content,
					TestContext.CancellationTokenSource.Token);

				// Assert
				Assert.IsNotNull(response);
				response.EnsureSuccessStatusCode();
				Assert.IsNotNull(response.Content.Headers.ContentType);
				Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
				var result = await response!.Content.ReadFromJsonAsync<ScanPersonResultResponse<PersonInfoItem>>(
					TestContext.CancellationTokenSource.Token);
				Assert.IsNotNull(result);
				AssertHelper.AssertResult(personResponses[0], result);
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
			var data = JsonConvert.SerializeObject(new PersonInfoRequest
			{
				PhoneNumber = "123456789"
			});
			var content = new StringContent(data, Encoding.UTF8, "application/json");
			_personInfoServicesAggregator.Setup(x => x.GetScanPersonInfoAsync(It.IsAny<PersonInfoRequest>())).Throws<InvalidOperationException>();

			// Act
			try
			{
				var response = await HttpClient!.PostAsync(
					$"{Program.WebApi}/{PersonInfoControllerName}/{nameof(PersonInfoController.GetScanPersonInfoAsync)}",
					content,
					TestContext.CancellationTokenSource.Token);

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
