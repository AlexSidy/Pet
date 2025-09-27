Instructions for creating the ScanPerson.Common SDK project.
1. Right-click the ScanPerson.Common project.

2. Select Edit project file.

3. Increment the project version number.

4. Execute the command. This will create a .nupkg file in the bin/Release folder.

```
	dotnet pack --configuration Release 
```

5. On NuGet.org, you need to have an account. Copy the API key from the API Keys section and replace {YOUR_API_KEY} in the command below.

6. Use the dotnet nuget push command to publish the package:

```
	dotnet nuget push bin/Release/ScanPerson.Models.Sdk.1.0.8.nupkg --api-key {YOUR_API_KEY} --source https://api.nuget.org/v3/index.json
```

7. After a successful publication, you can find your package on NuGet.org by its name.