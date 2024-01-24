import { Component, OnInit, inject } from '@angular/core';
import { AddUserRequest, ApiClient, GetAllRolesResponse, UpdateUserRequest } from '../../core/api/ApiClient';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageService, TreeNode } from 'primeng/api';
import { finalize } from 'rxjs';
import { TabViewModule } from 'primeng/tabview';
import { TreeModule } from 'primeng/tree';

@Component({
    templateUrl: './manage-user.component.html',
    standalone: true,
    imports: [ReactiveFormsModule, TabViewModule, TreeModule]
})
export class ManageUserComponent implements OnInit {

    private readonly _apiClient = inject(ApiClient);
    private readonly _fb = inject(FormBuilder)
    private readonly _dialogRef = inject(DynamicDialogRef);
    private readonly _dialogConfig = inject(DynamicDialogConfig);
    private readonly _messageService = inject(MessageService);

    id: string | null = null;
    form!: FormGroup;
    loading = false;
    submitted = false;

    roles: TreeNode[] = [];
    selectedRoles: TreeNode[] | any = [];

    ngOnInit(): void {
        this.id = this._dialogConfig.data?.id;

        this.form = this._fb.nonNullable.group({
            email: ['', [Validators.required]],
            userName: [''],
            firstName: [''],
            lastName: [''],
            phoneNumber: [''],
            isActive: [true]
        });

        this.loadRoles();
        this.loadUser();
    }

    loadUser() {

        if (!this.id) {
            return;
        }

        this._apiClient.getUser(this.id)
            .pipe()
            .subscribe(
                x => {
                    this.form.setValue({
                        email: x.email,
                        userName: x.userName,
                        firstName: x.firstName,
                        lastName: x.lastName,
                        phoneNumber: x.phoneNumber,
                        isActive: x.isActive
                    });

                    if (x.roleIds) {
                        this.preselectNodes(this.roles, this.selectedRoles, x.roleIds);
                    }
                }
            )
    }

    loadRoles() {
        this._apiClient.getAllRoles()
            .pipe()
            .subscribe(x => this.processRoles(x));
    }

    processRoles(roles: GetAllRolesResponse[]){
        for (const role of roles) {
            const node = {
                key: role.id,
                label: role.name,
                selectable: !role.isDefault
            }
            this.roles.push(node);
            if(role.isDefault) {
                this.selectedRoles.push(node);
            }
        }
    }

    add() {
        const request = new AddUserRequest(this.form.value);

        if (this.selectedRoles) {
            request.roleIds = this.selectedRoles
                .map((x: { key: any; }) => String(x.key));
        }

        this._apiClient.addUser(request)
            .pipe(
                finalize(() => this.loading = false)
            )
            .subscribe({
                next: () => {
                    this._messageService.add({ severity: "success", summary: "Success" });
                    this.onClose();
                },
                error: () => {
                    this._messageService.add({ severity: "error", summary: "Error" });
                }
            });
    }

    update() {
        const request = new UpdateUserRequest(this.form.value);
        request.roleIds = this.selectedRoleNames;

        this._apiClient.updateUser(request, this.id!)
            .pipe(
                finalize(() => this.loading = false)
            )
            .subscribe({
                next: () => {
                    this._messageService.add({ severity: "success", summary: "Success" });
                    this.onClose();
                },
                error: () => {
                    this._messageService.add({ severity: "error", summary: "Error" });
                }
            });
    }

    onSubmit() {

        this.submitted = true;

        if (this.form.invalid) {
            return;
        }

        this.loading = true;

        if (this.id) {
            this.update();
        }
        else {
            this.add();
        }
    }

    onClose() {
        this._dialogRef.close();
    }

    get selectedRoleNames() {

        if (!this.selectedRoles)
            return null;

        return this.selectedRoles
            .filter((x: { children: any; }) => !x.children)
            .map((x: { key: string; }) => x.key)
    }

    preselectNodes(nodes: TreeNode[], checkedNodes: TreeNode[], keys: string[]) {
        for (const node of nodes) {

            if (node.children) {
                this.preselectNodes(node.children, checkedNodes, keys);
            }

            if (keys.includes(node.key!)) {
                checkedNodes.push(node);
            }

            if (node.children) {
                const count = node.children.length;
                if (count === 0) {
                    continue;
                }

                let selectedChildrenCount = 0;
                for (const child of node.children) {
                    if (keys.includes(child.key!)) {
                        selectedChildrenCount++;
                    }
                }
                if (selectedChildrenCount === 0) {
                    continue;
                }

                if (count === selectedChildrenCount) {
                    checkedNodes.push(node);
                }
                else {
                    node.partialSelected = true;
                }
            }
        }
    }
}