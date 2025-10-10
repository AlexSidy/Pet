using ScanPerson.Common.Tests;

namespace ScanPerson.Unit.Tests
{
	[TestClass]
	public sealed class ItemsTests
	{
		[TestMethod]
		public void LocationItem_ToString_Success()
		{
			// Arrange
			var expectedResult = $"Country: Россия," +
				$"Region: Московская область," +
				$"Okrug: Центральный федеральный округ," +
				$"Capital: Москва," +
				$"Operator: Билайн," +
				$"City: Москва";
			var cut = CreationHelper.GetPersonResponse().Result.Location;

			// Act
			var result = cut.ToString();

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		public void PersonInfoItem_ToString_Success()
		{
			// Arrange
			var location = $"Country: Россия," +
				$"Region: Московская область," +
				$"Okrug: Центральный федеральный округ," +
				$"Capital: Москва," +
				$"Operator: Билайн," +
				$"City: Москва";
			var expectedResult = $"Names: Name," +
				$"Mail: Mail," +
				$"Location: " + location;
			var cut = CreationHelper.GetPersonResponse().Result;

			// Act
			var result = cut.ToString();

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(expectedResult, result);
		}
	}
}
