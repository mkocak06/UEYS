using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Extentsions
{
    public static class DataPagerExtension
    {
        public static async Task<PagedModel<T>> PaginateAsync<T>(
               this IQueryable<T> query,
               int page,
               int limit,
               CancellationToken cancellationToken)
               where T : class
        {
            var paged = new PagedModel<T>();

            page = (page < 0) ? 1 : page;

            paged.Page = page;
            paged.PageSize = limit;

            var totalItemsCountTask = await query.CountAsync(cancellationToken);

            var startRow = (page - 1) * limit;
            paged.Items = await query
                       .Skip(startRow)
                       .Take(limit)
                       .ToListAsync(cancellationToken);

            paged.TotalItemCount = totalItemsCountTask;
            paged.TotalPages = (int)Math.Ceiling(paged.TotalItemCount / (double)limit);

            if (paged.TotalPages < page)
            {
                startRow = 1;
                paged.Items = await query
                    .Skip(startRow)
                    .Take(limit)
                    .ToListAsync(cancellationToken);

                paged.TotalItemCount = totalItemsCountTask;
                paged.TotalPages = (int)Math.Ceiling(paged.TotalItemCount / (double)limit);
                paged.Page = 1;
            }

            return paged;
        }
    }
}
