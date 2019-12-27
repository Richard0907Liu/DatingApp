import { AlertifyService } from './../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from './../_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { Component, OnInit } from '@angular/core';
import { User } from '../_models/user';
import { Pagination, PaginatedResult } from '../_models/Pagination';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  likesParam: string;

  constructor(private authService: AuthService, private userService: UserService,
    private route: ActivatedRoute, private alertify: AlertifyService ) { }

  ngOnInit() {
    this.route.data.subscribe( data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    })

    this.likesParam = 'Likers';
  }

  loadUsers() {
    this.userService
      .getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.likesParam)
      .subscribe((res: PaginatedResult<User[]>) => {
        this.users = res.result;
        this.pagination = res.pagination;
      },
      error => {
        this.alertify.error(error);
      }
    );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    console.log(this.pagination.currentPage); // just show clicked page in

    this.loadUsers();
  }

}
