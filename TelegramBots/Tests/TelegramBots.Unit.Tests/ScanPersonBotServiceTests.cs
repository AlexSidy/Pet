using Microsoft.Extensions.Logging;

using Moq;

using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

using TelegramBots.BusinessLogic.Options;
using TelegramBots.BusinessLogic.Services;
using TelegramBots.Unit.Tests;

namespace TelegramBots.Unit.Tests;

[TestClass]
public class ScanPersonBotServiceTests: UnitTestsBase
{
	private readonly Mock<ITelegramBotClient> _mockBotClient;
	private readonly Mock<ILogger<ScanPersonBotService>> _mockLogger;
	private readonly Mock<ScanPersonWebApiService> _mockWebApiService;

	public ScanPersonBotServiceTests()
	{
		_mockBotClient = new Mock<ITelegramBotClient>();
		_mockLogger = new Mock<ILogger<ScanPersonBotService>>();
		var options = new ServicesOptions
		{
			ScanPersonWebApiServiceOptions = new ScanPersonWebApiServiceOptions
			{
				ServiceHostOptions = new ScanPerson.Models.Options.ServiceHostOptions
				{
					Host = "http://localhost",
					Port = 5000,
					ApiVersion = "v1",
					ControllerName = "ScanPerson"
				}
			}
		};

		_mockWebApiService = new Mock<ScanPersonWebApiService>(
			 new Mock<ILogger<ScanPersonWebApiService>>().Object,
			 new HttpClient(),
			 options
		);
	}

	[TestMethod]
	[DataRow("/start")]
	[DataRow("/help")]
	[DataRow("/time")]
	[DataRow("9998887766")]
	public async Task HandleUpdateAsync_StartCommand_SendsCorrectResponse(string message)
	{
		// Arrange
		var chatId = 123456;
		var update = new Update { Message = new Message { Chat = new Chat { Id = chatId }, Text = message } };
		var cts = TestContext.CancellationToken;
		_mockBotClient.Setup(x => x.SendRequest(It.IsAny<IRequest<It.IsAnyType>>(), cts));
		var cut = new ScanPersonBotService(
			_mockBotClient.Object,
			_mockLogger.Object,
			_mockWebApiService.Object
		);

		// Act
		await cut.HandleUpdateAsync(_mockBotClient.Object, update, TestContext.CancellationToken);

		// Assert
		_mockBotClient.Verify(x => x.SendRequest(It.IsAny<IRequest<It.IsAnyType>>(), cts),
			Times.Once,
			"SendMessage was not called with the correct response for /start."
		);
	}
}