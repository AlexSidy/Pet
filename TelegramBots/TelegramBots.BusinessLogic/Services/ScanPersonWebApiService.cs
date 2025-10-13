using System.Text.Json;
using System.Text.RegularExpressions;

using AutoMapper;

using Microsoft.Extensions.Logging;

using ScanPerson.Common.GrpcServices;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Items;
using ScanPerson.Models.Responses;

using TelegramBots.BusinessLogic.Services.Interfaces;

namespace TelegramBots.BusinessLogic.Services
{
	/// <summary>
	/// Web api service.
	/// </summary>
	public class ScanPersonWebApiService(
		ILogger<ScanPersonWebApiService> logger,
			GrpcPersonInfo.GrpcPersonInfoClient grpcClient,
			IMapper mapper) : IApiService
	{
		public const string ValidationError = "Номер телефона должен начинаться с 9 и состоять из 10 цифр.";
		private const string PhoneNumberRule = "^9[0-9]{9}$";

		private static readonly Regex PhoneNumberRegex = new(
			PhoneNumberRule,
			RegexOptions.Compiled,
			TimeSpan.FromSeconds(1)
		);

		public async Task<string?> GetDataAsync(string? phoneNumber, CancellationToken cancellationToken)
		{
			try
			{
				logger.LogInformation(Messages.StartedMethodWithParameters, nameof(GetDataAsync), phoneNumber);
				if (!PhoneNumberRegex.IsMatch(phoneNumber!))
				{
					return ValidationError;
				}
				var request = new GrpcPersonInfoRequest { PhoneNumber = phoneNumber };
				var response = await grpcClient.GetGrpcScanPersonInfoAsync(request, cancellationToken: cancellationToken);
				if (response == null || !response.IsSuccess)
				{
					logger.LogInformation(Messages.OperationError, response?.Error);
					return Messages.ClientOperationError;
				}

				logger.LogInformation(Messages.OperationResult, JsonSerializer.Serialize(response));
				var mapped = mapper.Map<ScanPersonResultResponse<PersonInfoItem>>(response);

				return mapped.Result.ToString();
			}
			catch (Exception ex)
			{
				logger.LogError(ex, Messages.OperationError, ex.Message);
				return Messages.ClientOperationError;
			}
		}
	}
}
