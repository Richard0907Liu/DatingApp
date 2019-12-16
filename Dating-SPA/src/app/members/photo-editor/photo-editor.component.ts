import { AlertifyService } from "./../../_services/alertify.service";
import { Component, OnInit, Input, EventEmitter, Output } from "@angular/core";
import { Photo } from "src/app/_models/photo";
import { FileUploader } from "ng2-file-upload";
import { environment } from "src/environments/environment"; // Make sure import from 'environment/environment', not 'environment/environment.prod'
import { AuthService } from "./../../_services/auth.service";
import { UserService } from "./../../_services/user.service";

@Component({
  selector: "app-photo-editor",
  templateUrl: "./photo-editor.component.html",
  styleUrls: ["./photo-editor.component.css"]
})

// this component is going to be a child component of our member edit component.
export class PhotoEditorComponent implements OnInit {
  // bringing something from a parent component we're going to use an input decorator.
  @Input() photos: Photo[];

  // Need to send changed main photo info to parent component
  // this is going to be set to a new event limiter because "output properties emit events"
  // in this case we're going to output a string because we want to output the photo URL
  @Output() getMemberPhotoChange = new EventEmitter<string>(); // Next emit photo URL

  uploader: FileUploader; // initialize our uploader in a separate method
  hasBaseDropZoneOver = false;
  // hasAnotherDropZoneOver = false; // not need this
  baseUrl = environment.apiUrl;

  // Want to immediately reponse when changing a main botton.
  // Need to replicate the behaviour's happening on our API inside.
  currentMain: Photo;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      // this needs to matche the path and userId from authService
      url:
        this.baseUrl +
        "users/" +
        this.authService.decodedToken.nameid +
        "/photos",
      // Need to pass auth token, from authToken FileUploader class
      authToken: "Bearer " + localStorage.getItem("token"),
      isHTML5: true,
      allowedFileType: ["image"],
      removeAfterUpload: true, // remvoe image after uploading
      autoUpload: false, // we want to click a button to send file
      maxFileSize: 10 * 1024 * 1024 // 10 megabytes max size here. can set size whatever you want
    });

    // because we're not sending up our file with credentials, we can pass error that without credential (404)
    this.uploader.onAfterAddingFile = file => {
      file.withCredentials = false;
    };

    // Use onSuccessItem of FileUploader to immediately see all photos after uploading photos
    // pass required params
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response); // response is a string, user JSON.parse to make response as a json object for Photo object
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };
        // push "photo" into "photos: photo[]"
        this.photos.push(photo);
      }
    };
  }

  setMainPhoto(photo: Photo) {
    // params userId and photoId
    this.userService
      .setMainPhoto(this.authService.decodedToken.nameid, photo.id)
      .subscribe(
        () => {
          //console.log('Suceesfully set to main');

          // Using the array filter method to provide instane feedback in the SPA
          // return a copy of the photos array but filter out anything dosen't match what we're looking for
          // add [0], because it's returning an array that means we need to specify the element of the array
          // there is just one main photo, so add [0]
          this.currentMain = this.photos.filter(p => p.isMain === true)[0];
          this.currentMain.isMain = false;
          photo.isMain = true;
          // above just make Main button change color

          // Emit "string" to parent component, that is memeber-edit component
          // Need to modify for parent component
          // this.getMemberPhotoChange.emit(photo.url);

          // make main button and main photo are the same url, not emitting an output property
          this.authService.changeMemberPhoto(photo.url);

          // Solve the localStorage for refreshing problem on main photo
          // Overwrite user in localStorage (from authService)
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem(
            "user",
            JSON.stringify(this.authService.currentUser)
          );
        },
        error => {
          this.alertify.error(error);
        }
      );
  }

  deletePhoto(id: number) { // id => photoId
    // Before deleting, show an alert first
    this.alertify.confirm("Are you sure you want to delete this photo?", () => {
                                                       // (userId, photoId)
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
        // Want to do here is remove the photo from our photos array
        // use 'splice', find the photo we deleted. photos from parent component
        this.photos.splice(this.photos.findIndex(p => p.id == id), 1);
        this.alertify.success('Photo has been deleted');
      }, error => {
        this.alertify.error('Failed to delete the photo');
      });
    });
  }

}
