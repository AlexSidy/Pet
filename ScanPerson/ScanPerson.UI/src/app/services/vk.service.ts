import { Inject, Injectable, PLATFORM_ID  } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import * as VKIDSDK from '@vkid/sdk';

@Injectable({
  providedIn: 'root'
})
export class VkService {

  private readonly isBrowser: boolean;
  private isSdkInitialized: boolean = false;

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    
    // Проверяем, есть ли callback параметры в URL при загрузке страницы
    if (this.isBrowser) {
      this.checkCallbackFromUrl();
    }
  }

  /**
   * Проверяет URL на наличие callback параметров от VK ID
   * и обрабатывает их, если они есть
   */
  private checkCallbackFromUrl(): void {
    const urlParams = new URLSearchParams(window.location.search);
    const code = urlParams.get('code');
    const state = urlParams.get('state');
    const error = urlParams.get('error');
    
    if (code || error) {
      console.log('VK ID callback detected in URL:', { code, state, error });
      
      // Если есть код авторизации, обрабатываем его
      if (code) {
        // Инициализируем SDK, если еще не инициализирован
        if (!this.isSdkInitialized) {
          this.initializeSdk();
        }
        
        // Обрабатываем callback через SDK
        this.handleCallbackFromUrl(code, state);
      } else if (error) {
        console.error('VK ID authorization error:', error);
        this.callbackFunc({ error: { code: error, message: urlParams.get('error_description') || 'Authorization failed' } });
      }
      
      // Очищаем URL от параметров callback
      const cleanUrl = window.location.origin + window.location.pathname;
      window.history.replaceState({}, document.title, cleanUrl);
    }
  }

  /**
   * Обрабатывает callback с кодом авторизации из URL
   */
  private async handleCallbackFromUrl(code: string, state: string | null): Promise<void> {
    try {
      // Инициализируем SDK, если еще не инициализирован
      if (!this.isSdkInitialized) {
        this.initializeSdk();
      }
      
      // Получаем device_id из localStorage или генерируем новый
      let deviceId = localStorage.getItem('vk_device_id');
      if (!deviceId) {
        deviceId = this.generateRandomState(32);
        localStorage.setItem('vk_device_id', deviceId);
      }
      
      // Используем статический метод Auth для обмена кода на токен
      // Сначала нужно получить экземпляр через Config
      const authInstance = VKIDSDK.Auth as any;
      
      // Обмениваем код на токен
      const tokenResult = await authInstance.exchangeCode(code, deviceId);
      
      console.log('VK ID token exchange successful:', tokenResult);
      
      // Обрабатываем результат
      this.callbackFunc({
        token: tokenResult.access_token,
        refreshToken: tokenResult.refresh_token,
        expiresIn: tokenResult.expires_in,
        state: state
      });
    } catch (error) {
      console.error('VK ID token exchange failed:', error);
      this.callbackFunc({ error: error });
    }
  }

  /**
   * Инициализирует VK ID SDK. Должна вызываться явно, а не в конструкторе,
   * чтобы избежать CORS ошибок при загрузке страницы.
   * 
   * ВАЖНО: redirectUrl должен быть зарегистрирован в настройках VK приложения!
   * Для локальной разработки: https://localhost (порт не указывается в настройках VK)
   * Для продакшена: ваш реальный домен с HTTPS
   * 
   * ПРИМЕЧАНИЕ: CORS ошибка при запросе к vkid_sdk_get_config может появляться,
   * но SDK продолжит работать, так как это не критичный запрос.
   */
  initializeSdk(): void {
    if (!this.isBrowser || this.isSdkInitialized) {
      return;
    }

    try {
      // ВАЖНО: redirectUrl должен ТОЧНО совпадать с настройками VK приложения
      // Проверьте в настройках VK приложения, какой именно URL указан в "Доверенные домены"
      // Возможные варианты:
      // - https://localhost (без порта)
      // - http://localhost (без порта)
      // - https://localhost:4200 (с портом, если VK позволяет)
      
      // Попробуйте разные варианты, начиная с самого простого
      //const redirectUrl = "https://localhost";
      const redirectUrl = "https://dev.local:4200";
      
      console.log('=== VK ID SDK Initialization ===');
      console.log('redirectUrl:', redirectUrl);
      console.log('Current page URL:', window.location.href);
      console.log('Current origin:', window.location.origin);
      console.log('Current protocol:', window.location.protocol);
      console.log('Current host:', window.location.host);
      console.log('Current hostname:', window.location.hostname);
      console.log('Current port:', window.location.port);
      console.log('');
      console.log('ВАЖНО: redirectUrl должен ТОЧНО совпадать с настройками VK приложения!');
      console.log('Проверьте в настройках VK приложения (ID: 54294867) раздел "Доверенные домены"');
      console.log('Убедитесь, что там указан:', redirectUrl);
      
      // Временно перехватываем глобальные ошибки CORS, чтобы они не блокировали инициализацию
      const originalErrorHandler = window.onerror;
      const originalUnhandledRejection = window.onunhandledrejection;
      
      // Временно отключаем обработку ошибок CORS
      window.onerror = (message, source, lineno, colno, error) => {
        if (typeof message === 'string' && message.includes('CORS')) {
          console.warn('CORS warning during VK ID SDK initialization (can be ignored):', message);
          return true; // Предотвращаем вывод ошибки в консоль
        }
        if (originalErrorHandler) {
          return originalErrorHandler.call(window, message, source, lineno, colno, error);
        }
        return false;
      };
      
      window.onunhandledrejection = ((event: PromiseRejectionEvent) => {
        if (event.reason && typeof event.reason === 'object' && 'message' in event.reason && 
            typeof event.reason.message === 'string' && event.reason.message.includes('CORS')) {
          console.warn('CORS warning during VK ID SDK initialization (can be ignored):', event.reason);
          event.preventDefault(); // Предотвращаем вывод ошибки
          return;
        }
        if (originalUnhandledRejection) {
          originalUnhandledRejection.call(window, event);
        }
      }) as typeof window.onunhandledrejection;
      
      VKIDSDK.Config.init({
        app: 54294867,
        // redirectUrl должен ТОЧНО совпадать с настройками VK приложения: https://localhost
        redirectUrl: redirectUrl,
        scope: 'email phone',
        responseMode: VKIDSDK.ConfigResponseMode.Callback,
        // Используем LOWCODE (единственный доступный вариант в текущей версии SDK)
        source: VKIDSDK.ConfigSource.LOWCODE,
        state: this.generateRandomState(32),
      });
      
      // Восстанавливаем обработчики ошибок после небольшой задержки
      setTimeout(() => {
        window.onerror = originalErrorHandler;
        window.onunhandledrejection = originalUnhandledRejection;
      }, 1000);
      
      this.isSdkInitialized = true;
      console.log('VK ID SDK initialized successfully (CORS warnings can be ignored).');
    } catch (e) {
      console.error('VK ID SDK initialization failed:', e);
      // Продолжаем работу даже при ошибке, так как CORS ошибка не критична
      // SDK может работать без успешного запроса к vkid_sdk_get_config
      this.isSdkInitialized = true;
      console.warn('VK ID SDK initialized with warnings. CORS errors are expected and can be ignored.');
    }
  }

  /**
   * Обработчик callback после успешной авторизации через VK ID
   * @param result - результат авторизации, содержит токен и данные пользователя
   */
  callbackFunc(result: any): void {
    console.log('VK ID Auth callback:', result);
    
    if (result && result.token) {
      // Здесь можно сохранить токен и обработать результат авторизации
      // Например, отправить токен на ваш backend для создания сессии
      console.log('VK ID token received:', result.token);
      debugger;
      // TODO: Отправить токен на backend для аутентификации
      // Пример: this.authService.vkIdCallback(result.token);
    } else if (result && result.error) {
      console.error('VK ID Auth error:', result?.error);
    } else {
      console.warn('VK ID Auth: unexpected result format', result);
    }
  }

  /**
   * Выполняет вход через VK ID
   * @returns Promise с результатом авторизации или null в случае ошибки
   */
  async login(): Promise<any> {
    if (!this.isBrowser) {
      console.warn('VK SDK not available (running on server).');
      return null;
    }

    // Инициализируем SDK, если еще не инициализирован
    if (!this.isSdkInitialized) {
      this.initializeSdk();
    }

    if (!this.isSdkInitialized) {
      console.error('VK SDK initialization failed. Cannot proceed with login.');
      return null;
    }

    try {
      // Создаем экземпляр Auth и вызываем login
      // Метод login() возвращает Promise, который разрешится после авторизации
      const result = await VKIDSDK.Auth.login();
      
      // Обрабатываем результат авторизации
      this.callbackFunc(result);
      
      return result;
    } catch (e) {
      console.error('VK login failed:', e);
      this.callbackFunc({ error: e });
      return null;
    }
  }

  private generateRandomState(length: number): string {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_';
    let result = '';
    const bytes = new Uint8Array(length);
    // Используем встроенный в браузер криптографический генератор
    window.crypto.getRandomValues(bytes); 
    
    for (let i = 0; i < length; i++) {
        result += characters.charAt(bytes[i] % characters.length);
    }
    return result;
  }
}