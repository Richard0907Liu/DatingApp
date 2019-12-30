import { AlertifyService } from './../../_services/alertify.service';
import { AuthService } from './../../_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { tap } from 'rxjs/operators';



@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  // Need to pass "recipientId" from parent component "member-detail"
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};


  constructor(private userService: UserService, 
    private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    // Get userId
    // Add "+" can force any type to become "number"
    const currentUserId = +this.authService.decodedToken.nameid;
    // const currentUserId2:number= this.authService.decodedToken.nameid; also work

    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
      // we then call the markAsRead methods in our userService.
      .pipe(
        // tap() operator, this allows us to do something before we subscribe to this particular method.
        // In messages array we'll need to check that the recipientId matches the current userId for each message.
        tap(messages => {
          // use loop to scan 
          for(let i = 0; i < messages.length; i++) {
            if(messages[i].isRead === false && messages[i].recipientId === currentUserId) {
              this.userService.markAsRead(currentUserId, messages[i].id);
            }
          }

        })
      )
      .subscribe(messages => {
        this.messages = messages;
    }, error => {
      this.alertify.error(error);
    })
  }

  // Need to send the recipientId in the body
  sendMessage() {
    // set recipientId into the newMessage
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe((message: Message) => {
       // debugger;
        // add this message to the messages array at the start of array not end of array
        // Use "unshift" to add this message at the begining
        this.messages.unshift(message);

        // reset the form into empty string
        // this.newMessage = '';    wrong because newMessage is an object not string
        this.newMessage.content = ''; // correct
      }, error => {
        this.alertify.error(error);
      })
  }

}
