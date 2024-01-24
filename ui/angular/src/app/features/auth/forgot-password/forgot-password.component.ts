import { Component, inject } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { NgIf } from "@angular/common";
import { AuthService } from "../../../core/auth/auth.service";

enum OperationStatus {
    Processing,
    Successed
}

@Component({
    selector     : 'app-forgot-password',
    templateUrl  : './forgot-password.component.html',
    standalone   : true,
    imports      : [ReactiveFormsModule, NgIf]
})
export class ForgotPasswordComponent
{
    private readonly _authService = inject(AuthService);
    private readonly _fb = inject(FormBuilder)

    OperationStatus = OperationStatus;
    operationStatus = OperationStatus.Processing;

    form!: FormGroup;
    loading = false;
    submitted = false;

    constructor() {
        this.form = this._fb.nonNullable.group({
            email: ['', [Validators.required, Validators.email]]
        })
    }

    get f() { return this.form.controls; }

    onSubmit() {

        this.submitted = true;

        if (this.form.invalid) {
            return;
        }

        this.loading = true;

        const { email } = this.form.value;

        this._authService.forgotPassword(email)
            .pipe()
            .subscribe({
                next: () => {
                    this.loading = false;
                    this.operationStatus = OperationStatus.Successed;
                }
            });

    }
}