using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Persistence
{
    public static class LinksGenerator
    {
        public static Dictionary<string, string> GenerateLinks(string baseURL,
      int currentPage, int totalRecords, int pageSize)
        {
            var result = new Dictionary<string, string>();
            int totalPages = (int)Math.Ceiling((decimal)totalRecords / pageSize);
            result.Add("First", $"{baseURL}?pageNumber=1&pagesize={pageSize}");  // /api/people?pageNumber=1&pageSize=10


            result.Add("Prev", currentPage > 1 ? $"{baseURL}?pageNumber={currentPage - 1}&pageSize={pageSize}" : "");
            result.Add("Next", currentPage == totalPages ? "" : $"{baseURL}?pageNumber={currentPage + 1}&pageSize={pageSize}");

            result.Add("Last", $"{baseURL}?pageNumber={totalPages}&pageSize={pageSize}");
            return result;

        }
    }
}