import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";

import { RouterModule } from "@angular/router";
// import { ValueComponent } from "./value/value.component";
import { HttpClientModule } from "@angular/common/http";
import { NavComponent } from "./nav/nav.component";

import { FormsModule } from "@angular/forms";
import { BsDropdownModule } from "ngx-bootstrap";

import { AppComponent } from "./app.component";
import { AuthService } from "./_services/auth.service";
import { HomeComponent } from "./home/home.component";
import { RegisterComponent } from "./register/register.component";
import { ErrorInterceptorProvider } from "./_services/error.interceptor";
import { AlertifyService } from "./_services/alertify.service";
import { MemberListComponent } from "./member-list/member-list.component";
import { ListComponent } from "./list/list.component";
import { MessagesComponent } from "./messages/messages.component";
import { appRoutes } from "./routes";

@NgModule({
  declarations: [
    AppComponent,
    //ValueComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    MemberListComponent,
    ListComponent,
    MessagesComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    BsDropdownModule.forRoot(),
    // First import RouterModule from @angular, and then add routes.ts into forRoot
    RouterModule.forRoot(appRoutes)
  ],
  providers: [AuthService, ErrorInterceptorProvider, AlertifyService],
  bootstrap: [AppComponent]
})
export class AppModule {}
