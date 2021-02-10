using Microsoft.AspNetCore.Components;

namespace a1.Pages
{

    [Microsoft.AspNetCore.Components.RouteAttribute("/")]
    public class Index : Microsoft.AspNetCore.Components.ComponentBase
    {
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            BuildRenderTreeCore(__builder);
        }

        private void BuildRenderTreeCore(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            var header = Foo();
            __builder.OpenElement(0, "h1");
            __builder.AddContent(1, header);
            __builder.CloseElement();
            __builder.AddMarkupContent(3, "\r\n\r\nWelcome to your new app son.\r\n\r\n");
        }
        
        private static string Foo() => "Goodbye world?";
    }
}
