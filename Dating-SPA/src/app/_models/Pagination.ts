// export to UserServices

// going to "strongly type pagination" 
// So we're going to create an "interface".
export interface Pagination {
  // replicate the information we're getting back 
  // that we're sending in our pagination header.
  currentPage: number; // the property names are the same in back-end of PaginationHeaders.cs of
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

// user info and pagination info
// Use "T", becuase T can include User or Message
export class PaginatedResult<T> {
  result: T; // store user
  pagination: Pagination; // inherit Pagination and store message
}