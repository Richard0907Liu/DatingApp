import { Injectable } from "@angular/core";

// Need to solve this problem (...), add typings.d.ts in src folder
import * as alertify from "alertifyjs";

@Injectable({
  providedIn: "root"
})
export class AlertifyService {
  constructor() {}

  // Add a method called confirm with message and okCallBack the returned type is "any"
  confirm(message: string, okCallBack: () => any) {
    // Pass "message", and take event 'e'
    alertify.confirm(message, (e: any) => {
      if (e) {
        // event if clicked ok button, then we'll call okCallBack,
        //  this okCallBack is something that will "define inside our components"
        okCallBack();
      } else {
        // if no event, do nothing and cancell this callback function, cancel this confirmation notification
      }
    });
  }

  // Add a method, it can pass success message
  success(message: string) {
    alertify.success(message);
  }

  error(message: string) {
    alertify.error(message);
  }

  warning(message: string) {
    alertify.warning(message);
  }

  message(message: string) {
    alertify.message(message);
  }
}
