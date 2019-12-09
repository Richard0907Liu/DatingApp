import { AlertifyService } from "./../_services/alertify.service";
import { UserService } from "./../_services/user.service";
import { Injectable } from "@angular/core";
import { User } from "../_models/user";
import { Resolve, Router, ActivatedRouteSnapshot } from "@angular/router";
import { Observable, of } from "rxjs";
import { catchError } from "rxjs/operators";

// To solve not using "?" => safe navigation operator. make user variables and component work together, not component works first

@Injectable()
export class MemberDetailResolver implements Resolve<User> {
  constructor(
    private userService: UserService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  // make resolve as Observable and returned type is User
  // when we use a resolve this automatically subscribes to the method so we don't need to subscribe
  resolve(route: ActivatedRouteSnapshot): Observable<User> {
    // need to get the idea from the root parameters
    return this.userService.getUser(route.params["id"]).pipe(
      // not need to subscribe but need to catch any error
      catchError(error => {
        this.alertify.error("Problem retreiving data");
        // give error and then get then bakc to the members page
        this.router.navigate(["/members"]);

        // return null type
        return of(null);
      })
    );
  }
}
