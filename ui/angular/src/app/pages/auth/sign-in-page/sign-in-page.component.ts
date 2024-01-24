import { Component } from '@angular/core';
import { SignInComponent } from '../../../features/auth/sign-in/sign-in.component';
import { Routes } from '@angular/router';

@Component({
    template     : `<app-sign-in></app-sign-in>`,
    standalone   : true,
    imports      : [SignInComponent]
})
export class SignInPageComponent
{
}

export default [
    {
        path     : '',
        component: SignInPageComponent,
    },
] as Routes;