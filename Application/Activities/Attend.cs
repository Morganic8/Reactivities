using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities {
    public class Attend {
        public class Command : IRequest {
            //activity ID
            public Guid Id { get; set; }

        }

        public class Handler : IRequestHandler<Command> {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor) {
                _context = context;
                _userAccessor = userAccessor;
            }

            //Unit is empty Object from Mediatr
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken) {

                //handler logic
                //get activity
                var activity = await _context.Activities.FindAsync(request.Id);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Activity = "could not find activity" });

                //get User
                var user = await _context.Users
                    //cant use FindAsync because the userName is not a Primary Key in ASP.NETUSERS DB
                    .SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUsername());

                //get attendance
                var attendance = await _context.UserActivities
                    .SingleOrDefaultAsync(x => x.ActivityId == activity.Id && x.AppUser.Id == user.Id);

                if (attendance != null)
                    throw new RestException(HttpStatusCode.BadRequest, new { Attendance = "Already attending this activity" });

                //create new attendance
                attendance = new UserActivity {
                    Activity = activity,
                    AppUser = user,
                    IsHost = false,
                    DateJoined = DateTime.Now
                };

                _context.UserActivities.Add(attendance);

                //SaveChangesAsync returns an Int, corresponding to how many items were added/changed;
                var success = await _context.SaveChangesAsync() > 0;

                if (success)return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}