import { AuthService } from "./../../_services/auth.service";
import { AlertifyService } from "./../../_services/alertify.service";
import { ActivatedRoute } from "@angular/router";
import { Component, OnInit, ViewChild, HostListener } from "@angular/core";
import { User } from "src/app/_models/user";
import { NgForm } from "@angular/forms";
import { UserService } from "src/app/_services/user.service";

@Component({
  selector: "app-member-edit",
  templateUrl: "./member-edit.component.html",
  styleUrls: ["./member-edit.component.css"]
})
export class MemberEditComponent implements OnInit {
  // Reset the <Information> propmt after saving
  @ViewChild("editForm", { static: true }) editForm: NgForm; // see member-edit.html

  // HostListener decorator, has got the ability to listen to our host in this case the browser
  // and take an action based on something that's happening inside our browser.
  // FOR prevetion, if close the browser, it show a dialogue first.
  @HostListener("window:beforeunload", ["$event"])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }
  /////

  user: User;
  constructor(
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private userService: UserService, // Also need AuthService because want to decode the token
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data["user"];
    });
  }

  updateUser() {
    // console.log(this.user); // FOR test
    this.userService
      .updateUser(this.authService.decodedToken.nameid, this.user)
      .subscribe(
        next => {
          this.alertify.success("Profile updated successfully");

          // There are form method in editForm
          // Reset form, mark as pristine and untouched. And then it wouldn't show Infomation propmt
          this.editForm.reset(this.user); // Add this.user, that reset the form and then show the new changes on form
        },
        error => {
          this.alertify.error(error);
        }
      );
  }
}
