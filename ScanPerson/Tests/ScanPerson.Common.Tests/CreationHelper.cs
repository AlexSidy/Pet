using System.Net;
using System.Text;

using ScanPerson.Models.Items;
using ScanPerson.Models.Responses;

namespace ScanPerson.Common.Tests
{
	/// <summary>
	/// A class for creating objects used in tests.
	/// </summary>
	public sealed class CreationHelper
	{
		/// <summary>
		/// Gets a template response from the server with a 500 error code.
		/// </summary>
		/// <returns>The unsuccessful response from the server.</returns>
		public static HttpResponseMessage GetInternalServerErrorHttpMessage()
		{
			return new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.InternalServerError,
				Content = new StringContent(GetInternalServerErrorJsonResponse(), Encoding.UTF8, "application/json")
			};
		}

		/// <summary>
		/// Gets a template response from the server with a 200 status code.
		/// </summary>
		/// <returns>The successful response from the server.</returns>
		public static HttpResponseMessage GetSuccessHttpMessage(string? response = null)
		{
			return new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(response ?? GetGeoJsonResponse(), Encoding.UTF8, "application/json")
			};
		}

		/// <summary>
		/// Gets a successful response from the service.
		/// </summary>
		/// <returns>The result containing an person information.</returns>
		public static ScanPersonResultResponse<PersonInfoItem> GetPersonResponse()
			=> new ScanPersonResultResponse<PersonInfoItem>(
				GetPerson());

		/// <summary>
		/// Get person info.
		/// </summary>
		/// <returns>Person Info</returns>
		private static PersonInfoItem GetPerson()
		{
			return new PersonInfoItem
			{
				Id = 1,
				Names = ["Name"],
				Mail = "Mail",
				Location = new LocationItem
				{
					CountryName = "Россия",
					CurrentRegion = "Московская область",
					OperatorCity = "Москва",
					OperatorName = "Билайн",
					RegistrationCapital = "Москва",
					RegistrationOkrug = "Центральный федеральный округ"
				}
			};
		}

		/// <summary>
		/// Gets a successful aggregated response from the service, wrapped in a Task.
		/// </summary>
		/// <param name="personResponse">The result received from the service.</param>
		/// <returns>A Task representing the result of the operation.</returns>
		public static Task<ScanPersonResponseBase> GetTaskResponse(ScanPersonResultResponse<PersonInfoItem> personResponses)
		{
			return Task.FromResult<ScanPersonResponseBase>(personResponses);
		}

		/// <summary>
		/// Gets a successful response from the service https://htmlweb.ru/geo/telcod_api_example.php
		/// </summary>
		/// <returns>The test result as a JSON object.</returns>
		private static string GetGeoJsonResponse()
		{
			return @"
			{
			    ""country"": { ""name"": ""Россия"" },
			    ""region"": { ""name"": ""Московская область"" },
			    ""okrug"": ""Центральный федеральный округ"",
			    ""capital"": { ""name"": ""Москва"" },
			    ""0"": { ""name"": ""Москва"", ""oper_brand"": ""Билайн"" }
			}";
		}

		/// <summary>
		/// Gets an unsuccessful response from the http client.
		/// </summary>
		/// <returns>The test result as a JSON object.</returns>
		private static string GetInternalServerErrorJsonResponse()
		{
			return @"{ ""message"": ""Internal server error"", ""code"": 500 }";
		}
	}
}
