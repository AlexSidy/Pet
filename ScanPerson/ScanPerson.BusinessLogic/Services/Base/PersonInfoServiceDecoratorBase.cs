using System.Text.Json;

using AutoMapper;

using Microsoft.Extensions.Logging;

using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Items;
using ScanPerson.Models.Options;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services.Base
{
	public abstract class PersonInfoServiceDecoratorBase : PersonInfoServiceBase
	{
		private IPersonInfoService DecoratedService { get; }

		protected PersonInfoServiceDecoratorBase(
		ILogger<PersonInfoServiceDecoratorBase> logger,
		IHttpClientFactory httpClientFactory,
		ScanPersonSecrets secrets,
		ServicesOptions serviceOptions,
		IMapper mapper,
		IPersonInfoService service) : base(logger, httpClientFactory, secrets, serviceOptions, mapper)
		{
			DecoratedService = service;
		}

		protected async override Task<ScanPersonResponseBase> GetAnyPersonInfoAsync(PersonInfoRequest request)
		{
			try
			{
				await DecoratedService.Semaphore.WaitAsync();
				var decoratedResult = await DecoratedService.GetInfoAsync(request);

				return await AfterGetInfoAsync(decoratedResult, request);

			}
			finally
			{
				DecoratedService.Semaphore.Release();
			}
		}

		/// <summary>
		/// Common method for processing the result of the decorated service.
		/// </summary>
		/// <param name="decoratedTask">Task of the decorated service.</param>
		/// <param name="request">Request.</param>
		/// <returns>Decorated service result.</returns>
		private async Task<ScanPersonResponseBase> AfterGetInfoAsync(ScanPersonResponseBase decoratedResult, PersonInfoRequest request)
		{
			if (decoratedResult == null || !decoratedResult.IsSuccess)
			{
				Logger.LogInformation($"{DecoratedService} returned failed result.");
				return GetFail(decoratedResult == null ? Messages.ClientOperationError : decoratedResult.Error);
			}
			var successResult = (ScanPersonResultResponse<PersonInfoItem>)decoratedResult;
			var preparedRequestResult = await GetRequestFromSuccessResultAsync(successResult, request);
			if (preparedRequestResult.IsSuccess)
			{
				Logger.LogInformation(Messages.OperationResult, JsonSerializer.Serialize(request));
				return await GetAfterDecoratedInfoAsync(preparedRequestResult.Result);
			}

			Logger.LogInformation(Messages.OperationResult, preparedRequestResult.Error);

			return GetFail(preparedRequestResult.Error);
		}

		/// <summary>
		/// Get request from success result.
		/// </summary>
		/// <param name="result">Success result from decorated service.</param>
		/// <param name="request">Common request.</param>
		/// <returns>New request.</returns>
		protected virtual async Task<ScanPersonResultResponse<PersonInfoRequest>> GetRequestFromSuccessResultAsync(
			ScanPersonResultResponse<PersonInfoItem> result,
			PersonInfoRequest request)
		{
			return await Task.FromResult(GetSuccess<PersonInfoRequest, ScanPersonResultResponse<PersonInfoRequest>>(request));
		}

		/// <summary>
		/// Concrete method for processing the result of the decorated service.
		/// </summary>
		/// <param name="request">Prepared request from decorated service.</param>
		/// <returns>Person info response.</returns>
		protected abstract Task<ScanPersonResponseBase> GetAfterDecoratedInfoAsync(PersonInfoRequest request);
	}
}
