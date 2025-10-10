using Microsoft.Extensions.Logging;

using Moq;

namespace TelegramBots.Unit.Tests
{
	public abstract class UnitTestsBase
	{
		public required TestContext TestContext { get; set; }

		public UnitTestsBase()
		{
		}

		protected static void VerifyLog<T>(Mock<ILogger<T>> logger, LogLevel level, Times times, string messagePart) where T : class
		{
			logger.Verify(
				x => x.Log(
					level,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(messagePart)),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()!
				),
				times
			);
		}
	}
}