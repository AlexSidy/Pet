using Microsoft.Extensions.Logging;

using Moq;

using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

using TelegramBots.BusinessLogic.Services;
using TelegramBots.BusinessLogic.Services.Interfaces;

namespace TelegramBots.Unit.Tests
{
	[TestClass]
	public class ScanPersonBotServiceTests : UnitTestsBase
	{
		private readonly Mock<ITelegramBotClient> _mockBotClient;
		private readonly Mock<ILogger<ScanPersonBotService>> _mockLogger;
		private readonly Mock<IApiService> _mockWebApiService;

		public ScanPersonBotServiceTests()
		{
			_mockBotClient = new Mock<ITelegramBotClient>();
			_mockLogger = new Mock<ILogger<ScanPersonBotService>>();
			_mockWebApiService = new Mock<IApiService>();
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
			var token = TestContext.CancellationToken;
			_mockWebApiService.Setup(x => x.GetDataAsync(message, token)).ReturnsAsync("response");

			// Act
			await cut.HandleUpdateAsync(_mockBotClient.Object, update, TestContext.CancellationToken);

			// Assert
			_mockBotClient.Verify(x => x.SendRequest(It.IsAny<IRequest<It.IsAnyType>>(), cts),
				Times.Once,
				"SendMessage was not called with the correct response."
			);
		}
	}
}