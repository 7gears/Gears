import { inject } from '@angular/core';
import { CanActivateChildFn, CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const AuthGuard: CanActivateFn | CanActivateChildFn = (_route, state) =>
{
    const _authService = inject(AuthService);
    const _router = inject(Router);

    if(!_authService.isAuthenticated) {
        const redirectURL = `redirectURL=${state.url}`;
        const result = _router.parseUrl(`sign-in?${redirectURL}`);

        return result;
    }

    return true;
};