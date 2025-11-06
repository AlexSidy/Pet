using System.Text.Json;

using LD.Sber.GigaChatSDK.Interfaces;
using LD.Sber.GigaChatSDK.Models;

using Microsoft.Extensions.Logging;

using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Helpers;
using ScanPerson.Common.Operations.Base;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Items;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services.Orchestration
{
	public class GigaChatService(ILogger<GigaChatService> logger, IGigaChat gigaChat) : OperationBase, IChatAIService
	{
		private const string Model = "GigaChat-Pro";

		public async Task<ScanPersonResultResponse<PersonIdentificationItem>> GetExactIdentificationAsync(string[] names)
		{
			try
			{
				var envBar = EnviromentHelper.GetVariableByName("GET_FIO_AI_REQUEST");
				var request = string.Format(EnviromentHelper.GetVariableByName("GET_FIO_AI_REQUEST"), JsonSerializer.Serialize(names));
				var query = GetMessageQuery(request);
				var response = await gigaChat.CompletionsAsync(query);
				if (response == null || response.Choices == null || response.Choices.FirstOrDefault()!.Message?.Content == null)
				{
					logger.LogInformation("AI return fails response");

					return GetFailResult();
				}

				var message = response.Choices.FirstOrDefault()!.Message?.Content!;
				//string cleanedJson = message.Substring(message.IndexOf('{'), message.IndexOf('}') - message.IndexOf('{') + 1);
				var cleanedJsonSpan = message.AsSpan(message.IndexOf('{'), message.IndexOf('}') - message.IndexOf('{') + 1);

				var result = JsonSerializer.Deserialize<PersonIdentificationItem>(cleanedJsonSpan);
				if (result != null && result.IsValid)
				{
					logger.LogInformation(Messages.OperationResult, JsonSerializer.Serialize(result));

					return GetSuccess<PersonIdentificationItem, ScanPersonResultResponse<PersonIdentificationItem>>(result);
				}

				logger.LogInformation("AI return not valid response");

				return GetFailResult();
			}
			catch (Exception ex)
			{
				logger.LogError(ex, Messages.OperationError, ex.Message);

				return GetFailResult();
			}
		}

		/// <summary>
		/// Get fail result.
		/// </summary>
		/// <returns>Fail result with standard client error message.</returns>
		private static ScanPersonResultResponse<PersonIdentificationItem> GetFailResult()
		{
			return GetFail<PersonIdentificationItem, ScanPersonResultResponse<PersonIdentificationItem>>(Messages.ClientOperationError);
		}

		/// <summary>
		/// Get message query.
		/// </summary>
		/// <param name="request">Formulated request to AI.</param>
		/// <returns>Object with message query.</returns>
		private static MessageQuery GetMessageQuery(string request)
		{
			var query = new MessageQuery();
			query.Model = Model;
			query.Temperature = 0.01f;
			query.Messages.Add(new MessageContent("system", "You are a helpful assistant designed to output JSON."));
			query.Messages.Add(new MessageContent("user", request));
			return query;
		}
	}
}
