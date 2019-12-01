import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.css"]
})
export class HomeComponent implements OnInit {
  registerMode = false;
  values: any;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    //For TEST
    // this.getValues();
  }

  registerToggle() {
    // this.registerMode = !this.registerMode;
    this.registerMode = true; // cancelRegisterMode to control the other mode
  }

  // For TEST
  // getValues() {
  //   this.http.get("http://localhost:5000/api/values").subscribe(
  //     response => {
  //       this.values = response;
  //     },
  //     error => {
  //       console.log(error);
  //     }
  //   );
  // }

  cancelRegisterMode(regitsterMode: boolean) {
    this.registerMode = regitsterMode;
  }
}
