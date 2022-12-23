using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MangaWeb.Filters
{
    public class ModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context is null) throw new ArgumentNullException("The context was null");
            if(!context.ModelState.IsValid) 
            { 
                context.Result = new PageResult(); 
            }
        }
    }
}
