using System.Net;
using System.Text;

using ScanPerson.Models.Items;
using ScanPerson.Models.Responses;

namespace ScanPerson.Common.Tests
{
	/// <summary>
	/// Класс для создания объектов, используемых в тестах.
	/// </summary>
	public sealed class CreationHelper
	{
		/// <summary>
		/// Получает шаблонный ответ от сервера с ошибкой 500.
		/// </summary>
		/// <returns>неуспешный ответ с сервера.</returns>
		public static HttpResponseMessage GetInternalServerErrorHttpMessage()
		{
			return new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.InternalServerError,
				Content = new StringContent(GetInternalServerErrorJsonResponse(), Encoding.UTF8, "application/json")
			};
		}

		/// <summary>
		/// Получает шаблонный ответ от сервера с кодом 200.
		/// </summary>
		/// <returns>Успешный ответ с сервера.</returns>
		public static HttpResponseMessage GetSuccessHttpMessage()
		{
			return new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(GetGeoJsonResponse(), Encoding.UTF8, "application/json")
			};
		}

		/// <summary>
		/// Получает успешный общий ответ с сервиса.
		/// </summary>
		/// <returns></returns>
		public static ScanPersonResultResponse<PersonInfoItem>[] GetPersonResponse()
			=> [ new ScanPersonResultResponse<PersonInfoItem>(
				new PersonInfoItem
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
				})];

		/// <summary>
		/// Получает успешный общий ответ с сервиса обернутый в Task.
		/// </summary>
		/// <param name="personResponse">Результат полученный от сервиса.</param>
		/// <returns>Задача с результатом операции.</returns>
		public static Task<ScanPersonResponseBase[]> GetTaskResponse(ScanPersonResultResponse<PersonInfoItem>[] personResponses)
		{
			return Task.FromResult<ScanPersonResponseBase[]>(personResponses);
		}

		/// <summary>
		/// Получает успешный ответ с сервиса  https://htmlweb.ru/geo/telcod_api_example.php
		/// </summary>
		/// <returns>Тест в виде json объекта.</returns>
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
		/// Получает неуспешный ответ с сервиса  https://htmlweb.ru/geo/telcod_api_example.php
		/// </summary>
		/// <returns>Тест в виде json объекта.</returns>
		private static string GetInternalServerErrorJsonResponse()
		{
			return @"{ ""message"": ""Internal server error"", ""code"": 500 }";
		}
	}
}
