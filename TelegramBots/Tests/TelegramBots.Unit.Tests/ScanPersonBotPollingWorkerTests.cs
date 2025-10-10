using Microsoft.Extensions.Logging;

using Moq;

using TelegramBots.BusinessLogic.Services.Interfaces;
using TelegramBots.BusinessLogic.Workers;

namespace TelegramBots.Unit.Tests
{
	[TestClass]
	public class ScanPersonBotPollingWorkerTests: UnitTestsBase
	{
		// Class under test.
		private readonly ScanPersonBotPollingWorker _cut;

		private readonly Mock<IBotService> _mockBotService;
		private readonly Mock<ILogger<ScanPersonBotPollingWorker>> _mockLogger;

		public ScanPersonBotPollingWorkerTests()
		{
			_mockBotService = new Mock<IBotService>();
			_mockLogger = new Mock<ILogger<ScanPersonBotPollingWorker>>();

			_cut = new ScanPersonBotPollingWorker(_mockBotService.Object, _mockLogger.Object);
		}

		[TestMethod]
		public async Task ExecuteAsync_OnStart_CallsStartReceivingAndLogsStart()
		{
			// Arrange
			var cts = TestContext.CancellationTokenSource;
			_mockBotService
				.Setup(x => x.StartReceiving(cts.Token));

			// Act
			await _cut.StartAsync(cts.Token);

			// Assert
			_mockBotService.Verify(
				x => x.StartReceiving(
					It.Is<CancellationToken>(t => t.CanBeCanceled)
				),
				Times.Once(),
				"ScanPersonBotService.StartReceiving must be called once with the correct token."
			);

			VerifyLog(_mockLogger, LogLevel.Information, Times.Once(), "Worker Service started.");
			VerifyLog(_mockLogger, LogLevel.Information, Times.Never(), "Worker Service stoped.");
		}

		[TestMethod]
		public async Task ExecuteAsync_WhenTokenIsCanceled_ExitsLoopGracefully()
		{
			// Arrange
			var cts = TestContext.CancellationTokenSource;

			_mockBotService
				.Setup(x => x.StartReceiving(It.IsAny<CancellationToken>()))
				.Callback(() => cts.Cancel());

			// Act
			await _cut.StartAsync(cts.Token);

			// Assert
			VerifyLog(_mockLogger, LogLevel.Debug, Times.Never(), "Worker is active...");
			VerifyLog(_mockLogger, LogLevel.Information, Times.Once(), "Worker Service stoped.");
		}

		[TestMethod]
		public async Task ExecuteAsync_WhileActive_CallsLogsActive()
		{
			// Arrange
			var cts = TestContext.CancellationTokenSource;
			_mockBotService
				.Setup(x => x.StartReceiving(It.IsAny<CancellationToken>()))
				.Verifiable();

			// Act
			var task = _cut.StartAsync(cts.Token);
			await Task.Delay(100);
			await _cut.StopAsync(cts.Token);

			// Assert
			VerifyLog(_mockLogger, LogLevel.Debug, Times.AtLeastOnce(), "Worker is active...");
			VerifyLog(_mockLogger, LogLevel.Information, Times.Once(), "Worker Service started.");
			VerifyLog(_mockLogger, LogLevel.Information, Times.Never(), "Worker Service stoped.");
		}

		[TestMethod]
		public async Task ExecuteAsync_TokenCancelled_CallsLogsStoped()
		{
			// Arrange
			var cts = TestContext.CancellationTokenSource;
			cts.Cancel();
			_mockBotService
				.Setup(x => x.StartReceiving(cts.Token));

			// Act
			await _cut.StartAsync(cts.Token);

			// Assert
			_mockBotService.Verify(
				x => x.StartReceiving(
					It.Is<CancellationToken>(t => t.CanBeCanceled)
				),
				Times.Once(),
				"ScanPersonBotService.StartReceiving must be called once with the correct token."
			);

			VerifyLog(_mockLogger, LogLevel.Information, Times.Once(), "Worker Service started.");
			VerifyLog(_mockLogger, LogLevel.Information, Times.Once(), "Worker Service stoped.");
		}
	}
}