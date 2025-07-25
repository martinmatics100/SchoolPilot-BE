﻿

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Interfaces;

namespace SchoolPilot.Data.Helpers
{
    public static class PagedQueryHelper
    {
        public static async Task<PagedResult<T>> ToPageResultsAsync<T>(this IQueryable<T> queryable, int page, int pageLength)
        {
            page = page < 1 ? 1 : page;
            pageLength = pageLength < 1 ? 10 : pageLength;

            var items = await queryable.Page(page, pageLength).ToListAsync();
            var count = await queryable.CountAsync();
            var pageCount = (int)Math.Ceiling(count / (double)pageLength); // Will return Ceiling(NaA) if pageLength = 0 which equals something like Int.Min

            return new PagedResult<T>
            {
                Items = items,
                CurrentPage = page,
                ItemCount = count,
                PageCount = pageCount,
                PageLength = pageLength
            };
        }

        public static async Task<TResult> ToPageResultsAsync<T, TResult>(this IQueryable<T> queryable, int page, int pageLength)
            where TResult : PagedResult<T>, new()
        {
            page = page < 1 ? 1 : page;
            pageLength = pageLength < 1 ? 10 : pageLength;

            var items = await queryable.Page(page, pageLength).ToListAsync();
            var count = await queryable.CountAsync();
            var pageCount = (int)Math.Ceiling(count / (double)pageLength); // Will return Ceiling(NaA) if pageLength = 0 which equals something like Int.Min

            return new TResult
            {
                Items = items,
                CurrentPage = page,
                ItemCount = count,
                PageCount = pageCount,
                PageLength = pageLength
            };
        }

        public static Task<PagedResult<T>> ToPageResultsAsync<T>(this IQueryable<T> queryable, IPagedRequest request, int maxPageLength = 50)
        {
            var pageLength = request.PageLength > maxPageLength ? maxPageLength : request.PageLength;

            return queryable.ToPageResultsAsync<T, PagedResult<T>>(request.Page, pageLength);
        }

        public static Task<TResult> ToPageResultsAsync<T, TResult>(this IQueryable<T> queryable, IPagedRequest request, int maxPageLength = 100)
            where TResult : PagedResult<T>, new()
        {
            var pageLength = request.PageLength > maxPageLength ? maxPageLength : request.PageLength;

            return queryable.ToPageResultsAsync<T, TResult>(request.Page, pageLength);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> queryable, int pageIndex, int pageLength, bool zeroBase = false)
        {
            if (!zeroBase)
            {
                pageIndex -= 1;
            }

            var itemsToSkip = Math.Max(pageIndex * pageLength, 0);

            return queryable.Skip(itemsToSkip).Take(pageLength);
        }


        public static async Task<CursorPagedResult<T, TCursorType>> ToPageResultsAsync<T, TCursorType>(this IQueryable<T> queryable, int pageLength, Func<T, TCursorType> cursorSelector)
            where TCursorType : struct
        {
            pageLength = pageLength < 1 ? 10 : pageLength;

            var items = await queryable.Take(pageLength + 1).ToListAsync();
            var trueItems = items.Take(pageLength).ToList();
            return new CursorPagedResult<T, TCursorType>
            {
                IsAtEnd = !(items.Count > pageLength),
                Items = trueItems,
                NextCursor = trueItems.Count > 0
                    ? trueItems.Select(cursorSelector).Last()
                    : default(TCursorType),
                PageLength = pageLength
            };
        }

        public static Task<CursorPagedResult<T, TCursorType>> ToPageResultsAsync<T, TCursorType>(this IQueryable<T> queryable, ICursorPagedRequest<TCursorType> request, Func<T, TCursorType> cursorSelector, int maxPageLength = 25)
            where TCursorType : struct
        {
            var pageLength = request.PageLength > maxPageLength ? maxPageLength : request.PageLength;

            return queryable.ToPageResultsAsync(pageLength, cursorSelector);
        }
    }
}
