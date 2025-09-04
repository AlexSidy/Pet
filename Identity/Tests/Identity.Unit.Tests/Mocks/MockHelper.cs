using Microsoft.AspNetCore.Identity;

using Moq;

namespace Identity.Unit.Tests.Mocks
{
	public class MockHelper
	{
#pragma warning disable CS8625
		public static Mock<UserManager<TUser>> GetMockUserManager<TUser>(Mock<IUserStore<TUser>>? store = null)
		where TUser : class
		{
			store ??= new Mock<IUserStore<TUser>>();

			// Создаем мок UserManager с необходимыми зависимостями
			var mockUserManager = new Mock<UserManager<TUser>>(
				store.Object,
				null, // IOptions<IdentityOptions>
				null, // IPasswordHasher<TUser>
				null, // IEnumerable<IUserValidator<TUser>>
				null, // IEnumerable<IPasswordValidator<TUser>>
				null, // ILookupNormalizer
				null, // IdentityErrorDescriber
				null, // IServiceProvider
				null); // ILogger<UserManager<TUser>>

			return mockUserManager;
		}
#pragma warning restore CS8625
	}
}
