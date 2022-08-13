using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SF.LvmBlog.ViewModels;

namespace SF.LvmBlog.TagHelpers;

public class SortLinkTagHelper : TagHelper
{
    public SortState Property { get; set; }      // значение активного свойства, выбранного для сортировки (предыдущего)
    public SortState Current { get; set; }       // сортировка столбца на который кликнули
    public string Other { get; set; }           // остальные параметры

    public string Action { get; set; }          // действие контроллера, на которое создается ссылка
    public bool Up { get; set; }                // сортировка по возрастанию или убыванию

    private IUrlHelperFactory urlHelperFactory;
    public SortLinkTagHelper(IUrlHelperFactory helperFactory)
    {
        urlHelperFactory = helperFactory;
    }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
        output.TagName = "a";
        string url = urlHelper.Action(Action,
            new { sortOrder = Property, up = (Current == Property) ? !Up : true }) + Other;
        output.Attributes.SetAttribute("href", url);
        output.Attributes.SetAttribute("class", "dropdown-item");

        // Добавляем значок сортировки
        if (Current == Property)
        {
            TagBuilder tag = new TagBuilder("i");
            tag.AddCssClass("fa-solid");

            if (Up == true)   // если сортировка по возрастанию
                tag.AddCssClass("fa-chevron-up");
            else   // если сортировка по убыванию
                tag.AddCssClass("fa-chevron-down");

            output.PreContent.AppendHtml(tag);
        }
    }
}


