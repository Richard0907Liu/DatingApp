import { AuthService } from "./_services/auth.service";
import { Component, OnInit } from "@angular/core";
import { JwtHelperService } from "@auth0/angular-jwt";
import { User } from "./_models/user";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.css"]
})
export class AppComponent implements OnInit {
  title = "Dating-SPA";
  jwtHelper = new JwtHelperService();

  constructor(private authService: AuthService) {}

  ngOnInit() {
    // Sovle when refreshing, the login name in top-right panel is gone
    const token = localStorage.getItem("token");

    // Getting user info from localStorage
    // Assign "User" for this variable, and Use JSON.parse() to transfer user as an object
    const user: User = JSON.parse(localStorage.getItem("user"));

    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }

    if (user) {
      this.authService.currentUser = user;

      // For updating user nav photo
      // When user main photo is changed, the user photoUrl is changed too. So the nav photo is updated too.
      this.authService.changeMemberPhoto(user.photoUrl);
    }
  }
}
