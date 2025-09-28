using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Tests;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;
using ScanPerson.WebApi.Controllers;

namespace ScanPerson.Unit.Tests
{
	[TestClass]
	public sealed class PersonControllerTests
	{
		// class under tests
		private readonly PersonInfoController _cut;

		private readonly Mock<ILogger<PersonInfoController>> _logger;
		private readonly Mock<IPersonInfoServicesAggregator> _servicesAggregator;

		public PersonControllerTests()
		{
			_logger = new Mock<ILogger<PersonInfoController>>();
			_servicesAggregator = new Mock<IPersonInfoServicesAggregator>();
			_cut = new PersonInfoController(_logger.Object, _servicesAggregator.Object);
		}

		[TestMethod]
		public void Ctor_DependiesAreNotNull_Success()
		{
			// Arrange

			// Act
			var cut = new PersonInfoController(_logger.Object, _servicesAggregator.Object);

			// Assert
			Assert.IsNotNull(cut);
		}

		[TestMethod]
		public async Task GetPersonAsync_PersonRequestIsCorrect_ReturnSeccessResult()
		{
			// Arrange
			var personRequest = new PersonInfoRequest { PhoneNumber = "12345" };
			var personResponses = CreationHelper.GetPersonResponse();
			var taskResponse = Task.FromResult<ScanPersonResponseBase>(personResponses);
			_servicesAggregator.Setup(x => x.GetScanPersonInfoAsync(It.IsAny<PersonInfoRequest>())).Returns(taskResponse);

			// Act
			var test = await _cut.GetScanPersonInfoAsync(personRequest);
			var response = (Microsoft.AspNetCore.Http.HttpResults.Ok<ScanPersonResponseBase>)await _cut.GetScanPersonInfoAsync(personRequest);

			// Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
			var result = (ScanPersonResultResponse<PersonInfoItem>)response.Value!;
			Assert.IsNotNull(result);
			Assert.IsTrue(result.IsSuccess);
			Assert.IsNull(result.Error);
			AssertHelper.AssertResult(personResponses, result);
		}

		[TestMethod]
		public async Task GetPersonAsync_PersonInfoIsCorrect_ReturnFailResult()
		{
			// Arrange
			var personRequest = new PersonInfoRequest { PhoneNumber = "12345" };
			var errorMessage = "error";
			var personResponses = new ScanPersonResultResponse<PersonInfoItem[]>([errorMessage]);
			var taskResponse = Task.FromResult<ScanPersonResponseBase>(personResponses);
			_servicesAggregator.Setup(x => x.GetScanPersonInfoAsync(It.IsAny<PersonInfoRequest>())).Returns(taskResponse);

			// Act
			var result = (Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>)await _cut.GetScanPersonInfoAsync(personRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
			Assert.IsNotNull(result.Value);
			Assert.AreEqual(errorMessage, result.Value);
		}
	}
}
