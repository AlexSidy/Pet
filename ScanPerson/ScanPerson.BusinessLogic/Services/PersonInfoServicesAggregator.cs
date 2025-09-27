using System.Text.Json;

using Microsoft.Extensions.Logging;

using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.Operations.Base;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;

namespace ScanPerson.BusinessLogic.Services
{
	/// <summary>
	/// Service for aggregation of services for getting information about a person.
	/// </summary>
	public class PersonInfoServicesAggregator : OperationBase, IPersonInfoServicesAggregator
	{
		private readonly ILogger<PersonInfoServicesAggregator> _logger;
		private readonly IEnumerable<IPersonInfoService> _personInfoServices;

		public PersonInfoServicesAggregator(
			ILogger<PersonInfoServicesAggregator> logger,
			IEnumerable<IPersonInfoService> personInfoServices)
		{
			_logger = logger;
			_personInfoServices = [.. (personInfoServices ?? []).Where(x => x.CanAccept())];
			_logger.LogInformation(Messages.OperationInput, string.Join(", ", _personInfoServices.Select(x => x.GetType().Name)));
		}

		public async Task<ScanPersonResponseBase> GetScanPersonInfoAsync(PersonInfoRequest request)
		{
			try
			{
				_logger.LogInformation(Messages.StartedMethodWithParameters, nameof(GetScanPersonInfoAsync), JsonSerializer.Serialize(request));
				var results = await Task.WhenAll(_personInfoServices.Select(x => x.GetInfoAsync(request)));
				_logger.LogInformation(Messages.OperationResult, JsonSerializer.Serialize(results));

				var aggregatedResult = GetAggregatedResult(results);
				_logger.LogInformation(Messages.OperationResult, JsonSerializer.Serialize(aggregatedResult));

				return aggregatedResult;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, Messages.OperationError, GetType().Name);

				return GetFail<PersonInfoItem[]>(Messages.ClientOperationError);
			}
		}
	}
}
