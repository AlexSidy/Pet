using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Moq;

using Newtonsoft.Json;

using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Tests;
using ScanPerson.Integration.Tests.Base;
using ScanPerson.Models.Items;
using ScanPerson.Models.Options.Auth;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;
using ScanPerson.WebApi.Controllers;

namespace ScanPerson.Integration.Tests
{
	[TestClass]
	public class AuthenticationTests : IntegrationTestsBase
	{
		private readonly JwtOptions? _jwtOptions;
		private readonly Mock<IPersonInfoServicesAggregator> _personInfoServicesAggregator;
		private readonly Mock<ILogger<PersonInfoController>> _logger;

		public AuthenticationTests()
		{
			_personInfoServicesAggregator = new Mock<IPersonInfoServicesAggregator>();
			_logger = new Mock<ILogger<PersonInfoController>>();
			Factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder =>
			{
				builder.UseEnvironment("Staging");
				SetTestEnvironment();
				builder.ConfigureServices(services =>
				{
					var forRemoveDescriptors = new[] {
							typeof(ILogger<PersonInfoController>),
							typeof(IPersonInfoServicesAggregator)
						};
					RemoveFromServices(services, forRemoveDescriptors);

					services.AddTransient(_ => _logger.Object);
					services.AddSingleton(_ => _personInfoServicesAggregator.Object);
				});
			});

			using var scope = Factory.Services.CreateScope();
			var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
			_jwtOptions = configuration.GetSection(JwtOptions.AppSettingsSection).Get<JwtOptions>();
			HttpClient = Factory!.CreateDefaultClient();
		}

		/// <summary>
		/// Helper method for generating JWT token
		/// </summary>
		/// <param name="username">User`s name.</param>
		/// <returns>Token as string.</returns>
		private string GenerateJwtToken(string username)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.Name, username),
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				_jwtOptions?.Issuer,
				_jwtOptions?.Audience,
				claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		[TestMethod]
		public async Task ProtectedEndpoint_WithValidJwtToken_ReturnsOk()
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
			var token = GenerateJwtToken("testuser");

			HttpClient!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			// Act
			try
			{
				var response = await HttpClient.PostAsync(
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
		public async Task ProtectedEndpoint_WithInvalidJwtToken_ReturnsUnauthorized()
		{
			// Arrange
			HttpClient!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");
			var data = JsonConvert.SerializeObject(new PersonInfoRequest
			{
				PhoneNumber = "123456789"
			});
			var personResponses = CreationHelper.GetPersonResponse();
			var taskResponse = CreationHelper.GetTaskResponse(personResponses);
			var content = new StringContent(data, Encoding.UTF8, "application/json");

			// Act
			try
			{
				var response = await HttpClient.PostAsync(
					$"{Program.WebApi}/{PersonInfoControllerName}/{nameof(PersonInfoController.GetScanPersonInfoAsync)}",
					content,
					TestContext.CancellationTokenSource.Token);

				// Assert
				Assert.IsNotNull(response);
				Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
		}
	}
}