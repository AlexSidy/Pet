using System.Net;
using System.Net.Http.Json;
using System.Text;

using Identity.Api;
using Identity.Api.Controllers;
using Identity.Api.Services;
using Identity.Api.Services.Interfaces;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;
using Testcontainers.PostgreSql;
using Xunit;

using Newtonsoft.Json;

using ScanPerson.Models.Requests.Auth;
using ScanPerson.Models.Responses;

namespace ScanPerson.Integration.Tests
{
	[TestClass]
	public class IdentityApiTests : IAsyncLifetime
	{
		public TestContext TestContext { get; set; }

		private const string AuthControllerName = "Auth";
		private HttpClient? _httpClient;
		private WebApplicationFactory<Program>? _factory;
		private Mock<IUserService>? _userService;
		private Mock<ILogger<AuthController>>? _logger;
		private PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
		.WithDatabase("test_db")
		.WithUsername("testuser")
		.WithPassword("testpassword")
		.Build();

		[TestInitialize]
		public async Task InitializeAsync()
		{
			await _postgreSqlContainer.StartAsync();
			var connectionString = _postgreSqlContainer.GetConnectionString();

			_userService = new Mock<IUserService>();
			_logger = new Mock<ILogger<AuthController>>();
			_factory = new WebApplicationFactory<Program>()
				.WithWebHostBuilder(builder =>
				{
					builder.UseEnvironment("Staging");
					Environment.SetEnvironmentVariable("JWT_OPTIONS_SECRET_KEY", "value-does-not-matter");
					Environment.SetEnvironmentVariable("ConnectionStrings__IdentityDb", connectionString);

					builder.ConfigureServices(services =>
					{
						// Удаляем реальный сервис
						var forRemovedescritors = new[] {
							typeof(ILogger<AuthController>),
							typeof(IUserService),
							typeof(JwtProvider),
							typeof(AuthDbContext)
						};
						RemoveFromServices(services, forRemovedescritors);

						// Регистрируем InMemory базу данных
						services.AddDbContext<AuthDbContext>(options =>
						{
							options.UseInMemoryDatabase("IdentityDb");
						});

						// Подменяем на мок сервис
						services.AddTransient(_ => _logger.Object);
						services.AddSingleton(_ => _userService.Object);
					});
				});
			_httpClient = _factory.CreateDefaultClient();
		}

		[TestCleanup]
		public async Task DisposeAsync()
		{
			await _postgreSqlContainer.DisposeAsync();
		}

		private static void RemoveFromServices(IServiceCollection services, IEnumerable<Type> types)
		{
			foreach (var type in types)
			{
				var personServiceDescriptor = services.SingleOrDefault(
											d => d.ServiceType == type);
				if (personServiceDescriptor != null)
				{
					services.Remove(personServiceDescriptor);
				}
			}
		}

		[TestMethod]
		public async Task RegisterAsync_CorrectRequest_ReturnSuccessResult()
		{
			// Arrange
			var data = JsonConvert.SerializeObject(new RegisterRequest
			{
				Password = "123456789",
				Email = "3uQ5f@example.com"
			});
			var expectedResponse = new ScanPersonResultResponse<IdentityResult>(IdentityResult.Success);
			var taskResponse = Task.FromResult<ScanPersonResponseBase>(expectedResponse);
			var content = new StringContent(data, Encoding.UTF8, "application/json");
			_userService!.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>())).Returns(taskResponse);

