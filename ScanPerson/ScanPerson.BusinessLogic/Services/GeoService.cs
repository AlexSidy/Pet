using System.Net.Http.Json;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using ScanPerson.Models.Contracts;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services
{
	/// <summary>
	/// Сервис получения информации о местонахождении человека по номеру телефона с сервиса https://htmlweb.ru/geo/telcod_api_example.php
	/// </summary>
	public class GeoService : PersonInfoServiceBase
	{
		private const string BaseUrl = "http://htmlweb.ru/geo/api.php";

		public GeoService(
			ILogger<GeoService> logger,
			IHttpClientFactory httpClientFactory,
			ScanPersonSecrets secrets,
			ServicesOptions serviceOptions)
			: base(logger, httpClientFactory, secrets, serviceOptions)
		{
		}

		protected async override Task<ScanPersonResponseBase> GetPersonInfoAsync(PersonInfoRequest request)
		{
			var parameters = new Dictionary<string, string>
			{
				["json"] = "",
				["telcod"] = request.PhoneNumber,
				["api_key"] = Secrets.HtmlWebRuApiKey
			};
			var requestUrl = QueryHelpers.AddQueryString(BaseUrl, parameters!);
			var response = await HttpClient.GetAsync(requestUrl);
			response.EnsureSuccessStatusCode();
			var locationResult = await response!.Content.ReadFromJsonAsync<LocationSerialization>();
			var result = GetSuccess(new PersonInfoItem { Location = GetMappedItem(locationResult!) });

			return result;
		}

		[Obsolete("Will deleted in task #19")]
		private LocationItem GetMappedItem(LocationSerialization serializationItem)
		{
			var mapped = new LocationItem
			{
				CountryName = serializationItem.Country?.Name,
				CurrentRegion = serializationItem.Region?.Name,
				RegistrationOkrug = serializationItem.Okrug,
				RegistrationCapital = serializationItem.Capital?.Name,
				OperatorCity = serializationItem.Operator?.Name,
				OperatorName = serializationItem.Operator?.OperBrand
			};

			return mapped;
		}
	}
}
