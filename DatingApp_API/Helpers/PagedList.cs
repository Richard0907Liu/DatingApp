using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp_API.Helpers
{
    // T can be swapped with user or members or whatever else
    // We can use the same class now because we make it generic.
    public class PagedList<T> : List<T>  // PagedList return T object
    {
        public int CurrentPage { get; set;}
        public int TotalPages { get; set;}

        public int PageSize { get; set;}

        public int TotalCount { get; set;}

        // Constructor
        // the parameters in here what we're going to pass List class when we initialize a new instance of this.
        public PagedList(List<T> items, int count, int pageNumber, int pageSize) 
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // AddRange(),  Adds the elements of the specified collection to the end of the System.Collections.Generic.List`1.
            // Parameters:
            //   collection:
            //     The collection whose elements should be added to the end of the System.Collections.Generic.List`1.
            //     The collection itself cannot be null, but it can contain elements that are null,
            //     if type T is a reference type.
            this.AddRange(items);
        }

        // Paging
        // also want to do inside this class is create a static method and then
        // create a new instance of this class. 
        // Linq for "IQueryable". IQueryable means params can be use with Linq
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // return a new instance of a pagedList
            // IQueryable, this allows us to defer early execution of our request to get a bunch of users for example 
            // and it allows us to define parts of our query against our database in multiple steps and we have deferred execution
            // Add skip() and take() operators, so that we can skip a number of items and then take a number of items that matches 
            // our page number and page size. And this allows us to page our request.

            var count = await source.CountAsync(); // bring in EntityFramworkCore for CountAsync
            //("count:::::::::::: " + count);  // 16
            // Get items
            // ToListAsync(), execute this request and list members
            var items = await source.Skip((pageNumber -1) * pageSize).Take(pageSize).ToListAsync();
            // items would be a list 

            // when we call this method we're going to return a new "pageList" with passing in this information.
            return new PagedList<T>(items, count, pageNumber, pageSize);  
        }


    }
}