using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace P7.Core.TagHelpers
{
    public class P7ScriptTagHelper : P7TagHelperBase
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "script"; // Replaces <email> with <a> tag
            var src = output.Attributes["src"];
            var finalSrcValue = UriHelper.CombineUriToString("http://localhost:7791/", src.Value.ToString());
            output.Attributes.SetAttribute("src", finalSrcValue);
        }
    }
}