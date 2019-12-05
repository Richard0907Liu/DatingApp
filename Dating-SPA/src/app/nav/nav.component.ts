import { AlertifyService } from "./../_services/alertify.service";
import { Component, OnInit } from "@angular/core";
import { AuthService } from "../_services/auth.service";

@Component({
  selector: "app-nav",
  templateUrl: "./nav.component.html",
  styleUrls: ["./nav.component.css"]
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(
    public authService: AuthService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {}

  login() {
    //console.log(this.model);
    this.authService.login(this.model).subscribe(
      next => {
        // console.log("Logged in successfully");  // Just show on the console
        this.alertify.success("Logged in successfully");
      },
      error => {
        // After setting gobal exception in Back-end, can get a specific error from certain API
        //console.log(error);
        this.alertify.error(error);
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
    this.alertify.message("Logged out");
  }
}
