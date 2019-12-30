namespace DatingApp_API.Helpers
{
    public class MessageParams
    {
        // Set a maximum page size the maximum number of items that will return from our API.
        private const int MaxPageSize = 50;
        public int PageNumber {get; set; } = 1; // Return first page in the begining

        // use 'propfull' , means manually set the get and set properties 
        // which gives us more control overlay geter and the setter.
        private int pageSize = 10;

        // custom page size, how many photos in one page
        public int PageSize 
        {
            get { return pageSize; } // this pageSize value can be changed by set{}
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public int UserId {get; set;}  // UserId matches the "senderId" or "recipientId"
        public string MessageContainer {get; set; } = "Unread";

    }
}