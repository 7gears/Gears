import { Component } from '@angular/core';
import { MenubarModule } from 'primeng/menubar';
import { MenuItem } from 'primeng/api';

@Component({
    selector     : 'app-main-menu',
    standalone   : true,
    templateUrl  : './main-menu.component.html',
    imports      : [MenubarModule]
})
export class MainMenuComponent
{
    items: MenuItem[] | undefined;

    constructor() {
        this.items = [
            {
                label: 'Identity Management',
                icon: 'pi pi-fw pi-users',
                items: [
                    {
                        label: 'Roles',
                        icon: 'pi pi-fw pi-sitemap',
                        routerLink: '/roles'
                    },
                    {
                        label: 'Users',
                        icon: 'pi pi-fw pi-user',
                        routerLink: '/users'
                    }
                ]
            }
        ];
    }
}