import { inject, Injectable } from '@angular/core';
import { ApiClient, ConfirmEmailRequest, ForgotPasswordRequest, ResetPasswordRequest, SignInRequest, SignInResponse, SignUpRequest } from '../api/ApiClient';
import { map, Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  private _apiClient = inject(ApiClient);
  private _isAuthenticated = false;

  constructor() {
    const token = localStorage.getItem('token');
    if(token) {
      this._isAuthenticated = true;
    }
  }

  signIn(email: string, password: string): Observable<void> {
      if (this._isAuthenticated)
      {
        return throwError(() => new Error());
      }

      const request = new SignInRequest({email, password});

      return this._apiClient.signIn(request).pipe(
        map((response: SignInResponse) => {
          localStorage.setItem('token', response.token!);
          this._isAuthenticated = true;
        })
      );
  }

  signUp(email: string, password: string): Observable<void> {
    if (this._isAuthenticated)
    {
      return throwError(() => new Error());
    }

    const request = new SignUpRequest({email, password});

    return this._apiClient.signUp(request).pipe(
      map(() => {
      })
    );
  }

  signOut() {
    localStorage.removeItem('token');
    this._isAuthenticated = false;
  }

  confirmEmail(id: string, token: string): Observable<void> {
    if (this._isAuthenticated)
    {
      return throwError(() => new Error());
    }

    const request = new ConfirmEmailRequest({id, token});

    return this._apiClient.confirmEmail(request);
  }

  forgotPassword(email: string): Observable<void> {
    if (this._isAuthenticated)
    {
      return throwError(() => new Error());
    }
    const request = new ForgotPasswordRequest({email});

    return this._apiClient.forgotPassword(request);
  }

  resetPassword(id: string, token: string, password: string): Observable<void> {
    if (this._isAuthenticated)
    {
      return throwError(() => new Error());
    }

    const request = new ResetPasswordRequest({id, token, password});

    return this._apiClient.resetPassword(request);
  }
  
  get isAuthenticated(): boolean {
    return this._isAuthenticated;
  }

  get token(): string | null {
    return localStorage.getItem('token');
  }

}
