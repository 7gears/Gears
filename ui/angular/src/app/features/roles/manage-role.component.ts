import { Component, OnInit, inject } from '@angular/core';
import { AddRoleRequest, ApiClient, PermissionGroup, UpdateRoleRequest } from '../../core/api/ApiClient';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageService, TreeNode } from 'primeng/api';
import { finalize } from 'rxjs';
import { TabViewModule } from 'primeng/tabview';
import { TreeModule } from 'primeng/tree';

@Component({
    templateUrl: './manage-role.component.html',
    standalone: true,
    imports: [ReactiveFormsModule, TabViewModule, TreeModule]
})
export class ManageRoleComponent implements OnInit {

    private readonly _apiClient = inject(ApiClient);
    private readonly _fb = inject(FormBuilder)
    private readonly _dialogRef = inject(DynamicDialogRef);
    private readonly _dialogConfig = inject(DynamicDialogConfig);
    private readonly _messageService = inject(MessageService);

    id: string | null = null;
    form!: FormGroup;
    loading = false;
    submitted = false;

    permissions: TreeNode[] = [];
    selectedPermissions: TreeNode[] | any = [];

    ngOnInit(): void {
        this.id = this._dialogConfig.data?.id;

        this.form = this._fb.nonNullable.group({
            name: ['', [Validators.required]],
            description: [''],
            isDefault: [false]
        });

        this.loadPermissions();
        this.loadRole();
    }

    loadPermissions() {
        this._apiClient.getAllPermissions()
            .pipe()
            .subscribe(
                x => {
                    this.permissions = this.transformToUIPermissionsTree(x);
                }
            );
    }

    loadRole() {
        if (!this.id) {
            return;
        }

        this._apiClient.getRole(this.id)
            .pipe()
            .subscribe(
                x => {
                    this.form.setValue({
                        name: x.name,
                        description: x.description,
                        isDefault: x.isDefault
                    });

                    if (x.permissions) {
                        this.preselectNodes(this.permissions, this.selectedPermissions, x.permissions);
                    }
                }
            )
    }

    add() {
        const request = new AddRoleRequest(this.form.value);
        if (this.selectedPermissions) {
            request.permissions = this.selectedPermissions
                .filter((x: { children: any; }) => !x.children)
                .map((x: { key: any; }) => String(x.key));
        }

        this._apiClient.addRole(request)
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
        const request = new UpdateRoleRequest(this.form.value);
        request.permissions = this.selectedPermissionsNames;

        this._apiClient.updateRole(request, this.id!)
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

    get selectedPermissionsNames() {

        if (!this.selectedPermissions)
            return null;

        return this.selectedPermissions
            .filter((x: { children: any; }) => !x.children)
            .map((x: { key: string; }) => x.key)
    }

    transformToUIPermissionsTree(input: PermissionGroup[]) {
        return input.map(x => ({
            key: x.groupId,
            label: x.groupName,
            children: x.items?.map((permission) => ({
                key: permission.id,
                label: permission.visibleName
            }))
        }));
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