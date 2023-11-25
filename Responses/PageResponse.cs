using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Responses
{
    public class PageResponse<T>

    {
        public List<T> Data { get; set; }
        public Dictionary<string, int> Meta { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, string> Links { get; set; } = new Dictionary<string, string>();
        public PageResponse(List<T> data)
        {
            Data = data;


        }
    }
}