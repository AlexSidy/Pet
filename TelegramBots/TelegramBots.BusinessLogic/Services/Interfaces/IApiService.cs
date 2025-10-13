namespace TelegramBots.BusinessLogic.Services.Interfaces
{
	/// <summary>
	/// Api service.
	/// </summary>
	public interface IApiService
	{
		/// <summary>
		/// Get data.
		/// </summary>
		/// <param name="phoneNumber">Phone number.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Response as string.</returns>
		Task<string?> GetDataAsync(string? phoneNumber, CancellationToken cancellationToken);
	}
}
