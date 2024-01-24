import { Component, inject } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { ApiClient, ChangePasswordRequest } from "../../../core/api/ApiClient";
import { MessageService } from "primeng/api";
import { PasswordModule } from "primeng/password";
import { NgIf } from "@angular/common";
import { finalize } from "rxjs";

@Component({
    selector        : 'app-change-password',
    templateUrl     : './change-password.component.html',
    standalone      : true,
    imports         : [ReactiveFormsModule, NgIf, PasswordModule]
})
export class ChangePasswordComponent
{
    private readonly _apiClient = inject(ApiClient);
    private readonly _fb = inject(FormBuilder)
    private readonly _messageService = inject(MessageService);

    form!: FormGroup;
    loading = false;
    submitted = false;

    constructor() {
        this.form = this._fb.nonNullable.group({
            password: ['', [Validators.required]],
            newPassword: ['', [Validators.required]]
        })
    }

    get f() { return this.form.controls; }

    onSubmit() {

        this.submitted = true;

        if (this.form.invalid) {
            return;
        }

        this.loading = true;

        const request = new ChangePasswordRequest(this.form.value);

        this._apiClient.changePassword(request)
            .pipe(
                finalize(() => this.loading = false)
            )
            .subscribe({
                next: () => this._messageService.add({ severity: "success", summary: "Success" }),
                error: () => { this._messageService.add({ severity: "error", summary: "Error" }) }
            });
    }
}
