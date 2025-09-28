using FluentValidation.TestHelper;

using ScanPerson.BusinessLogic.Validators;
using ScanPerson.Models.Requests;

namespace ScanPerson.Unit.Tests.Validators
{
	[TestClass]
	public class PersonInfoRequestValidatorTests
	{
		/// <summary>
		/// CUT
		/// </summary>
		private readonly PersonInfoRequestValidator _validator;

		public PersonInfoRequestValidatorTests()
		{
			_validator = new PersonInfoRequestValidator();
		}

		[TestMethod]
		public void PhoneNumberIsNull_ShouldHaveValidationError()
		{
			// Arrange
			var request = new PersonInfoRequest { PhoneNumber = null };

			// Act
			var result = _validator.TestValidate(request);

			// Assert
			result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
				  .WithErrorMessage("The phone number is required.");
		}

		[TestMethod]
		[DataRow("9123456789")] // Корректный номер (10 цифр, начинается с 9)
		public void PhoneNumberIsValid_ShouldNotHaveValidationError(string validNumber)
		{
			// Arrange
			var request = new PersonInfoRequest { PhoneNumber = validNumber };

			// Act
			var result = _validator.TestValidate(request);

			// Assert
			result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
		}

		[TestMethod]
		[DataRow("8123456789")] // Начинается не с 9
		[DataRow("912345678")] // Меньше 10 цифр (9)
		[DataRow("91234567890")] // Больше 10 цифр (11)
		[DataRow("91234a6789")] // Содержит буквы
		[DataRow("1234567890")]// Начинается не с 9
		public void Should_Have_Error_When_PhoneNumber_Is_Invalid(string invalidNumber)
		{
			// Arrange
			var request = new PersonInfoRequest { PhoneNumber = invalidNumber };

			// Act
			var result = _validator.TestValidate(request);

			// Assert
			result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
				  .WithErrorMessage("The phone number must start with a 9 and consist of 10 digits.");
		}
	}
}