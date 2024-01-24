import { Component, OnInit, inject } from "@angular/core";
import { AuthService } from "../../../core/auth/auth.service";
import { ActivatedRoute } from "@angular/router";

enum OperationStatus {
    Processing,
    Successed,
    Failed
}

@Component({
    selector     : 'app-confirm-email',
    templateUrl  : './confirm-email.component.html',
    standalone   : true
})
export class ConfirmEmailComponent implements OnInit
{
    private readonly _authService = inject(AuthService);
    private readonly _route = inject(ActivatedRoute)

    OperationStatus = OperationStatus;
    operationStatus = OperationStatus.Processing;

    ngOnInit(): void {
        const id = this._route.snapshot.queryParams['Id'];
        const token = this._route.snapshot.queryParams['Token'];

        this._authService.confirmEmail(id, token)
            .pipe()
            .subscribe({
                next: () => {
                    this.operationStatus = OperationStatus.Successed
                },
                error: () => {
                    this.operationStatus = OperationStatus.Failed
                }
            });
    }
}