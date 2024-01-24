import { ApplicationConfig } from '@angular/core';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { environment } from '../environments/environment';
import { API_BASE_URL } from './core/api/ApiClient';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { jwtInterceptor } from './core/auth/auth.jwt-interceptor';
import { MessageService } from 'primeng/api';

export const appConfig: ApplicationConfig = {
  providers: [
    provideAnimations(),
    provideRouter(routes),
    {
      provide: API_BASE_URL,
      useValue: environment.apiBaseUrl
    },
    provideHttpClient(withInterceptors([jwtInterceptor])),
    MessageService
  ]
};
