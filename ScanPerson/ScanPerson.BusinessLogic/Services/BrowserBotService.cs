using System.Net.Http.Json;
using System.Text.Json;

using AutoMapper;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using ScanPerson.Common.Resources;
using ScanPerson.Models.Items;
using ScanPerson.Models.Options;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services
{
	/// <summary>
	/// Service for getting information about a person's location by phone number from the service https://htmlweb.ru/geo/telcod_api_example.php
	/// </summary>
	public class BrowserBotService(
		ILogger<BrowserBotService> logger,
		IHttpClientFactory httpClientFactory,
		ScanPersonSecrets secrets,
		ServicesOptions serviceOptions,
		IMapper mapper) : PersonInfoServiceBase(logger, httpClientFactory, secrets, serviceOptions, mapper)
	{
		protected async override Task<ScanPersonResponseBase> GetAnyPersonInfoAsync(PersonInfoRequest request)
		{
			// Due to the problem of using a single profile on the service and accessing profile data in a single
			// process, you need to connect to the BrowserBotService sequentially.
			var namePersonResult = await GetNamesByServiceMethodAsync<string>(request, "GetNameByPhoneNumberAsync");
			var namesPersonResult = await GetNamesByServiceMethodAsync<string[]>(request, "GetNamesByPhoneNumberAsync");

			var (names, errors) = GetConcationationResult(namePersonResult, namesPersonResult);
			Logger.LogInformation(Messages.OperationResult, JsonSerializer.Serialize(names));

			return names.Length != 0
				? GetSuccess(new PersonInfoItem { Names = names }, errors)
				: GetFail(errors);
		}

		/// <summary>
		/// Get concatenation values from two results.
		/// </summary>
		/// <param name="nameResult">Result with name.</param>
		/// <param name="namesResult">Result with names.</param>
		/// <returns>Array of names.</returns>
		private static (string[] names, string[] errors) GetConcationationResult(
			ScanPersonResultResponse<string> nameResult,
			ScanPersonResultResponse<string[]> namesResult)
		{
			string[] result = [
				.. nameResult.IsSuccess ? new[] { nameResult.Result } : [],
				.. namesResult.IsSuccess ? namesResult.Result : [] ];

			string[] errors = [
				.. nameResult.IsSuccess ? [] : new[] { nameResult.Error },
				.. namesResult.IsSuccess ? [] : new[] { namesResult.Error } ];

			return ([.. result.Distinct()], [.. errors.Distinct()]);

		}

		/// <summary>
		/// Get parameters for get request.
		/// </summary>
		/// <param name="request">Request object.</param>
		/// <returns>Dictionary with parameters.</returns>
		private static Dictionary<string, string> GetParameters(PersonInfoRequest request)
		{
			return new Dictionary<string, string>
			{
				["phoneNumber"] = request.PhoneNumber
			};
		}

		/// <summary>
		/// Get names person info from BrowserBotService.
		/// </summary>
		/// <param name="request">Request object.</param>
		/// <returns>Result operation as response wrapper with string[] result.</returns>
		private async Task<ScanPersonResultResponse<T>>
			GetNamesByServiceMethodAsync<T>(PersonInfoRequest request, string methodName)
			where T : class
		{
			var response = await HttpClient.GetAsync(GetRequestByMethodName(request, methodName));
			if (response.IsSuccessStatusCode)
			{
				var result = await response!.Content.ReadFromJsonAsync<ScanPersonResultResponse<T>>();
				Logger.LogInformation(Messages.OperationResult, JsonSerializer.Serialize(result));
				return result!;
			}

			string errorContent = await response.Content.ReadAsStringAsync();
			return (ScanPersonResultResponse<T>)GetFail<T>(
					$"Response status code does not indicate success:" +
					$"{(int)response.StatusCode} ({response.ReasonPhrase}). " +
					$"Content: {errorContent}");
		}

		/// <summary>
		/// Get request by method name.
		/// </summary>
		/// <param name="request">Request object.</param>
		/// <param name="methodName">Method name of service.</param>
		/// <returns>Full url with parameters.</returns>
		private string GetRequestByMethodName(PersonInfoRequest request, string methodName)
		{
			var parameters = GetParameters(request);
			var url = GetBaseUrl() + methodName;
			var requestUrl = QueryHelpers.AddQueryString(url, parameters!);

			return requestUrl;
		}

		/// <summary>
		/// Get base url.
		/// </summary>
		/// <returns>Base url.</returns>
		private string GetBaseUrl()
		{
			return
				$"{ServicesOptions.BrowserBotServiceOptions.ServiceHostOptions.Host}" +
				$":{ServicesOptions.BrowserBotServiceOptions.ServiceHostOptions.Port}" +
				$"/{ServicesOptions.BrowserBotServiceOptions.ServiceHostOptions.ApiVersion}" +
				$"/{ServicesOptions.BrowserBotServiceOptions.ServiceHostOptions.ControllerName}/";
		}
	}
}
