import { MessagesComponent } from './../messages/messages.component';
import { Observable } from "rxjs";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { User } from "../_models/user";
import { PaginatedResult } from '../_models/Pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';

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

  //send up our Paging information to the server inside a HTTP prams object.
  // observe by default, it returns "body of request" but we can ask it to observe the "response (including response headers)" 
  // optional params.
  // By defualt in back end,  page=1 and itemsPerPage=10
  getUsers(page?, itemsPerPage?, userParams?, likesParam?): Observable<PaginatedResult<User[]>> {
    // because we created paginated result as a class we need to create a new instance
    const paginationResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    // Create new params or http params, put into headers
    let params = new HttpParams();

    if(page != null && itemsPerPage != null) {
      // Add params into URL like "?pageNumber=1&pageSize=10?
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    // if userParams exist
    if (userParams != null ) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    // Liker or Likee list, user can choose 'Likers' or 'Likees'
    // Need "resolver" to load a like list first in List page
    if(likesParam === 'Likers') {
      params = params.append('likers', 'true');
    }
    if(likesParam === 'Likees') {
      params = params.append('likees', 'true');
    }


    // observe: 'response', it will give us access to the full HTTP response and also pass the "params"
    // In get mehtod, we cannot get body info directly by observable, so use RXjs operator to get info from body.
    // go to chrome network users? => can see full request URL with params
    return this.http.get<User[]>(this.baseUrl + "users", { observe: 'response', params}) /*httpOptions); // Once JwtModule.forRoot() in app.module is set up, not need  httpOptions */
      .pipe(
        // map(), applies a given project function to each value emitted by the source Observable, and emits the resulting values as an Observable.
        // get body of response and pagination headers
        map( response => {
          paginationResult.result = response.body;

          if( response.headers.get('Pagination') != null) {
            // Convert response serial string into an JSON object
            paginationResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          console.log("paginationResult in userService::::", paginationResult);
          return paginationResult;
          // Need to update Observable<User[]> to Observable<Pagination<User[]>>
        })
      );
  }

  // Old getUsers()
  // Return type is User[] array
  // getUsers(): Observable<User[]> {
  //   // Need to tell our get mehtod to return what type will be returned, so add <User[]>
  //   return this.http.get<User[]>(this.baseUrl + "users"); // httpOptions); // Once JwtModule.forRoot() in app.module is set up, not need  httpOptions
  // }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + "users/" + id); //  httpOptions);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + "users/" + id, user);
  }

  // id => photo Id
  setMainPhoto(userId: number, id: number) {
    // Need to send body in POST method, but here is just {} empty object
    return this.http.post(
      this.baseUrl + "users/" + userId + "/photos/" + id + "/setMain",
      {}
    );
  }

  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + "users/" + userId + "/photos/" + id);
  }

  sendLike(id: number, recipientId: number){
    return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
  }

  // Like a method getUsers()
  getMessages(id: number, page?, itemsPerPage?, messageContainer?) {
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

    // Create params for Url params
    let params = new HttpParams();

    params = params.append('MessageContainer', messageContainer);

    if(page != null && itemsPerPage != null) {
      // Add params into URL like "?pageNumber=1&pageSize=10?
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    // Add {oberve: 'response', params} for Url params
    // Error, have to specify what type would be return, get() => get<Message[]>
    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {observe: 'response', params})
      .pipe(
        map(response => {
          paginatedResult.result = response.body;  
          if(response.headers.get('Pagination') !== null) {
            // get json string into an object
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }

          return paginatedResult;
        })
      )
  }

  // Get messages from both sides into one page
  getMessageThread(id: number, recipientId: number) {
    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId);
  }

  sendMessage(id: number, message: Message) {
    return this.http.post(this.baseUrl + 'users/' + id + '/messages', message);
  }

  deleteMessage(id: number, userId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id, {})
  }

  markAsRead(userId: number, messageId: number) {
    // Because not send anything back, so directly use "subscribe" here
    // We want this to be execute each time the message tab is opened 
    // when a user clicks on a particular message from their inbox or unread, go to member-message component
    this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + messageId + '/read', {})
      .subscribe();
  }

}
