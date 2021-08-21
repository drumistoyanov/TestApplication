﻿namespace TestApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public static class PageHelper
    {
        public static PagedResult<T> GetPaged<T>(this ICollection<T> query, int page, int pageSize) where T : class
        {
            var result = new PagedResult<T>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }
    }
}
