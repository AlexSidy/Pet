using ScanPerson.Models.Items;
using ScanPerson.Models.Responses;

namespace ScanPerson.Common.Tests
{
	/// <summary>
	/// Класс для проверки результатов, gjkextyys[ в тестах.
	/// </summary>
	public static class AssertHelper
	{
		/// <summary>
		/// Проверяет полученый результат с ожидаемым по информации о человеке.
		/// </summary>
		/// <param name="expected">Ожидаемый результат.</param>
		/// <param name="result">Полученный результат.</param>
		public static void AssertResult(ScanPersonResultResponse<PersonInfoItem> expected, ScanPersonResultResponse<PersonInfoItem> result)
		{
			Assert.IsTrue(result.IsSuccess);
			Assert.IsNull(result.Error);
			Assert.AreEqual(expected.Result.Names[0], result.Result.Names[0]);
			Assert.AreEqual(expected.Result.Mail, result.Result.Mail);
			Assert.AreEqual(expected.Result.Id, result.Result.Id);
			AssertLocationResult(expected, result);
		}


		/// <summary>
		/// Проверяет полученый результат с ожидаемым по информации о локации человека.
		/// </summary>
		/// <param name="expected">Ожидаемый результат.</param>
		/// <param name="result">Полученный результат.</param>
		public static void AssertLocationResult(ScanPersonResultResponse<PersonInfoItem> expected, ScanPersonResultResponse<PersonInfoItem> result)
		{
			Assert.IsTrue(result.IsSuccess);
			Assert.IsNull(result.Error);
			Assert.IsNotNull(result.Result.Location);
			Assert.AreEqual(expected!.Result!.Location.CurrentRegion, result.Result.Location.CurrentRegion);
			Assert.AreEqual(expected!.Result!.Location.OperatorName, result.Result.Location.OperatorName);
			Assert.AreEqual(expected!.Result!.Location.CountryName, result.Result.Location.CountryName);
			Assert.AreEqual(expected!.Result!.Location.OperatorCity, result.Result.Location.OperatorCity);
			Assert.AreEqual(expected!.Result!.Location.RegistrationCapital, result.Result.Location.RegistrationCapital);
			Assert.AreEqual(expected!.Result!.Location.RegistrationOkrug, result.Result.Location.RegistrationOkrug);
		}
	}
}
