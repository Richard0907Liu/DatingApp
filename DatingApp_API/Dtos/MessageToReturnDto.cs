using System;

namespace DatingApp_API.Dtos
{
    public class MessageToReturnDto
    {
        
        public int Id {get; set;}
        public int SenderId { get; set; }

        // It's important we keep the properties at the part after the sender and the recipient.
        // In this case the same as what they are in our User class because AutoMapper is pretty clever and it's
        // recognize that if my senderId relates to a userId. AutoMapper also recoginzes KnownAs and PhotoUrl keywords 
        public string SenderKnownAs { get; set; }
        public string SenderPhotoUrl { get; set; }
        public int RecipientId { get; set; }
        public string RecipientKnownAs {get; set; }
        public string RecipientPhotoUrl { get; set; }
        public string Content {get; set; }
        public bool IsRead { get; set; }

        // After being read, DateRead works
        public DateTime? DateRead { get; set; }

        public DateTime MessageSent { get; set; }
    }
}