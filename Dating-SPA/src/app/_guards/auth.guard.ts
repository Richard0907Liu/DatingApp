import { AlertifyService } from "./../_services/alertify.service";

import { AuthService } from "./../_services/auth.service";
import { Injectable } from "@angular/core";
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
  Router
} from "@angular/router";
import { Observable } from "rxjs";

// user cannot just key in a certain path on URL to go to the protected page without logging

@Injectable({
  // This guard is already provided in root, so not need to add this to app.module file
  // but need to go to route.ts
  providedIn: "root"
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  // Original code
  // canActivate(): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
  //   return true;
  // }

  // Check user logg in or not
  // Just want to return boolean
  canActivate(): boolean {
    // If user log in, return ture, and then logged user can use protected content
    if (this.authService.loggedIn()) {
      return true;
    }

    // if user does not log in, then return an error
    this.alertify.error("You shall not pass!!!");
    // Return to home component
    this.router.navigate(["/home"]);
    return false;
  }
}
