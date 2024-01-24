import { Component } from '@angular/core';
import { Routes } from '@angular/router';

@Component({
    templateUrl  : './confirm-email-required-page.component.html',
    standalone   : true,
    imports      : []
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
