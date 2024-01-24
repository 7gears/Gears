import { Component, inject } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { AuthService } from "../../../core/auth/auth.service";
import { ActivatedRoute, Router } from "@angular/router";
import { NgIf } from "@angular/common";
import { PasswordModule } from "primeng/password";

@Component({
    selector     : 'app-sign-in',
    templateUrl  : './sign-in.component.html',
    standalone   : true,
    imports      : [ReactiveFormsModule, NgIf, PasswordModule]
})
export class SignInComponent
{
    private readonly _authService = inject(AuthService);
    private readonly _router = inject(Router);
    private readonly _route = inject(ActivatedRoute)
    private readonly _fb = inject(FormBuilder)

    form!: FormGroup;
    loading = false;
    submitted = false;
    error?: string;

    constructor() {
        this.form = this._fb.nonNullable.group({
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required]]
        })
    }

    get f() { return this.form.controls; }

    onSubmit() {

        this.submitted = true;
        this.error = '';

        if (this.form.invalid) {
            return;
        }

        this.loading = true;

        const { email, password } = this.form.value;

        this._authService.signIn(email, password)
            .pipe()
            .subscribe({
                next: () => {
                    const redirectUrl = this._route.snapshot.queryParams['redirectURL'] || '/';
                    this._router.navigateByUrl(redirectUrl);
                },
                error: error => {
                    this.error = error;
                    this.loading = false;
                }
            });
    }
}