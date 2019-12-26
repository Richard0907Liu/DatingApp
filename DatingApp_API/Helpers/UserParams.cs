namespace DatingApp_API.Helpers
{
    // Need to deal with serialized JSON string in Header params for pagination
    public class UserParams
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

        // FOr filtering members
        public int UserId {get; set;}
        public string Gender {get; set; }

        public int MinAge {get; set; } = 18; // by default 18
        public int MaxAge {get; set; } = 99;

        // Add OrderBy filter property
        public string OrderBy {get; set; }
        
    }
}