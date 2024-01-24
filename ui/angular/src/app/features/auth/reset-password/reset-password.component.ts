import { Component, ViewEncapsulation, inject } from "@angular/core";
import { AuthService } from "../../../core/auth/auth.service";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { NgIf } from "@angular/common";
import { ActivatedRoute } from "@angular/router";
import { PasswordModule } from 'primeng/password';

enum OperationStatus {
    Processing,
    Successed,
    Failed
}

@Component({
    selector     : 'app-reset-password',
    templateUrl  : './reset-password.component.html',
    encapsulation: ViewEncapsulation.None,
    standalone   : true,
    imports      : [ReactiveFormsModule, NgIf, PasswordModule]
})
export class ResetPasswordComponent {

    private readonly _authService = inject(AuthService);
    private readonly _route = inject(ActivatedRoute);
    private readonly _fb = inject(FormBuilder)

    OperationStatus = OperationStatus;
    operationStatus = OperationStatus.Processing;

    form!: FormGroup;
    loading = false;
    submitted = false;

    constructor() {
        this.form = this._fb.nonNullable.group({
            password: ['', [Validators.required]]
        })
    }

    get f() { return this.form.controls; }

    onSubmit() {

        this.submitted = true;

        if (this.form.invalid) {
            return;
        }

        this.loading = true;

        const token = this._route.snapshot.queryParams['Token'];
        const id = this._route.snapshot.queryParams['Id'];
        const { password } = this.form.value;

        this._authService.resetPassword(id, token, password)
            .pipe()
            .subscribe({
                next: () => {
                    this.loading = false;
                    this.operationStatus = OperationStatus.Successed;
                }
            });
    }
}
