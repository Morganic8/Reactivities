using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles {
    public class Edit {
        public class Command : IRequest {
            public string DisplayName { get; set; }
            public string Bio { get; set; }

        }

        public class CommandValidator : AbstractValidator<Command> {
            public CommandValidator() {
                RuleFor(x => x.DisplayName).NotEmpty();

            }
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

                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUsername());

                if (user == null)
                    throw new RestException(HttpStatusCode.NotFound, new { User = "User Not Found" });

                user.DisplayName = request.DisplayName ?? user.DisplayName;
                user.Bio = request.Bio ?? user.Bio;

                //SaveChangesAsync returns an Int, corresponding to how many items were added/changed;
                var success = await _context.SaveChangesAsync() > 0;

                if (success)return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}