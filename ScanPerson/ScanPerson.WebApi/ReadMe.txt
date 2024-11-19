запуск проекта:
1. Переходим в VS Code в папку cd ..Pet\ScanPerson\ScanPerson.UI
2. Запускаем команду npm run build 
3. В VS преходим в папку cd ..ScanPerson\ScanPerson.WebApi
4. Запускаем команду в терминале docker build -t sidralex/scanpersonwebapi:latest .
5. Запускаем докер  docker run --rm -it -p 8000:8080 -e ASPNETCORE_HTTP_PORTS=8080 sidralex/scanpersonwebapi:latest
6. Переходим на страницу http://localhost:8000/
