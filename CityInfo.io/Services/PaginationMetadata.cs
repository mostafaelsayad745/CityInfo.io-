using System.ComponentModel;

namespace CityInfo.io.Services
{
    public class PaginationMetadata
    {
        public PaginationMetadata(int totalItmeCount, int pageSize, int currentPage)
        {
            TotalItmeCount = totalItmeCount;
            TotalPageCount = (int)Math.Ceiling(totalItmeCount/(double)pageSize);
            PageSize = pageSize;
            CurrentPage = currentPage;
        }

        public int TotalItmeCount { get; set; }
        public int TotalPageCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
    }
}
