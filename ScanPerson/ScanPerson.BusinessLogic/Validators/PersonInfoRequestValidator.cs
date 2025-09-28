using FluentValidation;

using ScanPerson.Models.Requests;

namespace ScanPerson.BusinessLogic.Validators
{
	/// <summary>
	/// Validator for <see cref="PersonInfoRequest"/>
	/// </summary>
	public class PersonInfoRequestValidator : AbstractValidator<PersonInfoRequest>
	{
		public PersonInfoRequestValidator()
		{
			RuleFor(x => x.PhoneNumber)
				.NotNull()
				.WithMessage("The phone number is required.")
				.Matches(@"^9[0-9]{9}$")
				.WithMessage("The phone number must start with a 9 and consist of 10 digits.");
		}
	}
}
