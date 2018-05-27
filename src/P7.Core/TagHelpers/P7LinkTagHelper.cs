using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace P7.Core.TagHelpers
{
    public class P7LinkTagHelper : P7TagHelperBase
    {
        //<link rel="stylesheet" href="/lib/bootstrap/dist/css/bootstrap.css" />

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "link";                                 // Replaces <p7-style> with <link> tag
            var href = output.Attributes["href"];
            var finalHrefValue = UriHelper.CombineUriToString("http://localhost:7791/", href.Value.ToString());
            output.Attributes.SetAttribute("href", finalHrefValue);
        }
    }
}