import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TopBarComponent } from '../top-bar/top-bar-component';
import { ToastModule } from "primeng/toast";

@Component({
    templateUrl  : './main-page.component.html',
    standalone   : true,
    imports      : [RouterOutlet, TopBarComponent, ToastModule]
})
export class MainPageComponent
{
}