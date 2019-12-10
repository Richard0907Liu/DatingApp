import { Observable } from "rxjs";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { User } from "../_models/user";

// Need to Add Bearer token into Headers
// const httpOptions = {  // Once JwtModule.forRoot() in app.module is set up, not need this property
//   headers: new HttpHeaders({
//     Authorization: "Bearer " + localStorage.getItem("token")
//   })
// };

@Injectable({
  providedIn: "root"
})
export class UserService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Return type is User[] array
  getUsers(): Observable<User[]> {
    // Need to tell our get mehtod to return what type will be returned, so add <User[]>
    return this.http.get<User[]>(this.baseUrl + "users"); // httpOptions); // Once JwtModule.forRoot() in app.module is set up, not need  httpOptions
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + "users/" + id); //  httpOptions);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + "users/" + id, user);
  }
}
