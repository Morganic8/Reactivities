using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Delete
    {
        public class Command : IRequest
            {
              public Guid Id { get; set; }
        
            }
        
            public class Handler : IRequestHandler<Command>
            {
              private readonly DataContext _context;
              public Handler(DataContext context)
              {
                _context = context;
              }
        
              //Unit is empty Object from Mediatr
              public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
              {
        
                //handler logic
                var activity = await _context.Activities.FindAsync(request.Id);

                if(activity == null)
                  throw new RestException(HttpStatusCode.NotFound, new {activity = "Not Found"});

                  _context.Remove(activity);
        
                //SaveChangesAsync returns an Int, corresponding to how many items were added/changed;
                var success = await _context.SaveChangesAsync() > 0;
        
                if(success) return Unit.Value;
        
                throw new Exception("Problem saving changes");
              }
            }
    }
}