using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.BusinessLogic.Services;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;
using ScanPerson.WebApi.Controllers;

namespace ScanPerson.WebApi.Unit.Tests
{
	[TestClass]
	public sealed class PersonControllerTests
	{
		// class under tests
		private readonly PersonController _cut;

		private readonly Mock<ILogger<PersonController>> _logger;
		private readonly Mock<IPersonService> _personService;

		public PersonControllerTests()
		{
			_logger = new Mock<ILogger<PersonController>>();
			_personService = new Mock<IPersonService>();
			_cut = new PersonController(_logger.Object, _personService.Object);
		}

		[TestMethod]
		public void Ctor_DependiesAreNotNull_Success()
		{
			// Arrange

			// Act
			var cut = new PersonController(_logger.Object, _personService.Object);

			// Assert
			Assert.IsNotNull(cut);
		}

		[TestMethod]
		public async Task GetPersonAsync_PersonRequestIsCorrect_ReturnFailResult()
		{
			// Arrange
			var personRequest = new PersonRequest
			{
				Email = "email",
				Login = "login",
				Password = "password"
			};
			var errorMessage = "error";
			var personResponse = new ScanPersonResultResponse<PersonItem?>([errorMessage]);
			_personService.Setup(x => x.Find(It.IsAny<PersonRequest>())).Returns(personResponse);

			// Act
			var result = (Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>)await _cut.GetPersonAsync(personRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
			Assert.IsNotNull(result.Value);
			Assert.AreEqual(errorMessage, result.Value);
		}

		[TestMethod]
		public async Task GetPersonAsync_PersonRequestIsCorrect_ReturnSeccessResult()
		{
			// Arrange
			var personRequest = new PersonRequest
			{
				Email = "email",
				Login = "login",
				Password = "password"
			};
			var personResponse = new ScanPersonResultResponse<PersonItem>(new PersonItem(1, "Test", "Mail"));
			_personService.Setup(x => x.Find(It.IsAny<PersonRequest>())).Returns(personResponse!);

			// Act
			var result = (Microsoft.AspNetCore.Http.HttpResults.Ok<ScanPersonResultResponse<PersonItem?>>)await _cut.GetPersonAsync(personRequest);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
			Assert.IsNotNull(result.Value);
			Assert.IsTrue(result.Value.IsSuccess);
			Assert.IsNull(result.Value.Error);
			Assert.IsNotNull(result.Value.Result);
			Assert.AreEqual(personResponse.Result.Id, result.Value.Result.Id);
			Assert.AreEqual(personResponse.Result.Name, result.Value.Result.Name);
			Assert.AreEqual(personResponse.Result.Mail, result.Value.Result.Mail);
		}
	}
}
