using System.Linq;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities {

    //IValueResolver lets us inject relational fields into AutoMapper
    public class FollowingResolver : IValueResolver<UserActivity, AttendeeDTO, bool> {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;
        public FollowingResolver(DataContext context, IUserAccessor userAccessor) {
            _userAccessor = userAccessor;
            _context = context;
        }

        //Cant be async method because implementation of Resolve requires a bool, not Task<Bool>
        public bool Resolve(UserActivity source, AttendeeDTO destination, bool destMember, ResolutionContext context) {

            //Since we cannot use await for SingleOrDefaulyAsync - we can use Result
            var currentUser = _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUsername()).Result;

            if (currentUser.Followings.Any(x => x.TargetId == source.AppUserId))
                return true;

            return false;
        }
    }
}