import { Component } from '@angular/core';
import { SignUpComponent } from '../../../features/auth/sign-up/sign-up.component';
import { Routes } from '@angular/router';

@Component({
    template     : `<app-sign-up></app-sign-up>`,
    standalone   : true,
    imports      : [SignUpComponent]
})
export class SignUpPageComponent
{
}

export default [
    {
        path     : '',
        component: SignUpPageComponent,
    },
] as Routes;
