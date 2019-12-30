using System;

namespace DatingApp_API.Dtos
{
    public class MessageForCreationDto
    {
        public int SenderId { get; set; }   
        public int RecipientId { get; set; }
        public DateTime MessageSent { get; set; }
        public string Content { get; set; }

        // Set the date of the message sent

        public MessageForCreationDto()
        {
            MessageSent = DateTime.Now;
        }
    }
}