import { AuthService } from './../_services/auth.service';
import { AlertifyService } from "../_services/alertify.service";
import { UserService } from "../_services/user.service";
import { Injectable } from "@angular/core";
import { Resolve, Router, ActivatedRouteSnapshot } from "@angular/router";
import { Observable, of } from "rxjs";
import { catchError } from "rxjs/operators";
import { Message } from '../_models/message';

// To solve not using "?" => safe navigation operator. make user variables and component work together, not component works first

@Injectable()
export class MessagesResolver implements Resolve<Message[]> {
  // set up variables for pagination 
  pageNumber = 1;
  pageSize = 5;
  messageContainer = 'Unread';

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  // make resolve as Observable and returned type is User
  // when we use a resolve this automatically subscribes to the method so we don't need to subscribe
  resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
    
    // need to get the idea from the root parameters
     // go to chrome network users? => can see full request URL with params
    return this.userService.getMessages(this.authService.decodedToken.nameid, 
        this.pageNumber, this.pageSize, this.messageContainer)
      .pipe(
      // not need to subscribe but need to catch any error
      catchError(error => {
        this.alertify.error("Problem retreiving messages");
        // give error and then get then bakc to the home page
        //this.router.navigate(["/members"]); if go back to /members, it would repeat going the same page
        this.router.navigate(["/home"]);

        // return null type
        return of(null);
      })
    );
  }
}
