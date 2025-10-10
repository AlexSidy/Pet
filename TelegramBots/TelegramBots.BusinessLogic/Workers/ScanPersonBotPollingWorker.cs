using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TelegramBots.BusinessLogic.Services.Interfaces;

namespace TelegramBots.BusinessLogic.Workers
{
	public class ScanPersonBotPollingWorker : BackgroundService
	{
		private readonly IBotService _botService;
		private readonly ILogger<ScanPersonBotPollingWorker> _logger;

		public ScanPersonBotPollingWorker(IBotService botService, ILogger<ScanPersonBotPollingWorker> logger)
		{
			_botService = botService;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Worker Service started.");

			_botService.StartReceiving(stoppingToken);

			// Этот цикл просто удерживает Worker активным, пока хост не будет остановлен
			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogDebug("Worker is active...");
				await Task.Delay(5000, stoppingToken);
			}
			_logger.LogInformation("Worker Service stoped.");
		}
	}
}