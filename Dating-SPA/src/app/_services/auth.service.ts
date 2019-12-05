import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { map } from "rxjs/operators";

import { JwtHelperService } from "@auth0/angular-jwt";

@Injectable({
  providedIn: "root"
})
export class AuthService {
  baseUrl = "http://localhost:5000/api/auth/";

  // Use Auth to manage token
  // JWT constain expiration and other info
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  constructor(private http: HttpClient) {}

  login(model: any) {
    // After log in, need to receive "token" from response,
    // use "pipe" => allow us to chain RX.js operators to our request
    return this.http.post(this.baseUrl + "login", model).pipe(
      map((response: any) => {
        const user = response; // user will take the response including "token"
        if (user) {
          localStorage.setItem("token", user.token);
          // When token decoded, it contains {nameid: "3", unique_name: "bob", nbf: 1575488549, exp: 1575574949, iat: 1575488549}
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          console.log("this.decodedToken: ", this.decodedToken);
          // Use unique_name in Token to get username
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + "register", model);
  }

  // Manage the JWT
  // Not need to change inside NAV component
  loggedIn() {
    const token = localStorage.getItem("token");

    // return boolean, if expired or other problem, it returns false
    return !this.jwtHelper.isTokenExpired(token);
  }
}
