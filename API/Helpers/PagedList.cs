using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count/ (decimal)pageSize); //pageSize - oldalak szama
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        // The current page where the user is in. i.e. 2nd page out of 50
        public int CurrentPage { get; set; }
        // Total number of pages. i.e. (100 data and ten each page. So 100/10 = 10 Pages)
        public int TotalPages { get; set; }
        //Number of items per page (i.e. 100 total data and ten items per page)
        public int PageSize { get; set; }
        //Defined as the total number of data in the dataset
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);

        }
    }
}