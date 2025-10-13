using System.Net;
using System.Net.Sockets;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Moq;

using ScanPerson.BusinessLogic.Managers;
using ScanPerson.Models.Options;

namespace ScanPerson.Unit.Tests
{
	[TestClass]
	public class HttpContextManagerTests
	{
		/// <summary>
		/// CUT.
		/// </summary>
		private HttpContextManager? _cut;

		private const string LocalTestIp = "127.0.0.1";
		private const string PublicTestIp = "203.0.113.42";
		private const string TrustedHostName = "localhost"; // IP: 127.0.0.1

		private readonly Mock<ILogger<HttpContextManager>> _mockLogger;
		private readonly HostWhiteListOptions _options;

		public HttpContextManagerTests()
		{
			_mockLogger = new Mock<ILogger<HttpContextManager>>();
			_options = new HostWhiteListOptions([TrustedHostName]);

			_cut = new HttpContextManager(_mockLogger.Object, _options);
		}


		[TestMethod]
		public async Task IsInternalRequestAsync_NullHttpContext_ReturnsFalse()
		{
			// Arrange
			HttpContext? httpContext = null;

			// Act
			var result = await _cut!.IsInternalRequestAsync(httpContext);

			// Assert
			Assert.IsFalse(result, "Manager should return false for null HttpContext.");
		}

		[TestMethod]
		public async Task IsInternalRequestAsync_RemoteIpIsNull_ReturnsFalse()
		{
			// Arrange
			var httpContext = new DefaultHttpContext();
			httpContext.Connection.RemoteIpAddress = null;

			// Act
			var result = await _cut!.IsInternalRequestAsync(httpContext);

			// Assert
			Assert.IsFalse(result, "Manager should return false when RemoteIpAddress is null.");
		}

		[TestMethod]
		public async Task IsInternalRequestAsync_TrustedInternalIp_ReturnsTrue()
		{
			// Arrange
			var httpContext = new DefaultHttpContext();

			// Устанавливаем IP, который будет резолвиться через Dns.GetHostEntryAsync('localhost')
			httpContext.Connection.RemoteIpAddress = IPAddress.Parse(LocalTestIp);

			// Act
			var result = await _cut!.IsInternalRequestAsync(httpContext);

			// Assert
			Assert.IsTrue(result, "Request from a trusted (white-listed) IP should return true.");
		}

		[TestMethod]
		public async Task IsInternalRequestAsync_UntrustedIp_ReturnsFalse()
		{
			// Arrange
			var httpContext = new DefaultHttpContext();

			// Устанавливаем публичный IP, который не должен быть в списке 'localhost'
			httpContext.Connection.RemoteIpAddress = IPAddress.Parse(PublicTestIp);

			// Act
			var result = await _cut!.IsInternalRequestAsync(httpContext);

			// Assert
			Assert.IsFalse(result, "Request from an untrusted IP should return false.");
		}

		[TestMethod]
		public async Task IsInternalRequestAsync_IPv6MappedToIPv4_ReturnsTrue()
		{
			// Arrange
			var httpContext = new DefaultHttpContext();
			var ipv6Address = IPAddress.Parse("::ffff:127.0.0.1");
			httpContext.Connection.RemoteIpAddress = ipv6Address;

			// Act
			var result = await _cut!.IsInternalRequestAsync(httpContext);

			// Assert
			Assert.IsTrue(result, "IPv6 address mapping to a trusted IPv4 should return true.");
		}

		[TestMethod]
		public async Task GetIpAddressesSafeAsync_EmptyHostName_ReturnsFalse()
		{
			// Arrange
			var httpContext = new DefaultHttpContext();

			// Устанавливаем IP, который будет резолвиться через Dns.GetHostEntryAsync('localhost')
			httpContext.Connection.RemoteIpAddress = IPAddress.Parse(LocalTestIp);
			_cut = new HttpContextManager(_mockLogger.Object, new HostWhiteListOptions([string.Empty]));

			// Act
			var result = await _cut!.IsInternalRequestAsync(httpContext);

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public async Task GetIpAddressesSafeAsync_InvalidHostName_LogsErrorAndReturnsEmptyArray()
		{
			// Arrange
			var httpContext = new DefaultHttpContext();

			httpContext.Connection.RemoteIpAddress = IPAddress.Parse(LocalTestIp);
			_cut = new HttpContextManager(_mockLogger.Object,
				new HostWhiteListOptions(["invalid_host_name_that_should_fail_dns_resolution"]));

			// Act
			var result = await _cut!.IsInternalRequestAsync(httpContext);

			// Assert
			Assert.IsFalse(result);

			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("DNS resolution error")),
					It.IsAny<SocketException>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);
		}
	}
}