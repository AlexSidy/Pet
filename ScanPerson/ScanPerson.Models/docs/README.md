���������� ��� �������� sdk ������� Models:
1) ������ ������� ���� �� ������� ScanPerson.Models -> Edit project file -> �������� ������ �������.
2) ��������� ������� dotnet pack --configuration Release
   ��� ������� ������� .nupkg ���� � ����� bin/Release.
3) �� NuGet.org ������ ���� ������� ������� ������ � ������� API Keys ������� ���� ����. ����� ����������� ��� � �������� � ��������� ������� ������ {YOUR_API_KEY}.
4) ������������ ������� dotnet nuget push ��� ���������� ������:
   dotnet nuget push bin/Release/ScanPersonModelsSdk.1.0.3.nupkg --api-key {YOUR_API_KEY} --source https://api.nuget.org/v3/index.json
5) ����� �������� ���������� �� ������� ����� ���� ����� �� NuGet.org �� ��� �����.
