using System;

namespace DatingApp.API.Models
{
    public class Message
    {
        public int Id {get; set;}
        public int SenderId { get; set; }
        public virtual User Sender { get; set; }
        public int RecipientId { get; set; }
        public virtual User Recipient {get; set; }
        public string Content {get; set; }
        public bool IsRead { get; set; }

        // After being read, DateRead works
        public DateTime? DateRead { get; set; }

        public DateTime MessageSent { get; set; }

        // We're only going to actually delete a message 
        // if both the sender and the recipient released a message.
        public bool SenderDeleted {get; set; }
        public bool RecipientDeleted { get; set; }
    }
}