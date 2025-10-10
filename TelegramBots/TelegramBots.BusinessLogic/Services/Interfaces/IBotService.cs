namespace TelegramBots.BusinessLogic.Services.Interfaces
{
	/// <summary>
	/// Telegram bot service.
	/// </summary>
	public interface IBotService
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cancellationToken">Cancelation token.</param>
		void StartReceiving(CancellationToken cancellationToken);
	}
}
