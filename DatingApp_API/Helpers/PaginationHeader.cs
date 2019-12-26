namespace DatingApp_API.Helpers
{
    // Pass this information back to HTTP response Header
    public class PaginationHeader
    {
        public int CurrentPage { get; set;}
        public int ItemsPerPage { get; set;}
        public int TotalItems {get; set;}
        public int TotalPages {get; set;}

        // Constructor for sending information back to Header
        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            this.CurrentPage = currentPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalItems = totalItems;
            this.TotalPages = totalPages;

            // Add params back to HTTP header, just like we've done before with the application errors 
            // we are also going to add another extension method into Extension.cs for HTTP response
            // So that we can add this pagination header
        }
    }
}