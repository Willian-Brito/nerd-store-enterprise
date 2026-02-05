using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NSE.WebAPI.Core.Structures;

namespace NSE.WebAPI.Core.Extensions;

public static class EnumerableExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
    {
        if (value == null)
            return true;

        return !value.Any();
    }

    public static PagedList<T> ToPagedList<T>(
        this IEnumerable<T> list,
        int pageNumber = 1,
        int pageSize = 10,
        string query = null
    )
    {
        var count = list.Count();
        var items = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedList<T>(items, count, pageNumber, pageSize, query);
    }
}