using ScanPerson.BusinessLogic.MapperProfiles;
using ScanPerson.Common.Helpers;
using ScanPerson.Common.MapperProfiles;

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
		/// <param name="services"></param>
		public static void AddScanPersonAutoMapper(this IServiceCollection services)
		{
			services.AddAutoMapper(cfg =>
			{
				cfg.LicenseKey = EnviromentHelper.GetVariableByName("AUTO_MAPPER_LICENSE_KEY");
				cfg.AddProfile<DisplayProfile>();
				cfg.AddProfile<RequestProfile>();
				cfg.AddProfile<GrpcProfile>();
			});
		}
	}
}
