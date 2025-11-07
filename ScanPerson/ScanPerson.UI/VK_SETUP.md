# Настройка VK ID для локальной разработки

## Проблема

VK ID требует точного совпадения `redirectUrl` с настройками приложения, но не позволяет указывать порт в настройках. Это создает проблему, когда приложение работает на нестандартном порту (например, 4200).

## Решение: Использование локального домена

### Шаг 1: Настройка hosts-файла

Добавьте запись в файл hosts для локального домена:

**Windows:**
1. Откройте файл `C:\Windows\System32\drivers\etc\hosts` от имени администратора
2. Добавьте строку:
   ```
   127.0.0.1 dev.local
   ```

**Linux/macOS:**
1. Откройте файл `/etc/hosts` с правами sudo
2. Добавьте строку:
   ```
   127.0.0.1 dev.local
   ```

### Шаг 2: Настройка SSL сертификата для dev.local

Обновите скрипт генерации сертификата, чтобы включить `dev.local` в Subject Alternative Names.

**Windows (PowerShell):**
```powershell
# Обновите generate-ssl-cert.ps1, добавив dev.local в DNS.3
```

**Linux/macOS:**
```bash
# Обновите generate-ssl-cert.sh, добавив dev.local в DNS.3
```

Или создайте новый сертификат с dev.local:
```bash
openssl req -x509 -newkey rsa:4096 -keyout ssl/server.key -out ssl/server.crt -days 365 \
  -subj "/C=RU/ST=Moscow/L=Moscow/O=Development/CN=dev.local" \
  -addext "subjectAltName=DNS:dev.local,DNS:localhost,DNS:*.localhost,IP:127.0.0.1"
```

### Шаг 3: Настройка VK приложения

В настройках VK приложения (ID: 54294867) добавьте в доверенные домены:
- `https://dev.local`

### Шаг 4: Обновление кода

Измените `redirectUrl` в `vk.service.ts`:
```typescript
const redirectUrl = "https://dev.local";
```

### Шаг 5: Запуск приложения

Запустите приложение с новым доменом:
```bash
ng serve --host dev.local --ssl --ssl-cert ssl/server.crt --ssl-key ssl/server.key
```

Или обновите `angular.json` для автоматического использования dev.local.

## Альтернативное решение: Использование стандартного порта 443

Если вы можете настроить локальный сервер на порту 443 (требует прав администратора):

1. Настройте Angular dev server на порту 443
2. В настройках VK укажите `https://localhost`
3. В коде используйте `redirectUrl: "https://localhost"`

## Текущая конфигурация

Сейчас используется `https://localhost` без порта. Это работает только если:
- Приложение запущено на стандартном HTTPS порту (443)
- Или VK SDK правильно обрабатывает редирект с портом

Если это не работает, используйте решение с `dev.local` доменом.

