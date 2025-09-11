using System.Net.Http.Json;

using AutoMapper;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using ScanPerson.Models.Items;
using ScanPerson.Models.Options;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services
{
	/// <summary>
	/// Service for getting information about a person's location by phone number from the service https://htmlweb.ru/geo/telcod_api_example.php
	/// </summary>
	public class GeoService : PersonInfoServiceBase
	{
		private readonly string _baseUrl;

		public GeoService(
			ILogger<GeoService> logger,
			IHttpClientFactory httpClientFactory,
			ScanPersonSecrets secrets,
			ServicesOptions serviceOptions,
			IMapper mapper)
			: base(logger, httpClientFactory, secrets, serviceOptions, mapper)
		{
			_baseUrl = serviceOptions.GeoServiceOptions.BaseUrl;
		}

		protected async override Task<ScanPersonResponseBase> GetPersonInfoAsync(PersonInfoRequest request)
		{
			var parameters = new Dictionary<string, string>
			{
				["json"] = "",
				["telcod"] = request.PhoneNumber,
				["api_key"] = Secrets.HtmlWebRuApiKey
			};
			var requestUrl = QueryHelpers.AddQueryString(_baseUrl, parameters!);
			var response = await HttpClient.GetAsync(requestUrl);
			response.EnsureSuccessStatusCode();
			var locationResult = await response!.Content.ReadFromJsonAsync<LocationDeserialized>();
			var result = GetSuccess(new PersonInfoItem { Location = Mapper.Map<LocationItem>(locationResult) });

			return result;
		}
	}
}
