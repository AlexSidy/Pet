using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TelegramBots.BusinessLogic.Services;

namespace TelegramBots.BusinessLogic.Workers
{
	public class ScanPersonBotPollingWorker : BackgroundService
	{
		private readonly ScanPersonBotService _botService;
		private readonly ILogger<ScanPersonBotPollingWorker> _logger;

		// Worker получает экземпляр сервиса через DI
		public ScanPersonBotPollingWorker(ScanPersonBotService botService, ILogger<ScanPersonBotPollingWorker> logger)
		{
			_botService = botService;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Worker Service started.");

			// Запускаем Long Polling в фоновом режиме.
			// CancellationToken автоматически остановит получение при остановке Worker'а
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