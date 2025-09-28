using System.Text.Json;

using AutoMapper;

using Microsoft.Extensions.Logging;

using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Operations.Base;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Items;
using ScanPerson.Models.Options;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services
{
	public abstract class PersonInfoServiceBase : OperationBase, IPersonInfoService
	{
		/// <summary>
		/// Logger.
		/// </summary>
		protected ILogger<PersonInfoServiceBase> Logger { get; }

		/// <summary>
		/// HTTP client.
		/// </summary>
		protected HttpClient HttpClient { get; }

		/// <summary>
		/// App secrets.
		/// </summary>
		protected ScanPersonSecrets Secrets { get; }

		/// <summary>
		/// Services options.
		/// </summary>
		protected ServicesOptions ServicesOptions { get; }

		/// <summary>
		/// Mapper service.
		/// </summary>
		protected IMapper Mapper { get; }

		protected PersonInfoServiceBase(
			ILogger<PersonInfoServiceBase> logger,
			IHttpClientFactory httpClientFactory,
			ScanPersonSecrets secrets,
			ServicesOptions servicesOptions,
			IMapper mapper)
		{
			Logger = logger;
			HttpClient = httpClientFactory.CreateClient(nameof(PersonInfoServiceBase));
			Secrets = secrets;
			ServicesOptions = servicesOptions;
			Mapper = mapper;
		}

		public async Task<ScanPersonResponseBase> GetInfoAsync(PersonInfoRequest request)
		{
			try
			{
				var preparedRequest = GetPreparedRequest(request);
				Logger.LogInformation(Messages.StartedMethodWithParameters, nameof(GetInfoAsync), JsonSerializer.Serialize(preparedRequest));
				var result = await GetAnyPersonInfoAsync(preparedRequest);
				Logger.LogInformation(Messages.OperationResult, JsonSerializer.Serialize(result));

				return result;
			}
			catch (Exception ex)
			{
				LogError(ex);

				return GetFail<PersonInfoItem>(Messages.ClientOperationError);
			}
		}

		/// <summary>
		/// Method for logging errors.
		/// </summary>
		/// <param name="ex">Exception.</param>
		protected void LogError(Exception ex)
		{
			Logger.LogError(ex, Messages.OperationError, GetType().Name);
		}

		/// <summary>
		/// Method for checking if the service can be used.
		/// </summary>
		/// <returns>True if the service can be used.</returns>
		public virtual bool CanAccept()
		{
			return !ServicesOptions.UnUsingServices.Contains(GetType().Name);
		}

		/// <summary>
		/// Prepare a request contract for processing.
		/// </summary>
		/// <param name="request">The request containing the input data.</param>
		/// <returns>The result containing the specific service information obtained.</returns>
		private PersonInfoRequest GetPreparedRequest(PersonInfoRequest request)
		{
			return Mapper.Map<PersonInfoRequest>(new ServiceRequest(request, GetType()));
		}

		/// <summary>
		/// Method for obtaining service-specific information.
		/// </summary>
		/// <param name="request">The request containing the input data.</param>
		/// <returns>The result containing the specific service information obtained.</returns>
		protected abstract Task<ScanPersonResponseBase> GetAnyPersonInfoAsync(PersonInfoRequest request);
	}
}
