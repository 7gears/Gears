import { Component, OnInit, inject } from '@angular/core';
import { Routes } from '@angular/router';
import { TableModule } from 'primeng/table';
import { ApiClient, DeleteUserRequest, IGetAllUsersResponse } from '../../core/api/ApiClient';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CommonModule } from '@angular/common';
import { MessageService } from 'primeng/api';
import { ManageUserComponent } from '../../features/users/manage-user.component';

@Component({
    templateUrl  : './users-page.component.html',
    standalone   : true,
    imports      : [TableModule, ToolbarModule, ButtonModule, MenuModule, CommonModule],
    providers    : [DialogService]
})
export class UsersPageComponent implements OnInit 
{
    private readonly _apiClient = inject(ApiClient);
    private readonly _dialogService = inject(DialogService);
    private readonly _messageService = inject(MessageService);

    dialogRef: DynamicDialogRef | undefined;
    items!: IGetAllUsersResponse[];

    menuItems: any;
    menuSelectedItem: IGetAllUsersResponse | undefined;

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
        this._apiClient.getAllUsers()
            .pipe()
            .subscribe({
                next: result => {
                    this.items = result;
                }
            });
    }

    add() {
        this.dialogRef = this._dialogService.open(ManageUserComponent, {
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

        this.dialogRef = this._dialogService.open(ManageUserComponent, {
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

        this._apiClient.deleteUser(id)
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
        path     : '',
        component: UsersPageComponent,
    },
] as Routes;
