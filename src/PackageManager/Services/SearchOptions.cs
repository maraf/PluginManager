using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    public class SearchOptions
    {
        public static class Default
        {
            public const int PageSize = 10;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public SearchOptions()
        { }

        public SearchOptions(int pageIndex)
        {
            PageIndex = pageIndex;
            PageSize = Default.PageSize;
        }

        public SearchOptions(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
