import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { MenuItem } from 'primeng/api';
import { AuthService } from '../../core/auth/auth.service';
import { MainMenuComponent } from '../main-menu/main-menu.component';

@Component({
    selector     : 'app-top-bar',
    templateUrl  : './top-bar-component.html',
    standalone   : true,
    imports      : [RouterOutlet, MenuModule, ButtonModule, MainMenuComponent]
})
export class TopBarComponent implements OnInit 
{
    private _authService = inject(AuthService);

    items: MenuItem[] | undefined;

    ngOnInit() {
        this.items = [
            {
                label: 'Settings',
                icon: 'pi pi-cog mr-3',
                url: '/account/settings'
            },
            {
                label: 'Sign out',
                icon: 'pi pi-power-off mr-3',
                command: () => {
                    this.signOut();
                }
            }
        ];
    }

    public signOut(): void {
        this._authService.signOut();
        window.location.href = '/';
    }
}