using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SF.LvmBlog.ViewModels;

namespace SF.LvmBlog.TagHelpers;

public class PageLinkTagHelper : TagHelper
{
    public SortState Property { get; set; }
    public bool Up { get; set; }
    public string Other { get; set; }           // остальные параметры

    private IUrlHelperFactory urlHelperFactory;
    public PageLinkTagHelper(IUrlHelperFactory helperFactory)
    {
        urlHelperFactory = helperFactory;
    }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }
    public PageViewModel PageModel { get; set; }
    public string PageAction { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
        output.TagName = "div";

        // набор ссылок будет представлять список ul
        TagBuilder tag = new TagBuilder("ul");
        tag.AddCssClass("pagination pagination-sm justify-content-center");

        // ссылка на первую страницу
        if (PageModel.PageNumber != 1)
        {
            TagBuilder firstItem = CreateTag(1, urlHelper);
            tag.InnerHtml.AppendHtml(firstItem);
        }

        // создаем ссылку на предыдущую страницу, если она есть
        if (PageModel.HasPreviousPage && PageModel.PageNumber - 1 != 1)
        {
            TagBuilder prevItem = CreateTag(PageModel.PageNumber - 1, urlHelper);
            tag.InnerHtml.AppendHtml(CreateEllipsis());
            tag.InnerHtml.AppendHtml(prevItem);
        }

        // ссылка на текущую страницу
        TagBuilder currentItem = CreateTag(PageModel.PageNumber, urlHelper);
        tag.InnerHtml.AppendHtml(currentItem);

        // создаем ссылку на следующую страницу, если она есть
        if (PageModel.HasNextPage)
        {
            TagBuilder nextItem = CreateTag(PageModel.PageNumber + 1, urlHelper);
            tag.InnerHtml.AppendHtml(nextItem);
        }

        // создаем ссылку на последнюю страницу
        if (PageModel.HasNextPage && PageModel.PageNumber + 1 != PageModel.TotalPages)
        {
            TagBuilder lastItem = CreateTag(PageModel.TotalPages, urlHelper);
            tag.InnerHtml.AppendHtml(CreateEllipsis());
            tag.InnerHtml.AppendHtml(lastItem);
        }

        output.Content.AppendHtml(tag);
    }

    TagBuilder CreateEllipsis()
    {
        TagBuilder span = new TagBuilder("span");
        span.Attributes.Add("style", "padding: 10px");
        span.InnerHtml.Append(". . .");
        return span;
    }

    TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
    {
        TagBuilder item = new TagBuilder("li");
        TagBuilder link = new TagBuilder("a");

        if (pageNumber == this.PageModel.PageNumber)
        {
            item.AddCssClass("active");
        }
        else
        {
            link.Attributes["href"] = urlHelper.Action(PageAction, new { page = pageNumber, sortOrder = Property, up = Up }) + Other;
        }

        item.AddCssClass("page-item");
        link.AddCssClass("page-link");
        link.InnerHtml.Append(pageNumber.ToString());
        item.InnerHtml.AppendHtml(link);
        return item;
    }
}

