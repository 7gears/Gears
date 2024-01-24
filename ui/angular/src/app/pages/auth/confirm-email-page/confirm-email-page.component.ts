import { Component } from '@angular/core';
import { ConfirmEmailComponent } from '../../../features/auth/confirm-email/confirm-email.component';
import { Routes } from '@angular/router';

@Component({
    template     : `<app-confirm-email></app-confirm-email>`,
    standalone   : true,
    imports      : [ConfirmEmailComponent]
})
export class ConfirmEmailPageComponent
{
}

export default [
    {
        path     : '',
        component: ConfirmEmailPageComponent,
    },
] as Routes;