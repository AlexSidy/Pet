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
  }

  /**
   * Инициализирует VK ID SDK. Должна вызываться явно, а не в конструкторе,
   * чтобы избежать CORS ошибок при загрузке страницы.
   * 
   * ВАЖНО: redirectUrl должен быть зарегистрирован в настройках VK приложения!
   * Для локальной разработки: http://localhost:4200
   * Для продакшена: ваш реальный домен
   */
  initializeSdk(): void {
    if (!this.isBrowser || this.isSdkInitialized) {
      return;
    }

    try {
      // Получаем текущий URL для redirectUrl
      // ВАЖНО: этот URL должен быть зарегистрирован в настройках VK приложения!
      const currentUrl = window.location.origin + window.location.pathname;
      
      VKIDSDK.Config.init({
        app: 54294867,
        // redirectUrl должен точно соответствовать URL, зарегистрированному в настройках VK приложения
        // Для локальной разработки: http://localhost:4200 (или http://localhost:4200/auth)
        redirectUrl: currentUrl,
        scope: 'email phone',
        responseMode: VKIDSDK.ConfigResponseMode.Callback,
        // Используем VKID вместо LOWCODE для обычного веб-приложения
        source: VKIDSDK.ConfigSource.VKID,
        state: this.generateRandomState(32),
      });
      
      // Настраиваем обработчик callback для получения результата авторизации
      VKIDSDK.Auth.onAuth((result) => {
        this.callbackFunc(result);
      });
      
      this.isSdkInitialized = true;
      console.log('VK ID SDK initialized successfully.');
    } catch (e) {
      console.error('VK ID SDK initialization failed:', e);
      this.isSdkInitialized = false;
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
      // TODO: Отправить токен на backend для аутентификации
    } else if (result && result.error) {
      console.error('VK ID Auth error:', result.error);
    }
  }

  login() {
    if (!this.isBrowser) {
      console.warn('VK SDK not available (running on server).');
      return;
    }

    // Инициализируем SDK, если еще не инициализирован
    if (!this.isSdkInitialized) {
      this.initializeSdk();
    }

    if (!this.isSdkInitialized) {
      console.error('VK SDK initialization failed. Cannot proceed with login.');
      return;
    }

    try {
      const result = VKIDSDK.Auth.login();
      return result;
    } catch (e) {
      console.error('VK login failed:', e);
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