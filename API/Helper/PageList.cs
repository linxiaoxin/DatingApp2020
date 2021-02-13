using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helper
{
    public class PageList<T> : List<T>
    {
        public PageList(List<T> items, int pageNum, int totalCount, int pageSize)
        {
            CurrentPage = pageNum;
            TotalCount = totalCount;
            PageSize = pageSize;
            TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);
            AddRange(items);
        }
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public static async Task<PageList<T>> CreateAsync( IQueryable<T> source, int pageSize, int pageNum)
        {
            if(source == null) return new PageList<T>(new List<T>(), 0, 0, pageSize);
            var count = await source.CountAsync();    
            var items = await source.Skip((pageNum-1)*pageSize).Take(pageSize).ToListAsync();

            return new PageList<T>(items, pageNum, count, pageSize);
        }
    }

}