import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
// import { ValueComponent } from "./value/value.component";
import { HttpClientModule } from "@angular/common/http";
import { NavComponent } from "./nav/nav.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BsDropdownModule, TabsModule, BsDatepickerModule, PaginationModule, ButtonsModule } from "ngx-bootstrap";
import { JwtModule } from "@auth0/angular-jwt";
import { MemberDetailResolver } from "./_resolvers/member-detail.resolver";
import { MemberListResolver } from "./_resolvers/member-list.resolver";
import { NgxGalleryModule } from "ngx-gallery";
import {
  BrowserModule,
  HammerGestureConfig,
  HAMMER_GESTURE_CONFIG
} from "@angular/platform-browser";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';


// import services, resolve into "providers"

// Put Modules into "imports"
import { FileUploadModule } from 'ng2-file-upload';


//// import component into "declaration"
import { AppComponent } from "./app.component";
import { AuthService } from "./_services/auth.service";
import { HomeComponent } from "./home/home.component";
import { RegisterComponent } from "./register/register.component";
import { ErrorInterceptorProvider } from "./_services/error.interceptor";
import { AlertifyService } from "./_services/alertify.service";
import { MemberListComponent } from "./members/member-list/member-list.component";
import { ListComponent } from "./list/list.component";
import { MessagesComponent } from "./messages/messages.component";
import { appRoutes } from "./routes";
import { UserService } from "./_services/user.service";
import { MemberCardComponent } from "./members/member-card/member-card.component";
import { MemberDetailComponent } from "./members/member-detail/member-detail.component";
import { MemberEditComponent } from "./members/member-edit/member-edit.component";
import { MemberEditResolver } from "./_resolvers/member-edit.resolver";
import { PreventUnsavedChanges } from "./_guards/prevent-unsaved-changes.guard";
import { PhotoEditorComponent } from "./members/photo-editor/photo-editor.component";
import {TimeAgoPipe} from 'time-ago-pipe';

// Solve when first logging, the request didn't include token for sending the request
// Add JwtModule
export function tokenGetter() {
  return localStorage.getItem("token");
}

// Solve this problem, Class constructor HammerGestureConfig cannot be invoked without 'new'
export class CustomHammerConfig extends HammerGestureConfig {
  overrides = {
    pinch: { enable: false },
    rotate: { enable: false }
  };
}

@NgModule({
  declarations: [
    AppComponent,
    //ValueComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    MemberListComponent,
    ListComponent,
    MessagesComponent,
    MemberCardComponent,
    MemberDetailComponent,
    MemberEditComponent,
    PhotoEditorComponent,
    TimeAgoPipe // for date pipe
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),

    // First import RouterModule from @angular, and then add routes.ts into forRoot
    RouterModule.forRoot(appRoutes),
    // For solving when first logging, the request didn't include token for sending the request
    // Send token to the Root and other service can get a token automatically
    NgxGalleryModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter, // last one is a function from above
        whitelistedDomains: ["localhost:5000"], // have to be correct
        blacklistedRoutes: ["localhost:5000/api/auth"] // set up this because not need to send token to this url
      }
    }),
    // Import FileUpload module
    FileUploadModule,  // once import FileUploadModule, red underline on [uploader]="uploader" would be dispaeared
    ReactiveFormsModule,
    BsDatepickerModule.forRoot(),  // Add this to make datepicker workable for all browser
    BrowserAnimationsModule, // for datepicker too
    PaginationModule.forRoot(), // For pagination 
    ButtonsModule.forRoot() // for buttons
  ],
  providers: [
    AuthService,
    ErrorInterceptorProvider,
    AlertifyService,
    UserService,
    MemberDetailResolver,
    MemberListResolver,
    // Solve this problem, Class constructor HammerGestureConfig cannot be invoked without 'new'
    { provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig },
    MemberEditResolver,
    PreventUnsavedChanges
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
