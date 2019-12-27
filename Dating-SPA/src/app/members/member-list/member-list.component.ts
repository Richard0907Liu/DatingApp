import { AlertifyService } from "../../_services/alertify.service";
import { UserService } from "../../_services/user.service";
import { Component, OnInit } from "@angular/core";
import { User } from "../../_models/user";
import { ActivatedRoute } from "@angular/router";
import { Pagination, PaginatedResult } from './../../_models/Pagination';

@Component({
  selector: "app-member-list",
  templateUrl: "./member-list.component.html",
  styleUrls: ["./member-list.component.css"]
})
export class MemberListComponent implements OnInit {
  users: User[];

  // For filtering, store user information inside localStorage
  // Json object
  user: User = JSON.parse(localStorage.getItem('user')); // user information already store in authService


  pagination: Pagination; // pass pagination properties into html
  // Add some filter properties
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}];
  userParams: any = {}; // empty object, have to add "any" for empty object;

  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    
    // MemberListResolver send data into users from the 'root resolver" and then get data by data['users']
    this.route.data.subscribe(data => {
      // For pagination, add "result"
      this.users = data["users"].result;  // "users" from  resolve: { users: MemberListResolver } in routes.cs
      this.pagination = data["users"].pagination;

      //this.users = data["users"]; // old one, not include pagination
    });
    //this.loadUsers(); // Old

    // set some initial parameters for userParams names like API
    console.log('user in member-list component: ', this.user);
    // get gender from user
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.orderBy = "lastActive";
   
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    console.log(this.pagination.currentPage); // just show clicked page in

    this.loadUsers();
  }

  // Set up reset filter
  resetFilters() {
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.loadUsers();
  }

  // we're going to need to load the next batch of users
  // need to add params into getUsers
  loadUsers() {
    this.userService
      .getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
      .subscribe((res: PaginatedResult<User[]>) => {
        this.users = res.result;
        this.pagination = res.pagination;
      },
      error => {
        this.alertify.error(error);
      }
    );
  }

  // Old one, no pagiation
  // loadUsers() {
  //   this.userService.getUsers().subscribe(
  //     (users: User[]) => {
  //       this.users = users;
  //     },
  //     error => {
  //       this.alertify.error(error);
  //     }
  //   );
  // }
}
