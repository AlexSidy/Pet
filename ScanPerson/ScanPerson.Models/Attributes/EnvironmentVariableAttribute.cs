using System;

namespace ScanPerson.Models.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class EnvironmentVariableAttribute : Attribute
	{
		public string Name { get; }

		public EnvironmentVariableAttribute(string name)
		{
			Name = name;
		}
	}
}