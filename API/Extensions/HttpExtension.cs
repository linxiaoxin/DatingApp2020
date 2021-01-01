using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtension
    {
        public static void addPaginationHeader(this HttpResponse response, int pageNum, int pageSize, int totalCount, int totalPages){
            var paginationHeader = new PaginationHeader(pageNum, pageSize, totalCount, totalPages); 

            var options = new JsonSerializerOptions{
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };   
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader,options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }

    public class PaginationHeader
    {
        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

    }
}