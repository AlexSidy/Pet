using ScanPerson.Models.Items;
using ScanPerson.Models.Responses;

namespace ScanPerson.Common.Tests
{
	[TestClass]
	public static class AssertHelper
	{
		public static void AssertResult(ScanPersonResultResponse<PersonInfoItem> personResponse, ScanPersonResultResponse<PersonInfoItem> result)
		{
			Assert.IsTrue(result.IsSuccess);
			Assert.IsNull(result.Error);
			Assert.IsNotNull(result.Result);
			Assert.AreEqual(personResponse!.Result!.Name, result.Result.Name);
			Assert.AreEqual(personResponse!.Result!.Mail, result.Result.Mail);
			Assert.AreEqual(personResponse!.Result!.Id, result.Result.Id);
			AssertLocationResult(personResponse, result);
		}

		public static void AssertLocationResult(ScanPersonResultResponse<PersonInfoItem> personResponse, ScanPersonResultResponse<PersonInfoItem> result)
		{
			Assert.IsTrue(result.IsSuccess);
			Assert.IsNull(result.Error);
			Assert.IsNotNull(result.Result);
			Assert.IsNotNull(result.Result.Location);
			Assert.AreEqual(personResponse!.Result!.Location.CurrentRegion, result.Result.Location.CurrentRegion);
			Assert.AreEqual(personResponse!.Result!.Location.OperatorName, result.Result.Location.OperatorName);
			Assert.AreEqual(personResponse!.Result!.Location.CountryName, result.Result.Location.CountryName);
			Assert.AreEqual(personResponse!.Result!.Location.OperatorCity, result.Result.Location.OperatorCity);
			Assert.AreEqual(personResponse!.Result!.Location.RegistrationCapital, result.Result.Location.RegistrationCapital);
			Assert.AreEqual(personResponse!.Result!.Location.RegistrationOkrug, result.Result.Location.RegistrationOkrug);
		}
	}
}
