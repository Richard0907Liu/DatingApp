import { AlertifyService } from "./../_services/alertify.service";
import { AuthService } from "./../_services/auth.service";
import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { FormGroup, FormControl, Validators, FormBuilder } from "@angular/forms";
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: "app-register",
  templateUrl: "./register.component.html",
  styleUrls: ["./register.component.css"]
})
export class RegisterComponent implements OnInit {
  // Use Input() to get variable from parent (home html)
  @Input() valuesFromHome: any;

  // Use Output() to pass variable to parent component
  // Output property emits "events"
  @Output() cancelRegister = new EventEmitter(); // This EventEmitter from core

  user: User;
  //model: any = {}; old 

  registerForm: FormGroup;

  // Change bsDatepicker directive color
  // if we only want to implement parts of this bsDatepickerConfig, use Partial<>
  // otherwise there are many mandatory property have to be defined
  // by adding partial we've now made all of these optional.
  bsConfig: Partial<BsDatepickerConfig>;


  constructor( private authService: AuthService, 
      private router: Router,
      private alertify: AlertifyService,
      private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red'
    };

    this.createRegisterForm();

    // Replace FormGroup with "FormBuilder" to create a form, 
    // this.registerForm = new FormGroup({
    //   username: new FormControl('', Validators.required),
    //   password: new FormControl('', 
    //     [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
    //   confirmPassword: new FormControl('', Validators.required)
    // }, this.passwordMatchValidator);
  }

  // Create forms with FormBuilder, using FormBuilder is more simple to perform a form
  createRegisterForm() {
    this.registerForm = this.fb.group({
      // key:value pair
      gender: ['male'],  // default is 'male' in html
      username: ['', Validators.required], //first part is form state, second is validators
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required], // input is date, so default is null
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required]
    }, // Add custom validator as an object
      {validator: this.passwordMatchValidator});
  }

  // custom validator of reactive form
  passwordMatchValidator(g: FormGroup) {
    // get() from FormGroup
    // if not match, return an object
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true} 

  }


  register() {
    if(this.registerForm.valid) {
        // Object.assign(), This copy is about the use of all of the innumerable 
        // own properties from one or more source objects to a target objects.
        // first para is targe object, 
        // registerFrom => {} => this.user
        this.user = Object.assign({}, this.registerForm.value);
        this.authService.register(this.user).subscribe(() => {  // because param is "user" not model, so need to modify register()
          this.alertify.success('Registration successful');
        }, error => {
          this.alertify.error(error);
        },
          // Add complete, after a users registered will automatically log in immediately
          () => {
            this.authService.login(this.user).subscribe(() => {
              // after login, redirect to members page
              this.router.navigate(['/members']);
            })
          }
        );
    }

    //console.log(this.model);
    //// not recieve any response, so use () =>
    // this.authService.register(this.model).subscribe(
    //   () => {
    //     // console.log("registration succcessful");
    //     this.alertify.success("registration succcessful");
    //   },
    //   error => {
    //     this.alertify.error(error);
    //     //console.log(error);
    //     // console.log("error.error.errors: ", error.error.errors);
    //   }
    // );
    
    // Reactive form, output our values
    //console.log(this.registerForm.value);
  }

  cancel() {
    // emit something from this method, can be an object, values etc
    this.cancelRegister.emit(false); // this case, emit "false"
    console.log("cancelled!");
  }
}
