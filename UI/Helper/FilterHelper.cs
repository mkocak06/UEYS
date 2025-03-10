using System;
using System.Collections.Generic;
using System.Linq;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.Types;
using UI.Pages.Authentication;

namespace UI.Helper;

public static class FilterHelper
{
    public static FilterDTO CreateFilter(int page, int pageSize)
    {
        return new FilterDTO()
        {
            page = page,
            pageSize = pageSize
        };
    }

    public static FilterDTO Sort(this FilterDTO filterDto, string field, SortType? sortType = null)
    {
        filterDto.Sort ??= new List<Sort>();
        if (filterDto.IsSorted(field, out var sort))
        {
            filterDto.Sort = new List<Sort>()
            {
                new()
                {
                    Dir = sort.Dir == SortType.ASC ? SortType.DESC : SortType.ASC,
                    Field = field
                }
            };
        }
        else
        {
            filterDto.Sort = new List<Sort>()
            {
                new()
                {
                    Dir = sortType ?? SortType.ASC,
                    Field = field
                }
            };
        }
        return filterDto;
    }

    public static bool IsSorted(this FilterDTO filterDto, string field, out Sort sort)
    {
        filterDto.Sort ??= new List<Sort>();
        sort = filterDto.Sort.FirstOrDefault(x => x.Field == field);
        return sort != null;
    }

    private static Sort GetSort(this FilterDTO filterDto, string field)
    {
        return filterDto.Sort.FirstOrDefault(x => x.Field == field);
    }

    public static FilterDTO Filter(this FilterDTO filterDto, string field, string op, object value, string logic)
    {
        filterDto.Filter ??= new Filter();
        filterDto.Filter.Filters ??= new List<Filter>();
        filterDto.Filter.Logic = "and";

        if (!filterDto.IsFiltered(field))
        {
            filterDto.Filter.Filters.Add(new Filter()
            {
                Field = field,
                Logic = logic,
                Operator = op,
                Value = value
            });
        }
        else
        {
            filterDto = filterDto.UpdateFilter(field, op, value, logic);
        }

        return filterDto;
    }

    public static FilterDTO OrFilter(this FilterDTO filterDto, string[] fields, string op, object value)
    {
        filterDto.Filter ??= new Filter();
        filterDto.Filter.Filters ??= new List<Filter>();
        filterDto.Filter.Logic = "and";

        filterDto.Filter.Filters.Add(new Filter()
        {
            Logic = "or",
            Filters = fields.Select(x => new Filter()
            {
                Field = x,
                Operator = op,
                Value = value
            }).ToList()
        });

        return filterDto;
    }

    private static FilterDTO UpdateFilter(this FilterDTO filterDto, string field, string op, object value, string logic)
    {
        if (filterDto.IsFiltered(field))
        {
            filterDto.GetFilter(field).Operator = op;
            filterDto.GetFilter(field).Value = value;
            filterDto.GetFilter(field).Logic = logic;
        }

        return filterDto;
    }

    public static FilterDTO RemoveFilter(this FilterDTO filterDto, string field)
    {
        filterDto.Filter.Filters ??= new List<Filter>();
        if (filterDto.IsFiltered(field))
        {
            filterDto.Filter.Filters = filterDto.Filter.Filters.Where(x => x.Field != field).ToList();
        }

        if (!filterDto.Filter.Filters.Any())
        {
            filterDto.Filter = null;
        }
        return filterDto;
    }

    private static bool IsFiltered(this FilterDTO filterDto, string field)
    {
        return filterDto.Filter?.Filters?.Any(x => x.Field == field) ?? false;
    }

    private static Filter GetFilter(this FilterDTO filterDto, string field)
    {
        return filterDto.Filter.Filters.FirstOrDefault(x => x.Field == field);
    }
}