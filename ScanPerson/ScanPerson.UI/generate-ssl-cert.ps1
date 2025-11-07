# PowerShell скрипт для генерации самоподписанного SSL сертификата для локальной разработки
# Использование: .\generate-ssl-cert.ps1

$certPath = ".\ssl"
$keyFile = "$certPath\server.key"
$certFile = "$certPath\server.crt"

# Создаем директорию для сертификатов, если её нет
if (-not (Test-Path $certPath)) {
    New-Item -ItemType Directory -Path $certPath | Out-Null
    Write-Host "Создана директория $certPath" -ForegroundColor Green
}

# Проверяем наличие OpenSSL
$opensslPath = Get-Command openssl -ErrorAction SilentlyContinue

if (-not $opensslPath) {
    Write-Host "OpenSSL не найден. Устанавливаем через Chocolatey или используем альтернативный метод..." -ForegroundColor Yellow
    
    # Альтернативный метод через PowerShell (требует Windows 10+)
    try {
        $cert = New-SelfSignedCertificate `
            -DnsName "localhost" `
            -CertStoreLocation "cert:\LocalMachine\My" `
            -FriendlyName "Angular Dev Server SSL" `
            -NotAfter (Get-Date).AddYears(1) `
            -KeyUsage DigitalSignature, KeyEncipherment `
            -KeyAlgorithm RSA `
            -KeyLength 2048
        
        # Экспортируем сертификат в PFX
        $pfxPath = "$certPath\server.pfx"
        $password = ConvertTo-SecureString -String "angular-dev" -Force -AsPlainText
        Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $password | Out-Null
        
        # Конвертируем PFX в PEM (требует OpenSSL или другой инструмент)
        Write-Host "Сертификат создан: $pfxPath" -ForegroundColor Green
        Write-Host "Для использования с Angular нужны файлы .key и .crt" -ForegroundColor Yellow
        Write-Host "Установите OpenSSL и запустите:" -ForegroundColor Yellow
        Write-Host "  openssl pkcs12 -in $pfxPath -nocerts -nodes -out $keyFile -passin pass:angular-dev" -ForegroundColor Cyan
        Write-Host "  openssl pkcs12 -in $pfxPath -clcerts -nokeys -out $certFile -passin pass:angular-dev" -ForegroundColor Cyan
        
        # Удаляем сертификат из хранилища
        Remove-Item "Cert:\LocalMachine\My\$($cert.Thumbprint)" -ErrorAction SilentlyContinue
    }
    catch {
        Write-Host "Ошибка при создании сертификата: $_" -ForegroundColor Red
        Write-Host "Пожалуйста, установите OpenSSL:" -ForegroundColor Yellow
        Write-Host "  choco install openssl" -ForegroundColor Cyan
        Write-Host "Или скачайте с https://slproweb.com/products/Win32OpenSSL.html" -ForegroundColor Cyan
        exit 1
    }
}
else {
    Write-Host "Используется OpenSSL: $($opensslPath.Source)" -ForegroundColor Green
    
    # Генерируем приватный ключ
    Write-Host "Генерация приватного ключа..." -ForegroundColor Yellow
    & openssl genrsa -out $keyFile 2048
    
    # Создаем конфигурационный файл для сертификата
    $configContent = @"
[req]
distinguished_name = req_distinguished_name
x509_extensions = v3_req
prompt = no

[req_distinguished_name]
C = RU
ST = Moscow
L = Moscow
O = Development
CN = localhost

[v3_req]
keyUsage = keyEncipherment, dataEncipherment
extendedKeyUsage = serverAuth
subjectAltName = @alt_names

[alt_names]
DNS.1 = localhost
DNS.2 = *.localhost
IP.1 = 127.0.0.1
IP.2 = ::1
"@
    
    $configFile = "$certPath\cert.conf"
    $configContent | Out-File -FilePath $configFile -Encoding ASCII
    
    # Генерируем самоподписанный сертификат
    Write-Host "Генерация самоподписанного сертификата..." -ForegroundColor Yellow
    & openssl req -new -x509 -key $keyFile -out $certFile -days 365 -config $configFile -extensions v3_req
    
    # Удаляем временный конфигурационный файл
    Remove-Item $configFile -ErrorAction SilentlyContinue
    
    Write-Host "`nСертификат успешно создан!" -ForegroundColor Green
    Write-Host "  Ключ: $keyFile" -ForegroundColor Cyan
    Write-Host "  Сертификат: $certFile" -ForegroundColor Cyan
    Write-Host "`nТеперь можно запустить приложение с HTTPS:" -ForegroundColor Yellow
    Write-Host "  npm run start:https" -ForegroundColor Cyan
}

