import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
// import { ValueComponent } from "./value/value.component";
import { HttpClientModule } from "@angular/common/http";
import { NavComponent } from "./nav/nav.component";
import { FormsModule } from "@angular/forms";
import { BsDropdownModule, TabsModule } from "ngx-bootstrap";
import { JwtModule } from "@auth0/angular-jwt";
import { MemberDetailResolver } from "./_resolvers/member-detail.resolver";
import { MemberListResolver } from "./_resolvers/member-list.resolver";
import { NgxGalleryModule } from "ngx-gallery";
import {
  BrowserModule,
  HammerGestureConfig,
  HAMMER_GESTURE_CONFIG
} from "@angular/platform-browser";

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
    MemberDetailComponent
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
    })
  ],
  providers: [
    AuthService,
    ErrorInterceptorProvider,
    AlertifyService,
    UserService,
    MemberDetailResolver,
    MemberListResolver,
    // Solve this problem, Class constructor HammerGestureConfig cannot be invoked without 'new'
    { provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
