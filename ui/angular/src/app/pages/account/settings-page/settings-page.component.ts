import { Component } from '@angular/core';
import { ProfileComponent } from '../../../features/account/profile/profile.component';
import { Routes } from '@angular/router';
import { ChangePasswordComponent } from '../../../features/account/change-password/change-password.component';

@Component({
    templateUrl  : './settings-page.component.html',
    standalone   : true,
    imports      : [ProfileComponent, ChangePasswordComponent]
})
export class AccountSettingsPageComponent
{
}

export default [
    {
        path     : '',
        component: AccountSettingsPageComponent,
    },
] as Routes;
