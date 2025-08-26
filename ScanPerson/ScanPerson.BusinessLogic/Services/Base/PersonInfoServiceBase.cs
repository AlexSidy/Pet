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
		protected ILogger<PersonInfoServiceBase> Logger { get; }

		protected HttpClient HttpClient { get; }

		protected ScanPersonSecrets Secrets { get; }

		protected ServicesOptions ServicesOptions { get; }

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
				Logger.LogInformation(Messages.StartedMethodWithParameters, nameof(GetInfoAsync), JsonSerializer.Serialize(request));
				var result = await GetPersonInfoAsync(request);
				Logger.LogInformation(Messages.OperationResult, JsonSerializer.Serialize(result));

				return result;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Messages.OperationError, GetType().Name);

				return GetFail<PersonInfoItem>(Messages.ClientOperationError);
			}
		}

		public virtual bool CanAccept()
		{
			return !ServicesOptions.UnUsingServices.Contains(GetType().Name);
		}

		/// <summary>
		/// Метод получения информации свойственной для сервиса.
		/// </summary>
		/// <param name="request">Запрос с входящими данными.</param>
		/// <returns>Результат с полученной информацией конкретного сервиса.</returns>
		protected abstract Task<ScanPersonResponseBase> GetPersonInfoAsync(PersonInfoRequest request);
	}
}
