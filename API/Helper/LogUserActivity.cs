using System.Threading.Tasks;
using API.Extensions;
using API.Interface;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helper
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if(!resultContext.HttpContext.User.Identity.IsAuthenticated)
                return;
            else{
                //user is authenticated
                var userId = resultContext.HttpContext.User.GetUserId();
                var repo = resultContext.HttpContext.RequestServices.GetService<IAppUserRespository>();
                var user = await repo.GetUserByIdAsync(userId);
                user.LastActive = System.DateTime.Now;
                await repo.SaveAllAsync();
            }
        }
    }
}