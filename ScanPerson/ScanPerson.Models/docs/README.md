Инструкция для создания sdk проекта Models:
1) Правой кнопкой мыши на проекте ScanPerson.Models -> Edit project file -> повысить версию проекта.
2) Выполнить команду dotnet pack --configuration Release
   Эта команда создаст .nupkg файл в папке bin/Release.
3) На NuGet.org должна быть создана учетная запись в разделе API Keys ддолжен быть ключ. нужно скопировать его и вставить в следующую команду вместо {YOUR_API_KEY}.
4) Использовать команду dotnet nuget push для публикации пакета:
   dotnet nuget push bin/Release/ScanPersonModelsSdk.1.0.3.nupkg --api-key {YOUR_API_KEY} --source https://api.nuget.org/v3/index.json
5) После успешной публикации вы сможете найти свой пакет на NuGet.org по его имени.
