using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace P7.Core.TagHelpers
{
    public class P7ScriptCbvTagHelper : P7TagHelperBase
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "script";
            var src = output.Attributes["src"];
            var finalSrcValue = src.Value.ToString().Replace("~/", CBVPrepend);
            output.Attributes.SetAttribute("src", finalSrcValue);
        }
    }
}