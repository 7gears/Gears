import { Routes } from '@angular/router';
import { MainPageComponent } from './layout/main/main-page.component';
import { EmptyPageComponent } from './layout/empty-page/empty-page.component';
import { AuthGuard } from './core/auth/auth.guard';
import { NoAuthGuard } from './core/auth/no-auth.guard';
import { NotFoundPageComponent } from './layout/not-found-page/not-found-page.component';

export const routes: Routes = [
    {
        path: '',
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        component: MainPageComponent,
        children: [
            {path: 'account/settings', loadChildren: () => import('./pages/account/settings-page/settings-page.component')},
            {path: 'roles', loadChildren: () => import('./pages/roles/roles-page.component')},
            {path: 'users', loadChildren: () => import('./pages/users/users-page.component')},
        ]
    },
    {
        canActivate: [NoAuthGuard],
        canActivateChild: [NoAuthGuard],
        path: '',
        component: EmptyPageComponent,
        children: [
            {path: 'sign-in', loadChildren: () => import('./pages/auth/sign-in-page/sign-in-page.component')},
            {path: 'sign-up', loadChildren: () => import('./pages/auth/sign-up-page/sign-up-page.component')},
            {path: 'confirm-email-required', loadChildren: () => import('./pages/auth/confirm-email-required-page/confirm-email-required-page.component')},
            {path: 'confirm-email', loadChildren: () => import('./pages/auth/confirm-email-page/confirm-email-page.component')},
            {path: 'forgot-password', loadChildren: () => import('./pages/auth/forgot-password-page/forgot-password-page.component')},
            {path: 'reset-password', loadChildren: () => import('./pages/auth/reset-password-page/reset-password-page.component')}
        ]
    },
    {
        path: '**',
        component: NotFoundPageComponent
    }
];
