using Microsoft.Extensions.DependencyInjection;

using TelegramBots.BusinessLogic.Services;

namespace TelegramBots.Integration.Tests;

[TestClass]
public class ScanPersonBotServiceTests : IntegrationTestsBase
{
	/// <summary>
	/// CUT.
	/// </summary>
	private readonly ScanPersonBotService _cut;

	public ScanPersonBotServiceTests()
	{
		_cut = Factory.Services.GetRequiredService<ScanPersonBotService>();
	}

	[TestMethod]
	public void StartReceiving_StartSuccess()
	{
		// Arrange
		bool exceptionThrown = false;
		var token = TestContext.CancellationTokenSource.Token;

		// Act
		try
		{
			_cut.StartReceiving(token);
		}
		catch (Exception ex)
		{
			Assert.Fail("Expected no {0} to be thrown", ex.GetType().Name);
		}

		// Assert
		Assert.IsFalse(exceptionThrown, "StartReceiving should not throw an exception.");
	}
}