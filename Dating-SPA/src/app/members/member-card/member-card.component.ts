import { AlertifyService } from './../../_services/alertify.service';
import { Component, OnInit, Input } from "@angular/core";
import { User } from "src/app/_models/user";
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';

// Beacuse this component is created inside a folder,
// so need to manually add this component into app.module
@Component({
  selector: "app-member-card",
  templateUrl: "./member-card.component.html",
  styleUrls: ["./member-card.component.css"]
})
export class MemberCardComponent implements OnInit {
  // member-card component is going to be a child component of our member-list component.
  // So bring in @Input()
  // user is from "member-list.html" (parent component) not from member-list.ts
  @Input() user: User;

  // use AuthSerivce to get user's Id
  constructor(private authService: AuthService, 
    private userService: UserService, private alertify: AlertifyService) {}

  ngOnInit() {}

  sendLike(id: number) {
    this.userService.sendLike(this.authService.decodedToken.nameid, id)
      .subscribe(data => {
        this.alertify.success('You have like: ' + this.user.knownAs)
      }, error => {
        this.alertify.error(error);  // Get error from back end
      });
  }

}
