using System;
using System.Collections.Generic;

namespace NSE.WebApp.MVC.ViewModel;

public class PagedViewModel<T> : IPagedList where T : class
{
    public IEnumerable<T> Items { get; set; }
    public string ReferenceAction { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public string Query { get; set; }
    public int TotalCount { get; set; }
    public double TotalPages => Math.Ceiling((double)TotalCount / PageSize);
}

public interface IPagedList
{
    public string ReferenceAction { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public string Query { get; set; }
    public int TotalCount { get; set; }
    public double TotalPages { get; }
}