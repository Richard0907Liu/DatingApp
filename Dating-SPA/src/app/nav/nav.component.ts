import { AlertifyService } from "./../_services/alertify.service";
import { Component, OnInit } from "@angular/core";
import { AuthService } from "../_services/auth.service";
import { Router } from "@angular/router";

@Component({
  selector: "app-nav",
  templateUrl: "./nav.component.html",
  styleUrls: ["./nav.component.css"]
})
export class NavComponent implements OnInit {
  model: any = {};

  // For calling photoUrl from BehaviorSubject
  photoUrl: string;

  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router
  ) {}

  ngOnInit() {
    // photoUrl from currentPhotoUrl
    this.authService.currentPhotoUrl.subscribe(
      photoUrl => (this.photoUrl = photoUrl)
    );
  }

  login() {
    //console.log(this.model);
    this.authService.login(this.model).subscribe(
      next => {
        // console.log("Logged in successfully");  // Just show on the console
        this.alertify.success("Logged in successfully");
        // not use router.navigate in here, use that in "complete method"
      },
      error => {
        // After setting gobal exception in Back-end, can get a specific error from certain API
        //console.log(error);
        this.alertify.error(error);
      },
      () => {
        // Want to navigate to ceratin path after logged
        this.router.navigate(["/members"]);
      }
    );
  }

  loggedIn() {
    // Use JwtHelperService
    // token is checked for its expiration
    return this.authService.loggedIn();

    /*  Old 
    const token = localStorage.getItem("token");
    // !! => "double exclamation mark" to return "true or false" not token
    return !!token; // If token is null => return false, if token is available return true
    */
  }

  // Remove token when log out
  loggout() {
    localStorage.removeItem("token");
    // console.log("Logged out");

    localStorage.removeItem("user");
    this.authService.decodedToken = null;
    this.authService.currentUser = null;

    this.alertify.message("Logged out");

    // Navigate to Home page after logging out
    this.router.navigate(["/home"]);
  }
}
