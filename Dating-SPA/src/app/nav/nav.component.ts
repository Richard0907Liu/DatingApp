import { Component, OnInit } from "@angular/core";
import { AuthService } from "../_services/auth.service";

@Component({
  selector: "app-nav",
  templateUrl: "./nav.component.html",
  styleUrls: ["./nav.component.css"]
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(private authService: AuthService) {}

  ngOnInit() {}

  login() {
    //console.log(this.model);
    this.authService.login(this.model).subscribe(
      next => {
        console.log("Logged in successfully");
      },
      error => {
        // After setting gobal exception in Back-end, can get a specific error from certain API
        console.log(error);
      }
    );
  }

  loggedIn() {
    const token = localStorage.getItem("token");
    // !! => "double exclamation mark" to return "true or false" not token
    return !!token; // If token is null => return false, if token is available return true
  }

  // Remove token when log out
  loggout() {
    localStorage.removeItem("token");
    console.log("Logged out");
  }
}
