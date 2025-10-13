using System.Text.Json;

using AutoMapper;

using Grpc.Core;

using Microsoft.AspNetCore.Authorization;

using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.GrpcServices;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Requests;

namespace ScanPerson.WebApi.GrpcServices
{
	/// <summary>
	/// Gprc server service.
	/// </summary>
	public class GrpcPersonInfoService : GrpcPersonInfo.GrpcPersonInfoBase
	{
		private readonly ILogger<GrpcPersonInfoService> _logger;
		private readonly IPersonInfoServicesAggregator _service;
		private readonly IMapper _mapper;

		public GrpcPersonInfoService(
			ILogger<GrpcPersonInfoService> logger,
			IPersonInfoServicesAggregator service,
			IMapper mapper)
		{
			_logger = logger;
			_service = service;
			_mapper = mapper;
		}

		/// <summary>
		/// Method for get person info.
		/// </summary>
		/// <param name="request">The request containing the input data.</param>
		/// <param name="context">Context for server-side.</param>
		/// <returns>Response with person info.</returns>
		[Authorize(Policy = Program.AuthorizationPolicy)]
		public override async Task<GrpcScanPersonResponse> GetGrpcScanPersonInfo(
			GrpcPersonInfoRequest request,
			ServerCallContext? context)
		{
			_logger.LogInformation(Messages.StartedMethodWithParameters, nameof(GetGrpcScanPersonInfo), JsonSerializer.Serialize(request));
			var mappedRequest = _mapper.Map<PersonInfoRequest>(request);
			var result = await _service.GetScanPersonInfoAsync(mappedRequest);
			var response = _mapper.Map<GrpcScanPersonResponse>(result);
			_logger.LogInformation(Messages.OperationResult, JsonSerializer.Serialize(response));

			return response;
		}
	}
}
