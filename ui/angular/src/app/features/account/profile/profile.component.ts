import { Component, OnInit, inject } from "@angular/core";
import { ApiClient, UpdateProfileRequest } from "../../../core/api/ApiClient";
import { FormBuilder, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { NgIf } from "@angular/common";
import { MessageService } from "primeng/api";
import { finalize } from "rxjs";

@Component({
    selector        : 'app-profile',
    templateUrl     : './profile.component.html',
    standalone      : true,
    imports         : [ReactiveFormsModule, NgIf]
})
export class ProfileComponent implements OnInit {

    private readonly _apiClient = inject(ApiClient);
    private readonly _fb = inject(FormBuilder)
    private readonly _messageService = inject(MessageService);

    form!: FormGroup;
    loading = false;
    submitted = false;

    constructor() {
        this.form = this._fb.nonNullable.group({
            email: [{
                value: '',
                disabled: true
              }],
            userName: [''],
            firstName: [''],
            lastName: [''],
            phoneNumber: ['']
        })
    }

    ngOnInit(): void {
        this._apiClient.getProfile()
            .pipe()
            .subscribe(
                response => {
                    this.form.setValue({
                        email: response.email,
                        userName: response.userName,
                        firstName: response.firstName,
                        lastName: response.lastName,
                        phoneNumber: response.phoneNumber
                    });
                });
    }

    onSubmit() {

        this.submitted = true;

        if (this.form.invalid) {
            return;
        }

        this.loading = true;

        const request = new UpdateProfileRequest(this.form.value);

        this._apiClient.updateProfile(request)
            .pipe(
                finalize(() => this.loading = false)
            )
            .subscribe({
                next: () => this._messageService.add({ severity: "success", summary: "Success" }),
                error: () => this._messageService.add({ severity: "error", summary: "Error" })
            });
    }
}