using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace Infrastructure.Security {

    //IAuthRequirement makes a property more valuable for HTTP requests.  
    //IsHostRequirement for example, is being specified with the Interface IAuthReq as a requirement
    public class IsHostRequirement : IAuthorizationRequirement { }

    //Create Handler for requirement
    //Authorization Handler  From NetCore.Auth needs the IsHostRequirement class type
    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement> {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _context;

        //IHttpContext
        public IsHostRequirementHandler(IHttpContextAccessor httpContextAccessor, DataContext context) {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        //Must be implemented because of AuthHandler<>
        //Trying to establish that a given user is a host of a particular activity
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement) {
            //get the user name
            //We need to get route data from HttpContextAccessor
            var currentUserName = _httpContextAccessor.HttpContext.User?.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            //get activityId
            //RouteValues returns a dict - so we look for the activityId labeled "id"
            var activityId = Guid.Parse(_httpContextAccessor.HttpContext.Request.RouteValues.SingleOrDefault(x => x.Key == "id").Value.ToString());

            //get activity from DB - We need Result from FindAsync
            var activity = _context.Activities.FindAsync(activityId).Result;

            //get host if there is one
            var host = activity.UserActivities.FirstOrDefault(x => x.IsHost);

            if (host?.AppUser.UserName == currentUserName)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}