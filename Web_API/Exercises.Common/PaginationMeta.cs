using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Exercises.Common
{
    public class PaginationMeta
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public string PreviousPageLink { get; set; }
        public string NextPageLink { get; set; }


        public static PaginationMeta Create<TList, TFilter>(
            TFilter filter,
            PagedList<TList> pagedList,
            string routeName,
            IHttpContextAccessor accessor, LinkGenerator generator
        ) where TFilter : FilterBase
        {
            string prevPageLink = null;
            string nextPageLink = null;

            if (pagedList.HasPrevious)
            {
                filter.PageNumber = filter.PageNumber - 1;
                prevPageLink = generator.GetUriByAddress(accessor.HttpContext,routeName,new RouteValueDictionary(filter));
            }

            if (pagedList.HasNext)
            {
                filter.PageNumber = filter.PageNumber + 1;
                nextPageLink = generator.GetUriByAddress(accessor.HttpContext,routeName,new RouteValueDictionary(filter));
            }

            return new PaginationMeta
            {
                TotalCount = pagedList.TotalCount,
                PageSize = pagedList.PageSize,
                CurrentPage = pagedList.CurrentPage,
                TotalPages = pagedList.TotalPages,
                PreviousPageLink = prevPageLink,
                NextPageLink = nextPageLink,
            };
        }
    }
}