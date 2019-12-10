import { Injectable, Component } from "@angular/core";
import { CanDeactivate } from "@angular/router";
import { MemberEditComponent } from "./../members/member-edit/member-edit.component";

@Injectable()

// need to tell CanDeactivate which component applies to
export class PreventUnsavedChanges
  implements CanDeactivate<MemberEditComponent> {
  // pass this our component because we need to get access to the form inside MemberEditComponent component.
  canDeactivate(component: MemberEditComponent) {
    if (component.editForm.dirty) {
      // If editFrom is dirty, pop up dialogue. If user clikc OK let's move on to the page that navigate to
      // If click NO, simply close out a confirmed dialogue and the user can go back to editing the form.
      return confirm(
        "Are you sure you want to continue? Any unsaved changes will be lost!"
      );
    }

    return true;
  }
}
