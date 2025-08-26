using AutoMapper;

using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.BusinessLogic.MapperProfiles;
using ScanPerson.Models.Items;

namespace ScanPerson.Unit.Tests.Mapping
{
	[TestClass]
	public sealed class DisplayProfileTests
	{
		// Class under tests
		private readonly IMapper _cut;

		private readonly Mock<ILoggerFactory> _loggerFactory;

		public DisplayProfileTests()
		{
			_loggerFactory = new Mock<ILoggerFactory>();
			_loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);
			var configuration = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile<DisplayProfile>();
			}, _loggerFactory.Object);

			configuration.AssertConfigurationIsValid();
			_cut = configuration.CreateMapper();
		}

		[TestMethod]
		public void FromLocationDeserialized_ToLocationItem_Correct()
		{
			// Arrange
			var source = new LocationDeserialized
			{
				Country = new Country { Name = "Россия" },
				Capital = new Capital { Name = "Москва" },
				Region = new Region { Name = "Московская область" },
				Okrug = "Московская область",
				Operator = new Operator { Name = "Московская область", OperBrand = "Московская область" }
			};

			// Act
			var destination = _cut.Map<LocationItem>(source);

			// Assert
			Assert.IsNotNull(destination);
			Assert.AreEqual(source.Country.Name, destination.CountryName);
			Assert.AreEqual(source.Region.Name, destination.CurrentRegion);
			Assert.AreEqual(source.Okrug, destination.RegistrationOkrug);
			Assert.AreEqual(source.Capital.Name, destination.RegistrationCapital);
			Assert.AreEqual(source.Operator.Name, destination.OperatorCity);
			Assert.AreEqual(source.Operator.Name, destination.OperatorCity);
		}
	}
}
