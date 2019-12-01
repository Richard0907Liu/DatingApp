import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { map } from "rxjs/operators";

@Injectable({
  providedIn: "root"
})
export class AuthService {
  baseUrl = "http://localhost:5000/api/auth/";
  constructor(private http: HttpClient) {}

  login(model: any) {
    // After log in, need to receive "token" from response,
    // use "pipe" => allow us to chain RX.js operators to our request
    return this.http.post(this.baseUrl + "login", model).pipe(
      map((response: any) => {
        const user = response; // user will take the response including "token"
        if (user) {
          localStorage.setItem("token", user.token);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + "register", model);
  }
}
