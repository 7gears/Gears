import { inject } from '@angular/core';
import { CanActivateChildFn, CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const NoAuthGuard: CanActivateFn | CanActivateChildFn = (_route, _state) =>
{
    const _authService = inject(AuthService);
    const _router = inject(Router);

    if(_authService.isAuthenticated) {

        return _router.parseUrl('');
    }

    return true;
};