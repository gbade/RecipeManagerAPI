using System;
using System.Collections.Generic;
using System.Text;

namespace HelloFreshGo.Entities.Models
{
    public class UrlQuery
    {
        //private const int maxPageSize = 100;
        //public bool IncludeCount { get; set; } = false;
        //public int? PageNumber { get; set; }

        //private int _pageSize = 50;
        //public int PageSize
        //{
        //    get
        //    {
        //        return _pageSize;
        //    }
        //    set
        //    {
        //        _pageSize = (value < maxPageSize) ? value : maxPageSize;
        //    }
        //}

        public Recipes Recipe { get; set; }

        public string Name { get; set; }
        public string PrepTime { get; set; }
        public bool HaveFilter => !string.IsNullOrEmpty(Recipe.Name) || !string.IsNullOrEmpty(Recipe.PrepTime);

        public UrlQuery()
        {
            Recipe = new Recipes();
        }
    }
}
