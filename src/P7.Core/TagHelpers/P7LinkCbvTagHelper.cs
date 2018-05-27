using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace P7.Core.TagHelpers
{
    public class P7LinkCbvTagHelper : P7TagHelperBase
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "link";
            var href = output.Attributes["href"];
            var finalHrefValue = href.Value.ToString().Replace("~/", CBVPrepend);
            output.Attributes.SetAttribute("href", finalHrefValue);
        }
    }
}