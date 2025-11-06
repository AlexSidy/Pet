using System.Text.Json;

using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ScanPerson.BusinessLogic.Services.Base;
using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Items;
using ScanPerson.Models.Options;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services.Decorators
{
	/// <summary>
	/// Service - decorator for getting information about a person's works by name result from other service (BrowserBotService).
	/// </summary>
	public class GetWorksInfoServiceDecorator : PersonInfoServiceDecoratorBase
	{
		protected const string DecoratedType = nameof(BrowserBotService);

		private readonly IChatAIService _chatAIService;

		public GetWorksInfoServiceDecorator(ILogger<GetWorksInfoServiceDecorator> logger,
		IHttpClientFactory httpClientFactory,
		ScanPersonSecrets secrets,
		ServicesOptions serviceOptions,
		IMapper mapper,
		IChatAIService chatAIService,
		[FromKeyedServices(DecoratedType)] IPersonInfoService service):
		base(logger, httpClientFactory, secrets, serviceOptions, mapper, service)
		{
			_chatAIService = chatAIService;
		}

		protected override async Task<ScanPersonResultResponse<PersonInfoRequest>> GetRequestFromSuccessResultAsync(
			ScanPersonResultResponse<PersonInfoItem> result,
			PersonInfoRequest request)
		{
			var identificationResult = await _chatAIService.GetExactIdentificationAsync(result.Result.Names);
			if (identificationResult.IsSuccess && identificationResult.Result.IsValid)
			{
				var mappedRequest = Mapper.Map<PersonInfoRequest>(identificationResult.Result);
				mappedRequest.PhoneNumber = request.PhoneNumber;
				return GetSuccess<PersonInfoRequest, ScanPersonResultResponse<PersonInfoRequest>>(mappedRequest);
			}

			return GetFail<PersonInfoRequest, ScanPersonResultResponse<PersonInfoRequest>>("Can't get exact identification.");
		}

		protected async override Task<ScanPersonResponseBase> GetAfterDecoratedInfoAsync(PersonInfoRequest request)
		{
			var namesPersonResult = new ScanPersonResultResponse<string[]>();
			//var namesPersonResult = await GetNamesByServiceMethodAsync<string[]>(request, "GetPossibleNamesByPhoneNumberAsync");
			Logger.LogInformation(Messages.OperationResult, JsonSerializer.Serialize(namesPersonResult));

			return namesPersonResult.IsSuccess
				? GetSuccess(new PersonInfoItem { Names = namesPersonResult.Result })
				: GetFail(namesPersonResult.Error);
		}

	}
}
