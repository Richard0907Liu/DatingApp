

namespace DatingApp.API.Models
{
    // Fluent API, many to many relationship
    public class Like
    {
        public int LikerId {get; set;}  // userID
        public int LikeeId {get; set;}  // userID, Be liked
        public virtual User Liker {get; set;}
        public virtual User Likee {get; set;}
    }
}