			// Act
			try
			{
				var response = await _httpClient!.PostAsync(
					$"{Program.AuthApi}/{AuthControllerName}/{nameof(AuthController.RegisterAsync)}",
					content,
					TestContext.CancellationTokenSource.Token);

				// Assert
				Assert.IsNotNull(response);
				response.EnsureSuccessStatusCode();
				Assert.IsNotNull(response.Content.Headers.ContentType);
				Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
				var result = await response!.Content.ReadFromJsonAsync<ScanPersonResultResponse<IdentityResult>>(
					TestContext.CancellationTokenSource.Token);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.IsSuccess);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
		}

		[TestMethod]
		public async Task RegisterAsync_UserServiceReturnFailResult_ReturnFailResult()
		{
			// Arrange
			var data = JsonConvert.SerializeObject(new RegisterRequest
			{
				Password = "123456789",
				Email = "3uQ5f@example.com"
			});
			var error = "Error";
			var expectedResponse = new ScanPersonResultResponse<IdentityResult>(error);
			var taskResponse = Task.FromResult<ScanPersonResponseBase>(expectedResponse);
			var content = new StringContent(data, Encoding.UTF8, "application/json");
			_userService!.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>())).Returns(taskResponse);

			// Act
			try
			{
				var response = await _httpClient!.PostAsync(
					$"{Program.AuthApi}/{AuthControllerName}/{nameof(AuthController.RegisterAsync)}",
					content,
					TestContext.CancellationTokenSource.Token);

				// Assert
				Assert.IsNotNull(response);
				Assert.IsFalse(response.IsSuccessStatusCode);
				Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
		}

		[TestMethod]
		public async Task RegisterAsync_UserServiceThrowException_ThrowsException()
		{
			// Arrange
			var data = JsonConvert.SerializeObject(new RegisterRequest
			{
				Password = "123456789",
				Email = "3uQ5f@example.com"
			});
			var content = new StringContent(data, Encoding.UTF8, "application/json");
			_userService!.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>())).Throws<InvalidOperationException>();

			// Act & Assert
			await Assert.ThrowsExactlyAsync<InvalidOperationException>(
					() => _httpClient!.PostAsync(
						$"{Program.AuthApi}/{AuthControllerName}/{nameof(AuthController.RegisterAsync)}",
						content,
						TestContext.CancellationTokenSource.Token));
		}

		[TestMethod]
		public async Task LoginAsync_CorrectRequest_ReturnSuccessResult()
		{
			// Arrange
			var data = JsonConvert.SerializeObject(new LoginRequest
			{
				Password = "123456789",
				Email = "3uQ5f@example.com"
			});
			var expectedResponse = new ScanPersonResultResponse<IdentityResult>(IdentityResult.Success);

			var taskResponse = Task.FromResult<ScanPersonResponseBase>(expectedResponse);
			var content = new StringContent(data, Encoding.UTF8, "application/json");
			_userService!.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>())).Returns(taskResponse);

			// Act
			try
			{
				var response = await _httpClient!.PostAsync(
					$"{Program.AuthApi}/{AuthControllerName}/{nameof(AuthController.LoginAsync)}",
					content,
					TestContext.CancellationTokenSource.Token);

				// Assert
				Assert.IsNotNull(response);
				response.EnsureSuccessStatusCode();
				Assert.IsNotNull(response.Content.Headers.ContentType);
				Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
				var result = await response!.Content.ReadFromJsonAsync<ScanPersonResultResponse<IdentityResult>>(
					TestContext.CancellationTokenSource.Token);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.IsSuccess);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
		}

		[TestMethod]
		public async Task LoginAsync_UserServiceReturnFailResult_ReturnFailResult()
		{
			// Arrange
			var data = JsonConvert.SerializeObject(new RegisterRequest
			{
				Password = "123456789",
				Email = "3uQ5f@example.com"
			});
			var error = "Error";
			var expectedResponse = new ScanPersonResultResponse<IdentityResult>(error);
			var taskResponse = Task.FromResult<ScanPersonResponseBase>(expectedResponse);
			var content = new StringContent(data, Encoding.UTF8, "application/json");
			_userService!.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>())).Returns(taskResponse);

			// Act
			try
			{
				var response = await _httpClient!.PostAsync(
					$"{Program.AuthApi}/{AuthControllerName}/{nameof(AuthController.LoginAsync)}",
					content,
					TestContext.CancellationTokenSource.Token);

				// Assert
				Assert.IsNotNull(response);
				Assert.IsFalse(response.IsSuccessStatusCode);
				Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
		}

		[TestMethod]
		public async Task LoginAsync_UserServiceThrowException_ThrowsException()
		{
			// Arrange
			var data = JsonConvert.SerializeObject(new LoginRequest
			{
				Password = "123456789",
				Email = "3uQ5f@example.com"
			});
			var content = new StringContent(data, Encoding.UTF8, "application/json");
			_userService!.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>())).Throws<InvalidOperationException>();

			// Act & Assert
			await Assert.ThrowsExactlyAsync<InvalidOperationException>(
					() => _httpClient!.PostAsync(
						$"{Program.AuthApi}/{AuthControllerName}/{nameof(AuthController.LoginAsync)}",
						content,
						TestContext.CancellationTokenSource.Token));
		}
	}
}
