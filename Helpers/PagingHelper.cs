using System;
using System.Collections.Generic;
using System.Linq;
using whiskyshop.Models;
using System.Web;
using System.Web.Mvc;
using System.Text;


namespace whiskyshop.Helpers
{
    public static class PagingHelper
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html,
            PageInfo pageInfo, Func<int, string> pageUrl)
        {
            if (pageInfo.TotalPages > 1)
            {
                StringBuilder result = new StringBuilder();
                for (int i = 1; i <= pageInfo.TotalPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a");
                    tag.MergeAttribute("href", pageUrl(i));
                    tag.InnerHtml = i.ToString();
                    if (i == pageInfo.PageNumber)
                    {
                        tag.AddCssClass("selected");
                        tag.AddCssClass("btn-primary");
                    }
                    tag.AddCssClass("btn btn-default");
                    result.Append(tag.ToString());
                }
                return MvcHtmlString.Create(result.ToString());
            }
            else
            {
                return MvcHtmlString.Create("");
            }
        }
    }
}