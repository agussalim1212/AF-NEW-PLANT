using Microsoft.AspNetCore.Mvc.Filters;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Presentation.ActionFilter
{
    public class AuditLoggingFilter : IAsyncActionFilter
    {
    
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {


            var activityUser = new ActivityUser
            {
                Id = Guid.NewGuid(),
                UserName = context.HttpContext.User.Identity.Name,
                LogType = context.HttpContext.Request.Method,
                DateTime = DateTime.Now,

            };

            if (context.HttpContext.User.Identity.Name == null)
            {
                activityUser.UserName = "Unauthorized";
            }

            if (context.HttpContext.Request.Method == "GET")
            {
                activityUser.LogType = "View";
            }
            else if (activityUser.LogType == "POST")
            {
                activityUser.LogType = "Create";
            }
            else if (activityUser.LogType == "PUT")
            {
                activityUser.LogType = "Update";
            }
            else
            {
                activityUser.LogType = "Delete";
            }

           // _auditRepository.AddAuditActivity(activityUser);

            var resultContext = await next();


        }
    }
}

