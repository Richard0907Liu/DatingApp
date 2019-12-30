import { AlertifyService } from "./../../_services/alertify.service";
import { UserService } from "./../../_services/user.service";
import { Component, OnInit, ViewChild } from "@angular/core";
import { User } from "src/app/_models/user";
import { ActivatedRoute } from "@angular/router";
import {
  NgxGalleryOptions,
  NgxGalleryImage,
  NgxGalleryAnimation
} from "ngx-gallery";
import { TabsetComponent } from '../../../../node_modules/ngx-bootstrap';

@Component({
  selector: "app-member-detail",
  templateUrl: "./member-detail.component.html",
  styleUrls: ["./member-detail.component.css"]
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;


  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  // DI ActivatedRoute for getting Id in members/id
  // ## Because when the "route is activated" we'll have access to that particular parameter.
  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    // Get data form "route resolver"
    //  this.loadUser(); //not user this, it cause geting variable  and loading component are not work properly
    this.route.data.subscribe(data => {
      // MemberDetailResolver send data into user from the root and then get data by data['user']
      this.user = data["user"]; // Doing so, the variable can send into root and use by certain url
    });

    // Can subscribe queryParam to get tabId. When link to here from message page, it directly go to message tab
    this.route.queryParams.subscribe( params => {
      const selectedTab = params['tab'];
      this.memberTabs.tabs[selectedTab > 0 ? selectedTab : 0].active = true;
    })
    
    this.galleryOptions = [
      {
        width: "500px",
        height: "500px",
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];

    this.galleryImages = this.getItems();
  }

  getItems() {
    const imageUrls = [];
    for (const photo of this.user.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
        description: photo.description
      });
    }
    return imageUrls;
  }

  // TabsetComponent 
  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }



  // members/id
  // loadUser() {
  //   // Need to get id from Url, so need dependency injection of "ActivatedRoute"
  //   // url is a string, need the id as a number, SO add "+" in front of the parameter
  //   this.userService.getUser(+this.route.snapshot.params["id"]).subscribe(
  //     (user: User) => {
  //       this.user = user;
  //     },
  //     error => {
  //       this.alertify.error(error);
  //     }
  //   );
  // }
}
