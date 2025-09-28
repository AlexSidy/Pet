using System.Reflection;
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

				return GetFail<PersonInfoItem>(Messages.ClientOperationError);
			}
		}

		private ScanPersonResponseBase GetAggregatedResult<TResult>(TResult[] results) where TResult : ScanPersonResponseBase
		{
			var errors = results
				.Where(x => !x.IsSuccess)
				.Select(x => x.Error)
				.Where(e => !string.IsNullOrEmpty(e));

			if (results.Any(x => x.IsSuccess))
			{
				return GetSuccess(
					results
						.Where(x => x.IsSuccess)
						.OfType<ScanPersonResultResponse<PersonInfoItem>>()
						.Where(x => x != null)
						.Select(x => x!.Result)
						.ToHashSet()
						.Aggregate((x, y) => GetAggregatePersons(x, y)),
					errors);
			}

			return GetFail(errors);
		}

		/// <summary>
		/// Method for aggregation of person info.
		/// </summary>
		/// <param name="current">Current person info.</param>
		/// <param name="next">>Next person info.</param>
		/// <returns>Person info.</returns>
		private static PersonInfoItem GetAggregatePersons(PersonInfoItem current, PersonInfoItem next)
		{
			var properties = current.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var property in properties)
			{
				if (property.Name == nameof(PersonInfoItem.Id))
				{
					continue;
				}

				var nextValue = property.GetValue(next);
				var currentValue = property.GetValue(current);
				if (nextValue != null)
				{
					property.SetValue(current, property.GetValue(next));

				}
			}

			return current;
		}
	}
}
