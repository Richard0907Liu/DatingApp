// import { ErrorInterceptor } from './error.interceptor';
import { Injectable } from "@angular/core";
import {
  HttpInterceptor,
  HttpErrorResponse,
  HTTP_INTERCEPTORS
} from "@angular/common/http";
import { catchError } from "rxjs/operators";
import { throwError } from "rxjs";

// Use @Ingectalbe for Error handling in Angular
// That anuglar http interceptor is going to recognize an Http error like 400, 500
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(
    req: import("@angular/common/http").HttpRequest<any>,
    next: import("@angular/common/http").HttpHandler // What heppen after req, the next catch the error
  ): import("rxjs").Observable<import("@angular/common/http").HttpEvent<any>> {
    // throw new Error("Method not implemented."); // comment out
    return next.handle(req).pipe(
      catchError(error => {
        // What we want to do with error
        if (error.status === 401) {
          return throwError(error.statusText);
        }
        // Deal with the other type of errors, first we want to take a look inside this error object
        // If error is aHttpErrorResponse instance, want to see error further
        if (error instanceof HttpErrorResponse) {
          // When Application-error is added from back-end
          const applicationError = error.headers.get("Application-error");
          if (applicationError) {
            return throwError(applicationError);
          }

          // When there is 500 error, deal with 500 type errors
          const serverError = error.error;
          let modalStateErrors = "";
          // To see errors is available and when errors is an object, and then see errors in object
          if (serverError.errors && typeof serverError.errors === "object") {
            for (const key in serverError.errors) {
              // In this case, key is password
              console.log("key in error.interceptor: ", key);
              if (serverError.errors[key]) {
                modalStateErrors += serverError.errors[key] + "\n"; // build a list string
              }
            }
          }

          return throwError(modalStateErrors || serverError || "Server Error");
        }
      })
    );
  }
}

// Export ErrorInterceptorProvider, because going to need to add this to our provider array
export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true // Because HTTP_INTERCEPTORS has multiple interceptor
};
