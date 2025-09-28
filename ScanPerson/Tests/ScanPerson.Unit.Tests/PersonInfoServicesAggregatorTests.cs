using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.BusinessLogic.Services;
using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Resources;
using ScanPerson.Common.Tests;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.Unit.Tests
{
	[TestClass]
	public sealed class PersonInfoServicesAggregatorTests
	{
		// class under tests
		private readonly PersonInfoServicesAggregator _cut;

		private readonly Mock<ILogger<PersonInfoServicesAggregator>> _logger;
		private readonly Mock<IPersonInfoService> _personInfoService;

		public PersonInfoServicesAggregatorTests()
		{
			_logger = new Mock<ILogger<PersonInfoServicesAggregator>>();
			_personInfoService = new Mock<IPersonInfoService>();
			_personInfoService.Setup(x => x.CanAccept()).Returns(true);

			_cut = new PersonInfoServicesAggregator(_logger.Object, [.. new IPersonInfoService[] { _personInfoService.Object }]);
		}

		[TestMethod]
		public void Ctor_DependiesAreNotNull_Success()
		{
			// Arrange

			// Act
			var cut = new PersonInfoServicesAggregator(_logger.Object, [.. new IPersonInfoService[] { _personInfoService.Object }]);

			// Assert
			Assert.IsNotNull(cut);
		}

		[TestMethod]
		public async Task TaskGetScanPersonInfoAsync_PersonRequestIsCorrect_ReturnSuccessResult()
		{
			// Arrange
			var personRequest = new PersonInfoRequest();
			var expectedResult = CreationHelper.GetPersonResponse();
			var taskResponse = Task.FromResult<ScanPersonResponseBase>(expectedResult);
			_personInfoService.Setup(x => x.GetInfoAsync(It.IsAny<PersonInfoRequest>())).Returns(taskResponse);

			// Act
			var response = await _cut.GetScanPersonInfoAsync(personRequest);
			var result = (ScanPersonResultResponse<PersonInfoItem>)response;

			// Assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Result);
			Assert.IsTrue(result.IsSuccess);
			AssertHelper.AssertPersonInfo(expectedResult.Result, result.Result);
		}

		[TestMethod]
		public async Task GetInfoAsync_PersonInfoServiceReturnFailedResult_ReturnFailResult()
		{
			// Arrange
			var personRequest = new PersonInfoRequest();
			var errorMessage = "error";
			var errorPersonResponses = new ScanPersonResultResponse<PersonInfoItem>([errorMessage]);
			var erororTaskResponse = Task.FromResult<ScanPersonResponseBase>(errorPersonResponses);
			_personInfoService.Setup(x => x.GetInfoAsync(It.IsAny<PersonInfoRequest>())).Returns(erororTaskResponse);
			var cut = new PersonInfoServicesAggregator(_logger.Object, [.. new IPersonInfoService[] { _personInfoService.Object }]);

			// Act
			var result = await cut.GetScanPersonInfoAsync(personRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(errorMessage, result.Error);
		}

		[TestMethod]
		public async Task GetInfoAsync_PersonInfoServiceThrowsException_ReturnFailResult()
		{
			// Arrange
			var personRequest = new PersonInfoRequest();
			_personInfoService.Setup(x => x.GetInfoAsync(It.IsAny<PersonInfoRequest>())).Throws(new Exception());
			var cut = new PersonInfoServicesAggregator(_logger.Object, [.. new IPersonInfoService[] { _personInfoService.Object }]);

			// Act
			var result = await cut.GetScanPersonInfoAsync(personRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(Messages.ClientOperationError, result.Error);
		}

		[TestMethod]
		public async Task GetInfoAsync_PersonInfoServiceIsEmpty_ReturnBaseFailResult()
		{
			// Arrange
			var personRequest = new PersonInfoRequest();
			_personInfoService.Setup(x => x.GetInfoAsync(It.IsAny<PersonInfoRequest>())).Throws(new Exception());
			var cut = new PersonInfoServicesAggregator(_logger.Object, []);

			// Act
			var result = await cut.GetScanPersonInfoAsync(personRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
			Assert.IsNotEmpty(result.Error);
		}

		[TestMethod]
		public async Task TaskGetScanPersonInfoAsync_PersonRequestIsCorrectAndWorkTwoService_ReturnSuccessResult()
		{
			// Arrange
			var personRequest = new PersonInfoRequest();
			var expectedResult1 = CreationHelper.GetPersonResponse();
			expectedResult1.Result.Location = null;
			var taskResponse1 = Task.FromResult<ScanPersonResponseBase>(expectedResult1);
			var expectedResult2 = CreationHelper.GetPersonResponse();
			var taskResponse2 = Task.FromResult<ScanPersonResponseBase>(expectedResult2);
			var geoService = new Mock<IPersonInfoService>();
			_personInfoService.Setup(x => x.GetInfoAsync(It.IsAny<PersonInfoRequest>())).Returns(taskResponse1);
			geoService.Setup(x => x.GetInfoAsync(It.IsAny<PersonInfoRequest>())).Returns(taskResponse2);
			geoService.Setup(x => x.CanAccept()).Returns(true);

			var cut = new PersonInfoServicesAggregator(_logger.Object, [.. new IPersonInfoService[] { _personInfoService.Object, geoService.Object }]);

			// Act
			var response = await cut.GetScanPersonInfoAsync(personRequest);
			var result = (ScanPersonResultResponse<PersonInfoItem>)response;

			// Assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Result);
			Assert.IsTrue(result.IsSuccess);
			AssertHelper.AssertPersonInfo(expectedResult2.Result, result.Result);
		}

		[TestMethod]
		public async Task TaskGetScanPersonInfoAsync_ServiceReturnFailResult_ReturnFailResult()
		{
			// Arrange
			var personRequest = new PersonInfoRequest();
			var expectedResult = new ScanPersonResponseBase("error");
			var taskResponse = Task.FromResult(expectedResult);
			_personInfoService.Setup(x => x.GetInfoAsync(It.IsAny<PersonInfoRequest>())).Returns(taskResponse);

			var cut = new PersonInfoServicesAggregator(_logger.Object, [.. new IPersonInfoService[] { _personInfoService.Object }]);

			// Act
			var response = await _cut.GetScanPersonInfoAsync(personRequest);
			var result = response;

			// Assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result.IsSuccess);
		}
	}
}
