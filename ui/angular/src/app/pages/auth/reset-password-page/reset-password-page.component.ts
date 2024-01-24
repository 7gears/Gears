import { Component } from "@angular/core";
import { ResetPasswordComponent } from "../../../features/auth/reset-password/reset-password.component";
import { Routes } from "@angular/router";

@Component({
    template     : `<app-reset-password></app-reset-password>`,
    standalone   : true,
    imports      : [ResetPasswordComponent]
})
export class ResetPasswordPageComponent
{
}

export default [
    {
        path     : '',
        component: ResetPasswordPageComponent,
    },
] as Routes;
