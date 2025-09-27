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
		public static void AssertResult(ScanPersonResultResponse<PersonInfoItem[]> expected, ScanPersonResultResponse<PersonInfoItem[]> result)
		{
			Assert.IsTrue(result.IsSuccess);
			Assert.IsNull(result.Error);
			Assert.AreEqual(expected.Result[0].Names[0], result.Result[0].Names[0]);
			Assert.AreEqual(expected.Result[0].Mail, result.Result[0].Mail);
			Assert.AreEqual(expected.Result[0].Id, result.Result[0].Id);
			AssertLocationResultWithPersons(expected, result);
		}


		/// <summary>
		/// Checks the received result against the expected result for person location information.
		/// </summary>
		/// <param name="expected">The expected result.</param>
		/// <param name="result">The received result.</param>
		public static void AssertLocationResultWithPersons(ScanPersonResultResponse<PersonInfoItem[]> expected, ScanPersonResultResponse<PersonInfoItem[]> result)
		{
			Assert.IsTrue(result.IsSuccess);
			Assert.IsNull(result.Error);
			Assert.IsNotNull(result.Result[0].Location);
			Assert.AreEqual(expected!.Result![0].Location.CurrentRegion, result.Result[0].Location.CurrentRegion);
			Assert.AreEqual(expected!.Result![0].Location.OperatorName, result.Result[0].Location.OperatorName);
			Assert.AreEqual(expected!.Result![0].Location.CountryName, result.Result[0].Location.CountryName);
			Assert.AreEqual(expected!.Result![0].Location.OperatorCity, result.Result[0].Location.OperatorCity);
			Assert.AreEqual(expected!.Result![0].Location.RegistrationCapital, result.Result[0].Location.RegistrationCapital);
			Assert.AreEqual(expected!.Result![0].Location.RegistrationOkrug, result.Result[0].Location.RegistrationOkrug);
		}

		/// <summary>
		/// Checks the received result against the expected result for person information.
		/// </summary>
		/// <param name="expected">The expected result.</param>
		/// <param name="result">The received result.</param>
		public static void AssertPersonInfo(PersonInfoItem expected, PersonInfoItem result)
		{
			Assert.IsNotNull(result.Location);
			Assert.AreEqual(expected!.Id, result.Id);
			Assert.AreEqual(expected!.Mail, result.Mail);
			Assert.AreEqual(expected!.Names, result.Names);
			Assert.AreEqual(expected!.Location.CurrentRegion, result.Location.CurrentRegion);
			Assert.AreEqual(expected!.Location.OperatorName, result.Location.OperatorName);
			Assert.AreEqual(expected!.Location.CountryName, result.Location.CountryName);
			Assert.AreEqual(expected!.Location.OperatorCity, result.Location.OperatorCity);
			Assert.AreEqual(expected!.Location.RegistrationCapital, result.Location.RegistrationCapital);
			Assert.AreEqual(expected!.Location.RegistrationOkrug, result.Location.RegistrationOkrug);
		}

		/// <summary>
		/// Проверяет полученый результат с ожидаемым по информации о локации человека.
		/// </summary>
		/// <param name="expected">Ожидаемый результат.</param>
		/// <param name="result">Полученный результат.</param>
		public static void AssertLocationResultWithPerson(ScanPersonResultResponse<PersonInfoItem> expected, ScanPersonResultResponse<PersonInfoItem> result)
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
