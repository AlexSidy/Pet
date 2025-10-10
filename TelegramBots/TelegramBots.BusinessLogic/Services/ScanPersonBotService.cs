using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using TelegramBots.BusinessLogic.Services.Interfaces;

namespace TelegramBots.BusinessLogic.Services
{
	/// <summary>
	/// Telegram bot service for scan person.
	/// </summary>
	public class ScanPersonBotService: IBotService
	{
		private readonly ITelegramBotClient _botClient;
		private readonly ILogger<ScanPersonBotService> _logger;
		private readonly ScanPersonWebApiService _scanPersonWebApiService;

		public ScanPersonBotService(ITelegramBotClient botClient, ILogger<ScanPersonBotService> logger, ScanPersonWebApiService scanPersonWebApiService)
		{
			_botClient = botClient;
			_logger = logger;
			_scanPersonWebApiService = scanPersonWebApiService;
		}

		/// <summary>
		/// Handler for updates.
		/// </summary>
		/// <param name="botClient">Telegram bot client.<</param>
		/// <param name="update"></param>
		/// <param name="cancellationToken"></param>
		public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			if (update.Message is not { } message || message.Text is not { } messageText)
				return;

			var chatId = message.Chat.Id;
			_logger.LogInformation("Message received '{Message}' in chat {ChatId}.",
				new { Message = messageText }, new  { ChatId = chatId });

			string responseText = messageText.ToLower() switch
			{
				"/start" => "👁 Привет! Я Scan Person Service бот! Введите /help для просмотра моих возможностей.",
				"/help" => @"🥪 Введите одну из следующих команд:
										🕰 /time чтобы узнать текущее время на сервере;
										🐵 /help для получения справки;
										💾 введите номер телефона в формате 9991112233 (без + 7, без 8, без пробелов и -),
										чтобы узнать информацию о пользователе.",
				"/time" => $"Текущее время на сервере: {DateTime.Now:HH:mm:ss}",
				_ => await _scanPersonWebApiService.GetDataAsync(messageText, cancellationToken) ?? "👻 Пользователь не найден.",
			};

			await botClient.SendMessage(
				chatId: chatId,
				text: responseText,
				cancellationToken: cancellationToken);
		}

		/// <summary>
		/// Error handler.
		/// </summary>
		/// <param name="botClient">Telegram bot client.</param>
		/// <param name="exception">Exception</param>
		/// <param name="cancellationToken">Cancelation token.</param>
		/// <returns>Completed task.</returns>
		private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
		{
			_logger.LogError(exception, "Error occurred during Long Polling.");
			return Task.CompletedTask;
		}

		public void StartReceiving(CancellationToken cancellationToken)
		{
			var receiverOptions = new ReceiverOptions
			{
				AllowedUpdates = Array.Empty<UpdateType>() // Get all update types
			};

			_botClient.StartReceiving(
				updateHandler: HandleUpdateAsync,
				errorHandler: HandlePollingErrorAsync,
				receiverOptions: receiverOptions,
				cancellationToken: cancellationToken
			);
			_logger.LogInformation("The bot has started receiving messages.");
		}
	}
}