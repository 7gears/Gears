import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
    templateUrl  : './empty-page.component.html',
    standalone   : true,
    imports      : [RouterOutlet]
})
export class EmptyPageComponent
{
}