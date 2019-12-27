import { AuthGuard } from "./_guards/auth.guard";
import { ListComponent } from "./list/list.component";
import { HomeComponent } from "./home/home.component";
import { Routes } from "@angular/router";
import { MemberListComponent } from "./members/member-list/member-list.component";
import { MessagesComponent } from "./messages/messages.component";
import { MemberDetailComponent } from "./members/member-detail/member-detail.component";
import { MemberDetailResolver } from "./_resolvers/member-detail.resolver";
import { MemberListResolver } from "./_resolvers/member-list.resolver";
import { MemberEditComponent } from "./members/member-edit/member-edit.component";
import { MemberEditResolver } from "./_resolvers/member-edit.resolver";
import { PreventUnsavedChanges } from "./_guards/prevent-unsaved-changes.guard";
import { ListsResolver } from './_resolvers/lists.resolver';

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
      {
        path: "members",
        component: MemberListComponent,
        resolve: { users: MemberListResolver }
      },
      // resolve, passing object (MemberDetailResolver) to "user" from our root.
      // resolve for getting our data before we activate the Router itself.
      {
        path: "members/:id",
        component: MemberDetailComponent,
        resolve: { user: MemberDetailResolver }
      },
      {
        path: "member/edit",
        component: MemberEditComponent,
        resolve: { user: MemberEditResolver },
        canDeactivate: [PreventUnsavedChanges]
      },
      { path: "messages", component: MessagesComponent },
      { path: "lists", 
        component: ListComponent, 
        resolve: {users: ListsResolver} }
    ]
  },
  // END of dummy routes

  // wildcard route, want a match for full path URL to rediect
  // pathMatch optional property that defaults to ' determines whether to match full URLs or just the beginning
  { path: "**", redirectTo: "", pathMatch: "full" }
];
