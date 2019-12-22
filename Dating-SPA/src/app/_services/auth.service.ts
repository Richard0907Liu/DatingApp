import { Photo } from "src/app/_models/photo";
import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { map } from "rxjs/operators";

import { JwtHelperService } from "@auth0/angular-jwt";
import { environment } from "src/environments/environment";
import { User } from "../_models/user";

// BehaviorSubject make any to any component communicate
import { BehaviorSubject } from "rxjs";

@Injectable({
  providedIn: "root"
})
export class AuthService {
  // baseUrl = "http://localhost:5000/api/auth/"; old one
  baseUrl = environment.apiUrl + "auth/";

  // Use Auth to manage token
  // JWT constain expiration and other info
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;

  // BehaviorSubject photoUrl would be a string, give initial values
  photoUrl = new BehaviorSubject<string>("../../assets/user.png"); // now photoUrl is "asObservable" type.
  currentPhotoUrl = this.photoUrl.asObservable(); // make photoUrl can be "observable"

  constructor(private http: HttpClient) {}

  // When changing main photo, it would change memeber photo (in different components)
  // When user loging, want to call this method.
  changeMemberPhoto(photoUrl: string) {
    // next(), update the photoUrl with new photoUrl, not use '../../assets/user.png'
    this.photoUrl.next(photoUrl);
  }

  login(model: any) {
    // After log in, need to receive "token" from response,
    // use "pipe" => allow us to chain RX.js operators to our request
    return this.http.post(this.baseUrl + "login", model).pipe(
      map((response: any) => {
        const user = response; // user will take the response including "token"
        if (user) {
          localStorage.setItem("token", user.token);
          // Save user info into localStorage
          localStorage.setItem("user", JSON.stringify(user.user));
          // When token decoded, it contains {nameid: "3", unique_name: "bob", nbf: 1575488549, exp: 1575574949, iat: 1575488549}
          this.decodedToken = this.jwtHelper.decodeToken(user.token);

          // Want these user values to be retrieved when an application first loads up
          this.currentUser = user.user;

          console.log("this.decodedToken: ", this.decodedToken);
          // Use unique_name in Token to get username

          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  register(user: User) {
    return this.http.post(this.baseUrl + "register", user);
  }

  // Manage the JWT
  // Not need to change inside NAV component
  loggedIn() {
    const token = localStorage.getItem("token");

    // return boolean, if expired or other problem, it returns false
    return !this.jwtHelper.isTokenExpired(token);
  }
}
