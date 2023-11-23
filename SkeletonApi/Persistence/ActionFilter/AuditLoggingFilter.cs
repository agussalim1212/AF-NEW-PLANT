using Microsoft.AspNetCore.Mvc.Filters;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Repositories;

namespace SkeletonApi.Presentation.ActionFilter
{
    public class AuditLoggingFilter : IAsyncActionFilter
    {
        private readonly AuditRepository _auditRepository;

        public AuditLoggingFilter(AuditRepository auditRepository)
        {
            _auditRepository = auditRepository;

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            DateTime date = DateTime.Now;

            var activityUser = new ActivityUser
            {
                Id = Guid.NewGuid(),
                UserName = context.HttpContext.User.Identity.Name,
                LogType = context.HttpContext.Request.Method,
                DateTime = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second),

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

            _auditRepository.AddAuditActivity(activityUser);

            var resultContext = await next();


        }
    }
}

