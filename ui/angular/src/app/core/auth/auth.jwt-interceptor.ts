import { inject } from '@angular/core';
import { HttpRequest, HttpHandlerFn } from '@angular/common/http';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

export function jwtInterceptor(request: HttpRequest<any>, next: HttpHandlerFn) {
    const authService = inject(AuthService);

    const isAuthenticated = authService.isAuthenticated;
    const token = authService.token;
    const isApiUrl = request.url.startsWith(environment.apiBaseUrl);

    if (isAuthenticated && isApiUrl) {
        request = request.clone({
            setHeaders: { Authorization: `Bearer ${token}` }
        });
    }

    return next(request);
}