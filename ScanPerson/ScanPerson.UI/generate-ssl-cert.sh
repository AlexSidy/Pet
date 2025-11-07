#!/bin/bash
# Bash скрипт для генерации самоподписанного SSL сертификата для локальной разработки
# Использование: chmod +x generate-ssl-cert.sh && ./generate-ssl-cert.sh

CERT_DIR="./ssl"
KEY_FILE="$CERT_DIR/server.key"
CERT_FILE="$CERT_DIR/server.crt"

# Создаем директорию для сертификатов, если её нет
mkdir -p "$CERT_DIR"

echo "Генерация SSL сертификата для локальной разработки..."

# Генерируем приватный ключ
echo "Генерация приватного ключа..."
openssl genrsa -out "$KEY_FILE" 2048

# Создаем конфигурационный файл для сертификата
cat > "$CERT_DIR/cert.conf" <<EOF
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
EOF

# Генерируем самоподписанный сертификат
echo "Генерация самоподписанного сертификата..."
openssl req -new -x509 -key "$KEY_FILE" -out "$CERT_FILE" -days 365 -config "$CERT_DIR/cert.conf" -extensions v3_req

# Удаляем временный конфигурационный файл
rm "$CERT_DIR/cert.conf"

echo ""
echo "Сертификат успешно создан!"
echo "  Ключ: $KEY_FILE"
echo "  Сертификат: $CERT_FILE"
echo ""
echo "Теперь можно запустить приложение с HTTPS:"
echo "  npm run start:https"

