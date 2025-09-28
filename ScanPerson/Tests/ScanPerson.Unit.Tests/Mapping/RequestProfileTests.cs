using AutoMapper;

using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.BusinessLogic.MapperProfiles;
using ScanPerson.BusinessLogic.Services;
using ScanPerson.Models.Requests;

namespace ScanPerson.Unit.Tests.Mapping
{
	[TestClass]
	public sealed class RequestProfileTests
	{
		// Class under tests
		private readonly IMapper _cut;

		private readonly Mock<ILoggerFactory> _loggerFactory;

		public RequestProfileTests()
		{
			_loggerFactory = new Mock<ILoggerFactory>();
			_loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);
			var configuration = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile<RequestProfile>();
			}, _loggerFactory.Object);

			configuration.AssertConfigurationIsValid();
			_cut = configuration.CreateMapper();
		}

		[TestMethod]
		public void FromServiceRequestWithGeoService_ToPersonInfoRequest_DestinationWasModified()
		{
			// Arrange
			const string initialNumber = "9991112233";
			var source = new ServiceRequest(new PersonInfoRequest { PhoneNumber = initialNumber }, typeof(GeoService));

			// Act
			var destination = _cut.Map<PersonInfoRequest>(source);

			// Assert
			// Ожидаем префикс "7" + исходный номер
			Assert.AreNotEqual(initialNumber, destination.PhoneNumber);
			Assert.StartsWith("7", destination.PhoneNumber);
		}

		[TestMethod]
		public void FromServiceRequestWithBrowserBotService_ToPersonInfoRequest_DestinationWasNotModified()
		{
			// Arrange
			const string initialNumber = "9991112233";
			var source = new ServiceRequest(new PersonInfoRequest { PhoneNumber = initialNumber }, typeof(BrowserBotService));

			// Act
			var destination = _cut.Map<PersonInfoRequest>(source);

			// Assert
			// Ожидаем префикс "7" + исходный номер
			Assert.AreEqual(initialNumber, destination.PhoneNumber);
		}

		[TestMethod]
		public void FromServiceRequestWithNullInnerRequest_ToPersonInfoRequest_PhoneNumberReturnNull()
		{
			// Arrange
			var source = new ServiceRequest(null, typeof(BrowserBotService));

			// Act
			var destination = _cut.Map<PersonInfoRequest>(source);

			// Assert
			// Ожидаем префикс "7" + исходный номер
			Assert.IsNull(destination.PhoneNumber);
		}
	}
}
