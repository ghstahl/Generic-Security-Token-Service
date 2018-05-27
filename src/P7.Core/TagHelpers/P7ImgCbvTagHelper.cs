using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace P7.Core.TagHelpers
{
    public class P7ImgCbvTagHelper : P7TagHelperBase
    {
        //<img src = "~/images/ASP-NET-Banners-01.png" alt="ASP.NET" class="img-responsive" />
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";
            var src = output.Attributes["src"];
            var finalSrcValue = src.Value.ToString().Replace("~/", CBVPrepend);
            output.Attributes.SetAttribute("src", finalSrcValue);
        }
    }
}