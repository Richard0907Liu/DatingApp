import { Component, OnInit, Input } from "@angular/core";
import { User } from "src/app/_models/user";

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

  constructor() {}

  ngOnInit() {}
}
