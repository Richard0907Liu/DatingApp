import { AlertifyService } from "./../_services/alertify.service";
import { AuthService } from "./../_services/auth.service";
import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";

@Component({
  selector: "app-register",
  templateUrl: "./register.component.html",
  styleUrls: ["./register.component.css"]
})
export class RegisterComponent implements OnInit {
  // Use Input() to get variable from parent (home html)
  @Input() valuesFromHome: any;

  // Use Output() to pass variable to parent component
  // Output property emits "events"
  @Output() cancelRegister = new EventEmitter(); // This EventEmitter from core

  model: any = {};

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {}

  register() {
    //console.log(this.model);
    // not recieve any response, so use () =>
    this.authService.register(this.model).subscribe(
      () => {
        // console.log("registration succcessful");
        this.alertify.success("registration succcessful");
      },
      error => {
        this.alertify.error(error);
        //console.log(error);
        // console.log("error.error.errors: ", error.error.errors);
      }
    );
  }

  cancel() {
    // emit something from this method, can be an object, values etc
    this.cancelRegister.emit(false); // this case, emit "false"
    console.log("cancelled!");
  }
}
