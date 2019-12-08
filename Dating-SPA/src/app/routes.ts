import { AuthGuard } from "./_guards/auth.guard";
import { ListComponent } from "./list/list.component";
import { HomeComponent } from "./home/home.component";
import { Routes } from "@angular/router";
import { MemberListComponent } from "./member-list/member-list.component";
import { MessagesComponent } from "./messages/messages.component";

export const appRoutes: Routes = [
  { path: "", component: HomeComponent },

  // Protecting multiple routes once by using a "dummy routes"
  {
    // => / and then go to children add members or messages etc
    path: "", // path="dummy" => dummymembers
    runGuardsAndResolvers: "always",
    canActivate: [AuthGuard],
    children: [
      // Add "canActivate" to protect content, but have to add many canActivate into many path objects
      //{ path: "members", component: MemberListComponent, canActivate: [AuthGuard] },

      // All of three fo these is in roots.
      { path: "members", component: MemberListComponent },
      { path: "messages", component: MessagesComponent },
      { path: "lists", component: ListComponent }
    ]
  },
  // END of dummy routes

  // wildcard route, want a match for full path URL to rediect
  // pathMatch optional property that defaults to ' determines whether to match full URLs or just the beginning
  { path: "**", redirectTo: "", pathMatch: "full" }
];
