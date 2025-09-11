using ScanPerson.BusinessLogic.MapperProfiles;
using ScanPerson.Common.Helpers;

namespace ScanPerson.WebApi.Extensions
{
	/// <summary>
	/// Extensions for adding AutoMapper config.
	/// </summary>
	public static class AutoMapperExtensions
	{
		/// <summary>
		/// Add AutoMapper config.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="services"></param>
		public static void AddScanPersonAutoMapper(this IServiceCollection services)
		{
			services.AddAutoMapper(cfg =>
			{
				cfg.LicenseKey = EnviromentHelper.GetViriableByName("AUTO_MAPPER_LICENSE_KEY");
				cfg.AddProfile<DisplayProfile>();
			});
		}
	}
}
