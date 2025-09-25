using System.Net;
using System.Net.Http.Json;
using System.Text;

using GetContactAPI;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Newtonsoft.Json;

using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Tests;
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
		private Mock<IPersonInfoServicesAggregator>? _personInfoServicesAggregator;
		private Mock<ILogger<PersonInfoController>>? _logger;

		[TestInitialize]
		public override async Task InitializeAsync()
		{
			await InitializeBdAndSetConnectionStringAsync();

			_personInfoServicesAggregator = new Mock<IPersonInfoServicesAggregator>();
			_logger = new Mock<ILogger<PersonInfoController>>();
			Factory = new WebApplicationFactory<Program>()
				.WithWebHostBuilder(builder =>
				{
					builder.UseEnvironment("Staging");
					SetTestEnvironment();

					builder.ConfigureServices(services =>
					{
						// Удаляем реальный сервис
						var forRemoveDescriptors = new[] {
							typeof(ILogger<PersonInfoController>),
							typeof(IPersonInfoServicesAggregator)
						};
						RemoveFromServices(services, forRemoveDescriptors);

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
		public async Task MyGetContactAPI()
		{
			var phone = "+79253047679";
			var api = new GetContact(new Data(
				"cOYkBY8c68caa8f1384da416ab39b9cc173227f57d1cde6d2abca4bba0",
				"84a1e9d85986266055bb416e3bcf059b47262e777bb6a6d9a683499589aebe8e"
				));
			var phoneInfo = await api.GetByPhoneAsync(phone, TestContext.CancellationTokenSource.Token);
			var tagsInfo = await api.GetTagsAsync(phone, TestContext.CancellationTokenSource.Token);

			if (phoneInfo.Meta.IsRequestError)
			{
				// ваша обработка ошибки
				return;
			}

			if (tagsInfo.Meta.IsRequestError)
			{
				// ваша обработка ошибки
				return;
			}

			string name = phoneInfo.Response.Profile.DisplayName;
		}

		[TestMethod]
		public async Task MyTestVoxlinkRu()
		{
			var phoneNumber = "+79253047679";
			var url = $"http://num.voxlink.ru/get/?num={phoneNumber}";
			using (var client = new HttpClient())
			{
				var response = await client.GetAsync(url, TestContext.CancellationTokenSource.Token);
				response.EnsureSuccessStatusCode();
				var locationResult = await response!.Content.ReadAsStringAsync();
				//				{
				//					"code": "925",
				//					"num": "3047679",
				//					"full_num": "9253047679",
				//					"operator": "Билайн",
				//					"old_operator": "МЕГАФОН",
				//					"region": "Москва и Московская область"
				//				}
			}
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
			_personInfoServicesAggregator!.Setup(x => x.GetScanPersonInfoAsync(It.IsAny<PersonInfoRequest>())).Returns(taskResponse!);

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
			_personInfoServicesAggregator!.Setup(x => x.GetScanPersonInfoAsync(It.IsAny<PersonInfoRequest>())).Throws<InvalidOperationException>();

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
