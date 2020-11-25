using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities {
  public class Create {
    public class Command : IRequest {
      public Guid Id { get; set; }
      public string Title { get; set; }
      public string Description { get; set; }
      public string Category { get; set; }
      public DateTime Date { get; set; }
      public string City { get; set; }
      public string Venue { get; set; }
    }

    //Design Pattern - Put fluent validation between command and handler
    public class CommandValidator : AbstractValidator<Command> {
      public CommandValidator() {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Venue).NotEmpty();

      }
    }

    public class Handler : IRequestHandler<Command> {
      private readonly DataContext _context;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataContext context, IUserAccessor userAccessor) {
        _context = context;
        //refernce the user that is logged in
        _userAccessor = userAccessor;
      }

      //Unit is empty Object from Mediatr
      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken) {
        var activity = new Activity {
          Id = request.Id,
          Title = request.Title,
          Description = request.Description,
          Category = request.Category,
          Date = request.Date,
          City = request.City,
          Venue = request.Venue

        };
        //AddAsync not needed because it uses special value generation which we are not using for SQL
        _context.Activities.Add(activity);

        //Get user from the Database - need async call because we are going out to the DB
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUsername());

        // create host attendee
        var attendee = new UserActivity {
          AppUser = user,
          Activity = activity,
          IsHost = true,
          DateJoined = DateTime.Now
        };

        //make change to the DB
        _context.UserActivities.Add(attendee);

        //SaveChangesAsync returns an Int, corresponding to how many items were added/changed;
        //2 context changes - Activity line 59 - UserActivities line 72
        var success = await _context.SaveChangesAsync() > 0;

        if (success)return Unit.Value;

        throw new Exception("Problem saving changes");
      }
    }
  }
}