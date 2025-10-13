using Microsoft.Extensions.Configuration;

using ScanPerson.Common.Helpers;
using ScanPerson.Common.Resources;
using ScanPerson.Models.Attributes;

namespace ScanPerson.Unit.Tests
{
	[TestClass]
	public class EnviromentHelperTests
	{
		private const string TestVarName = "TEST_VAR";

		[TestCleanup]
		public void Cleanup()
		{
			Environment.SetEnvironmentVariable(TestVarName, null);
			Environment.SetEnvironmentVariable("TEST_ENV_STRING", null);
			Environment.SetEnvironmentVariable("TEST_ENV_INT", null);
			Environment.SetEnvironmentVariable("TEST_ENV_BOOL", null);
		}

		[TestMethod]
		public void GetVariableByName_VariableExists_ReturnsValue()
		{
			// Arrange
			const string expected = "test value";
			Environment.SetEnvironmentVariable(TestVarName, expected);

			// Act
			var actual = EnviromentHelper.GetVariableByName(TestVarName);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void GetVariableByName_VariableNotExists_ThrowsException()
		{
			// Arrange
			Environment.SetEnvironmentVariable(TestVarName, null);

			// Act
			Assert.ThrowsExactly<InvalidOperationException>(() =>
			{

				// Act
				try
				{
					EnviromentHelper.GetVariableByName(TestVarName);
				}
				catch (InvalidOperationException ex)
				{
					// Assert
					Assert.Contains(TestVarName, ex.Message, "The exception must contain a variable name.");
					throw;
				}
			});
		}

		[TestMethod]
		public void GetVariableArrayByName_VariableExistsWithMultipleValues_ReturnsArray()
		{
			// Arrange
			const string variableValue = "host1, host2,host3 , host4";
			var expected = new[] { "host1", "host2", "host3", "host4" };
			Environment.SetEnvironmentVariable(TestVarName, variableValue);

			// Act
			var actual = EnviromentHelper.GetVariableArrayByName(TestVarName);

			// Assert
			CollectionAssert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void GetVariableArrayByName_VariableExistsWithEmptyString_ReturnsEmptyArray()
		{
			// Arrange
			Environment.SetEnvironmentVariable(TestVarName, "");

			// Act
			var actual = EnviromentHelper.GetVariableArrayByName(TestVarName);

			// Assert
			Assert.IsNotNull(actual);
			Assert.AreEqual(0, actual.Length, "An empty string should return an empty array.");
		}

		[TestMethod]
		public void GetVariableArrayByName_VariableNotExists_ReturnsEmptyArray()
		{
			// Arrange
			Environment.SetEnvironmentVariable(TestVarName, null);

			// Act
			var actual = EnviromentHelper.GetVariableArrayByName(TestVarName);

			// Assert
			Assert.IsNotNull(actual);
			Assert.AreEqual(0, actual.Length, "A missing variable must return an empty array.");
		}

		[TestMethod]
		public void GetHostOptionsBySectionByName_SectionExists_ReturnsOptions()
		{
			// Arrange
			const string sectionName = "TestService";
			var config = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string?>
				{
				{ $"{sectionName}:Host", "test.host" },
				{ $"{sectionName}:Port", "1234" }
				})
				.Build();

			// Act
			var options = EnviromentHelper.GetHostOptionsBySectionName(sectionName, config);

			// Assert
			Assert.IsNotNull(options);
			Assert.AreEqual("test.host", options.Host);
			Assert.AreEqual(1234, options.Port);
		}

		[TestMethod]
		public void GetHostOptionsBySectionName_SectionNotExists_ThrowsException()
		{
			// Arrange
			var sectionName = "MissingSection";
			var expectedError = string.Format(Messages.SectionNotFound, sectionName);
			var config = new ConfigurationBuilder().Build();

			// Act
			Assert.ThrowsExactly<InvalidOperationException>(() =>
			{
				// Act
				try
				{
					EnviromentHelper.GetHostOptionsBySectionName(sectionName, config);
				}
				catch (InvalidOperationException ex)
				{
					// Assert
					Assert.Contains(expectedError, ex.Message, "The exception should be about a section not found.");
					throw;
				}
			});
		}

		[TestMethod]
		public void GetFilledFromEnvironment_AllVariablesExistAndValid_ReturnsFilledModel()
		{
			// Arrange
			Environment.SetEnvironmentVariable("TEST_ENV_STRING", "Hello");
			Environment.SetEnvironmentVariable("TEST_ENV_INT", "42");
			Environment.SetEnvironmentVariable("TEST_ENV_BOOL", "True");

			// Act
			var result = EnviromentHelper.GetFilledFromEnvironment<TestEnvOptions>();

			// Assert
			Assert.AreEqual("Hello", result.StringValue);
			Assert.AreEqual(42, result.IntValue);
			Assert.IsTrue(result.BoolValue);
			Assert.IsNull(result.NotMappedValue, "A property without an attribute should be ignored.");
		}

		[TestMethod]
		public void GetFilledFromEnvironment_MissingVariable_ThrowsException()
		{
			// Arrange
			Environment.SetEnvironmentVariable("TEST_ENV_INT", "42");
			Environment.SetEnvironmentVariable("TEST_ENV_BOOL", "True");
			Environment.SetEnvironmentVariable("TEST_ENV_STRING", null);

			// Act
			Assert.ThrowsExactly<InvalidOperationException>(() => EnviromentHelper.GetFilledFromEnvironment<TestEnvOptions>());
		}

		[TestMethod]
		public void GetFilledFromEnvironment_ConversionError_ThrowsException()
		{
			// Arrange
			Environment.SetEnvironmentVariable("TEST_ENV_STRING", "Hello");
			// Пытаемся преобразовать строку в int
			Environment.SetEnvironmentVariable("TEST_ENV_INT", "not_an_int");
			Environment.SetEnvironmentVariable("TEST_ENV_BOOL", "True");

			// Act
			Assert.ThrowsExactly<InvalidOperationException>(() =>
			{
				// Act
				try
				{
					EnviromentHelper.GetFilledFromEnvironment<TestEnvOptions>();
				}
				catch (InvalidOperationException ex)
				{
					// Assert
					Assert.AreEqual("Could not convert environment variable 'TEST_ENV_INT' to type 'Int32'.", ex.Message,
						"There must be a conversion error.");
					throw;
				}
			});
		}

		public class TestEnvOptions
		{
			[EnvironmentVariable("TEST_ENV_STRING")]
			public string? StringValue { get; set; }

			[EnvironmentVariable("TEST_ENV_INT")]
			public int IntValue { get; set; }

			[EnvironmentVariable("TEST_ENV_BOOL")]
			public bool BoolValue { get; set; }

			public string? NotMappedValue { get; set; }
		}
	}
}