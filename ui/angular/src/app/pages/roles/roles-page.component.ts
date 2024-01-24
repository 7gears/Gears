import { Component, OnInit, inject } from '@angular/core';
import { Routes } from '@angular/router';
import { ApiClient, DeleteRoleRequest, IGetAllRolesResponse } from '../../core/api/ApiClient';
import { TableModule } from 'primeng/table';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ManageRoleComponent } from '../../features/roles/manage-role.component';
import { MenuModule } from 'primeng/menu';
import { MessageService } from 'primeng/api';
import { CommonModule } from '@angular/common';

@Component({
    templateUrl: './roles-page.component.html',
    standalone: true,
    imports: [TableModule, ToolbarModule, ButtonModule, MenuModule, CommonModule],
    providers: [DialogService]
})
export class RolesPageComponent implements OnInit {

    private readonly _apiClient = inject(ApiClient);
    private readonly _dialogService = inject(DialogService);
    private readonly _messageService = inject(MessageService);

    dialogRef: DynamicDialogRef | undefined;
    items!: IGetAllRolesResponse[];

    menuItems: any;
    menuSelectedItem: IGetAllRolesResponse | undefined;

    ngOnInit() {

        this.load();

        this.menuItems = [
            {
                label: 'Edit',
                icon: 'pi pi-fw pi-file-edit',
                command: () => this.update()
            },
            {
                label: 'Delete',
                icon: 'pi pi-fw pi-trash',
                command: () => this.delete()
            }
        ];
    }

    load() {
        this._apiClient.getAllRoles()
            .pipe()
            .subscribe({
                next: result => {
                    this.items = result;
                }
            });
    }

    add() {
        this.dialogRef = this._dialogService.open(ManageRoleComponent, {
            closable: false
        });

        this.dialogRef.onClose.subscribe(() => {
            this.load();
        });
    }

    update() {
        const id = this.menuSelectedItem?.id;
        if (!id)
            return;

        this.dialogRef = this._dialogService.open(ManageRoleComponent, {
            closable: false,
            data: { id: id }
        });
        this.dialogRef.onClose.subscribe(() => {
            this.load();
        });
    }

    delete() {
        const id = this.menuSelectedItem?.id;
        if (!id)
            return;

        this._apiClient.deleteRole(id)
            .pipe()
            .subscribe({
                next: () => {
                    this._messageService.add({ severity: "success", summary: "Success" });
                    this.load();
                },
                error: () => {
                    this._messageService.add({ severity: "error", summary: "Error" });
                }
            });
    }
}

export default [
    {
        path: '',
        component: RolesPageComponent,
    },
] as Routes;
