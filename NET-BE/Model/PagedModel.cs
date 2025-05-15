using System.Collections.Generic;
using System.Linq;

namespace NET_BE.Model
{
    public class PagedModel<T>
    {
        public const int DefaultItemsPerPage = 20;

        public long TotalCount { get; set; }

        public IEnumerable<T> Data { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (int)System.Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PagedModel(long totalCount, IEnumerable<T> data, int pageIndex, int pageSize)
        {
            TotalCount = totalCount;
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public static PagedModel<T> Empty => new PagedModel<T>(0, Enumerable.Empty<T>(), 1, DefaultItemsPerPage);
    }
}
