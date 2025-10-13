using AutoMapper;

using Grpc.Core;

using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.BusinessLogic.Services.Interfaces;
using ScanPerson.Common.GrpcServices;
using ScanPerson.Common.Tests;
using ScanPerson.Models.Items;
using ScanPerson.Models.Requests;
using ScanPerson.Models.Responses;
using ScanPerson.WebApi.GrpcServices;

using GrpcRequest = ScanPerson.Common.GrpcServices.GrpcPersonInfoRequest;
using GrpcResponse = ScanPerson.Common.GrpcServices.GrpcScanPersonResponse;

namespace ScanPerson.Unit.Tests
{
	[TestClass]
	public class GrpcPersonInfoServiceTests
	{
		/// <summary>
		/// Class under tests.
		/// </summary>
		private readonly GrpcPersonInfoService _cut;

		private readonly Mock<ILogger<GrpcPersonInfoService>> _mockLogger;
		private readonly Mock<IPersonInfoServicesAggregator> _mockServiceAggregator;
		private readonly Mock<IMapper> _mockMapper;

		public GrpcPersonInfoServiceTests()
		{
			_mockLogger = new Mock<ILogger<GrpcPersonInfoService>>();
			_mockServiceAggregator = new Mock<IPersonInfoServicesAggregator>();
			_mockMapper = new Mock<IMapper>();

			_cut = new GrpcPersonInfoService(
				_mockLogger.Object,
				_mockServiceAggregator.Object,
				_mockMapper.Object);
		}

		[TestMethod]
		public async Task GetGrpcScanPersonInfo_ValidRequest_ReturnsMappedResponse()
		{
			// Arrange
			var grpcRequest = new GrpcRequest { PhoneNumber = "Test PhoneNumber" };
			var mappedRequest = new PersonInfoRequest { PhoneNumber = "Test PhoneNumber" };
			var serviceResult = CreationHelper.GetPersonResponse();
			var expectedResponse = new GrpcResponse { IsSuccess = true, Result = new GrpcPersonInfoItem { Id = serviceResult.Result.Id ?? 0 } };

			_mockMapper
				.Setup(m => m.Map<PersonInfoRequest>(It.Is<GrpcRequest>(r => r == grpcRequest)))
				.Returns(mappedRequest);
			_mockServiceAggregator
				.Setup(s => s.GetScanPersonInfoAsync(It.Is<PersonInfoRequest>(r => r == mappedRequest)))
				.ReturnsAsync(serviceResult);
			_mockMapper
				.Setup(m => m.Map<GrpcResponse>(It.Is<ScanPersonResultResponse<PersonInfoItem>>(r => r == serviceResult)))
				.Returns(expectedResponse);

			// Act
			var actualResponse = await _cut.GetGrpcScanPersonInfo(grpcRequest, null);

			// Assert
			Assert.IsNotNull(actualResponse);
			Assert.AreEqual(expectedResponse.IsSuccess, actualResponse.IsSuccess);
			Assert.AreEqual(expectedResponse.Result.Id, actualResponse.Result.Id);

			_mockMapper.Verify(m => m.Map<PersonInfoRequest>(grpcRequest), Times.Once);
			_mockServiceAggregator.Verify(s => s.GetScanPersonInfoAsync(mappedRequest), Times.Once);
			_mockMapper.Verify(m => m.Map<GrpcResponse>(serviceResult), Times.Once);
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains(nameof(GrpcPersonInfoService.GetGrpcScanPersonInfo))),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);
		}

		[TestMethod]
		public async Task GetGrpcScanPersonInfo_ServiceThrowsException_ThrowsRpcException()
		{
			// Arrange
			var grpcRequest = new GrpcRequest { PhoneNumber = "Test PhoneNumber" };
			var mappedRequest = new PersonInfoRequest { PhoneNumber = "Test PhoneNumber" };
			_mockMapper
				.Setup(m => m.Map<PersonInfoRequest>(It.IsAny<GrpcRequest>()))
				.Returns(mappedRequest);
			_mockServiceAggregator
				.Setup(s => s.GetScanPersonInfoAsync(It.IsAny<PersonInfoRequest>()))
				.ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "Internal Error")));

			// Act
			await Assert.ThrowsExactlyAsync<RpcException>(async () => await _cut.GetGrpcScanPersonInfo(grpcRequest, null));
		}
	}
}