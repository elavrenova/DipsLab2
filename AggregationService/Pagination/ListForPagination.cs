using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AggregationService.Pagination
{
    public class ListForPagination<T>
    {
        public ListForPagination(List<T> info, int size, int page, int maxPage)
        {
            InfoForList = info;
            Size = size;
            Page = page;
            MaxPage = maxPage;
        }

        public List<T> InfoForList { get; private set; }
        public int Size { get; private set; }  
        public int Page { get; private set; }
        public int MaxPage { get; private set; }
    }
}
