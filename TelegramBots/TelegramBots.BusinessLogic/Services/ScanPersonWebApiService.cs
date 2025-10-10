using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using ScanPerson.Common.Resources;
using ScanPerson.Models.Items;
using ScanPerson.Models.Options;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;


using ServicesOptions = TelegramBots.BusinessLogic.Options.ServicesOptions;

namespace TelegramBots.BusinessLogic.Services
{
	/// <summary>
	/// Web api service.
	/// </summary>
	public class ScanPersonWebApiService
	{
		public const string ValidationError = "Номер телефона должен начинаться с 9 и состоять из 10 цифр.";

		private const string MethodName = "GetScanPersonInfoAsync";
		private const string PhoneNumberRule = "^9[0-9]{9}$";
		private readonly HttpClient _httpClient;
		private readonly ILogger<ScanPersonWebApiService> _logger;

		private static readonly Regex PhoneNumberRegex = new(
			PhoneNumberRule,
			RegexOptions.Compiled,
			TimeSpan.FromSeconds(1) // Устанавливаем таймаут, например, 1 секунда
		);

		public ScanPersonWebApiService(ILogger<ScanPersonWebApiService> logger, HttpClient httpClient, ServicesOptions options)
		{
			_logger = logger;
			_httpClient = httpClient;
			var serviceHostOptions = options.ScanPersonWebApiServiceOptions.ServiceHostOptions;
			_httpClient.BaseAddress = GetBaseUrl(serviceHostOptions!);
		}

		public async Task<string?> GetDataAsync(string? phoneNumber, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation(Messages.StartedMethodWithParameters, nameof(GetDataAsync), phoneNumber);
				if (!PhoneNumberRegex.IsMatch(phoneNumber!))
				{
					return ValidationError;
				}
				var data = JsonSerializer.Serialize(new PersonInfoRequest
				{
					PhoneNumber = phoneNumber
				});
				var content = new StringContent(data, Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync(MethodName, content, cancellationToken);
				response.EnsureSuccessStatusCode();
				var result = await response!.Content.ReadFromJsonAsync<ScanPersonResultResponse<PersonInfoItem>>(cancellationToken);
				if (result == null || !result.IsSuccess)
				{
					_logger.LogInformation(Messages.OperationError, result?.Error);
					return Messages.ClientOperationError;
				}

				_logger.LogInformation(Messages.OperationResult, JsonSerializer.Serialize(result));

				return result.Result?.ToString();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, Messages.OperationError, ex.Message);
				return Messages.ClientOperationError;
			}
		}

		/// <summary>
		/// Get base url.
		/// </summary>
		/// <returns>Base url.</returns>
		private static Uri GetBaseUrl(ServiceHostOptions options)
		{
			return new Uri(
				$"{options.Host}" +
				$":{options.Port}" +
				$"/{options.ApiVersion}" +
				$"/{options.ControllerName}/");
		}
	}
}
