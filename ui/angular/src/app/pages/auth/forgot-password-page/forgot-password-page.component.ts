import { Component } from "@angular/core";
import { ForgotPasswordComponent } from "../../../features/auth/forgot-password/forgot-password.component";
import { Routes } from "@angular/router";

@Component({
    template     : `<app-forgot-password></app-forgot-password>`,
    standalone   : true,
    imports      : [ForgotPasswordComponent]
})
export class ForgotPasswordPageComponent
{
}

export default [
    {
        path     : '',
        component: ForgotPasswordPageComponent,
    },
] as Routes;
