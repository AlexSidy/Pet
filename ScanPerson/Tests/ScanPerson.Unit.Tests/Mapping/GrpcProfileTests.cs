using AutoMapper;

using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.Common.GrpcServices;
using ScanPerson.Common.MapperProfiles;
using ScanPerson.Common.Tests;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.Unit.Tests.Mapping
{
	[TestClass]
	public sealed class GrpcProfileTests
	{
		// Class under tests
		private readonly IMapper _cut;

		private readonly Mock<ILoggerFactory> _loggerFactory;

		public GrpcProfileTests()
		{
			_loggerFactory = new Mock<ILoggerFactory>();
			_loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);
			var configuration = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile<GrpcProfile>();
			}, _loggerFactory.Object);

			configuration.AssertConfigurationIsValid();
			_cut = configuration.CreateMapper();
		}

		[TestMethod]
		public void FromGrpcPersonInfoRequest_ToPersonInfoRequest_Correct()
		{
			// Arrange
			var source = new GrpcPersonInfoRequest
			{
				PhoneNumber = "test"
			};

			// Act
			var destination = _cut.Map<PersonInfoRequest>(source);

			// Assert
			Assert.IsNotNull(destination);
			Assert.AreEqual(source.PhoneNumber, destination.PhoneNumber);
		}

		[TestMethod]
		public void FromScanPersonResponseBase_ToGrpcScanPersonResponse_Correct()
		{
			// Arrange
			var source = new ScanPersonResponseBase("error");

			// Act
			var destination = _cut.Map<GrpcScanPersonResponse>(source);

			// Assert
			Assert.IsNotNull(destination);
			Assert.AreEqual(source.Error, destination.Error);
			Assert.AreEqual(source.IsSuccess, destination.IsSuccess);
		}

		[TestMethod]
		public void FromScanPersonResultResponse_ToGrpcScanPersonResponse_Correct()
		{
			// Arrange
			var source = CreationHelper.GetPersonResponse();

			// Act
			var destination = _cut.Map<GrpcScanPersonResponse>(source);

			// Assert
			Assert.IsNotNull(destination);
			Assert.IsEmpty(destination.Error);
			Assert.AreEqual(source.IsSuccess, destination.IsSuccess);
			Assert.AreEqual(source.Result.Names[0], destination.Result.Names[0]);
			Assert.AreEqual(source.Result.Mail, destination.Result.Mail);
			Assert.AreEqual(source.Result.Id, destination.Result.Id);
			Assert.AreEqual(source.Result.Location.CountryName, destination.Result.Location.CountryName);
			Assert.AreEqual(source.Result.Location.RegistrationOkrug, destination.Result.Location.RegistrationOkrug);
			Assert.AreEqual(source.Result.Location.CurrentRegion, destination.Result.Location.CurrentRegion);
			Assert.AreEqual(source.Result.Location.RegistrationCapital, destination.Result.Location.RegistrationCapital);
			Assert.AreEqual(source.Result.Location.OperatorName, destination.Result.Location.OperatorName);
			Assert.AreEqual(source.Result.Location.OperatorCity, destination.Result.Location.OperatorCity);
		}

		[TestMethod]
		public void FromGrpcScanPersonResponse_ToScanPersonResponseBase_Correct()
		{
			// Arrange
			var source = new GrpcScanPersonResponse
			{
				IsSuccess = true,
				Error = "error"
			};

			// Act
			var destination = _cut.Map<ScanPersonResponseBase>(source);

			// Assert
			Assert.IsNotNull(destination);
			Assert.AreEqual(source.Error, destination.Error);
			Assert.AreEqual(source.IsSuccess, destination.IsSuccess);
		}

		[TestMethod]
		public void FromGrpcScanPersonResponse_ToScanPersonResultResponse_Correct()
		{
			// Arrange
			var source = _cut.Map<GrpcScanPersonResponse>(CreationHelper.GetPersonResponse());

			// Act
			var destination = _cut.Map<ScanPersonResultResponse<PersonInfoItem>>(source);

			// Assert
			Assert.IsNotNull(destination);
			Assert.IsNull(destination.Error);
			Assert.AreEqual(source.IsSuccess, destination.IsSuccess);
			Assert.AreEqual(source.Result.Names[0], destination.Result.Names[0]);
			Assert.AreEqual(source.Result.Mail, destination.Result.Mail);
			Assert.AreEqual(source.Result.Id, destination.Result.Id);
			Assert.AreEqual(source.Result.Location.CountryName, destination.Result.Location.CountryName);
			Assert.AreEqual(source.Result.Location.RegistrationOkrug, destination.Result.Location.RegistrationOkrug);
			Assert.AreEqual(source.Result.Location.CurrentRegion, destination.Result.Location.CurrentRegion);
			Assert.AreEqual(source.Result.Location.RegistrationCapital, destination.Result.Location.RegistrationCapital);
			Assert.AreEqual(source.Result.Location.OperatorName, destination.Result.Location.OperatorName);
			Assert.AreEqual(source.Result.Location.OperatorCity, destination.Result.Location.OperatorCity);
		}
	}
}
