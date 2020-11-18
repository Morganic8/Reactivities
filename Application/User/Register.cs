using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Application.Validators;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.User {
    public class Register {
        public class Command : IRequest<User> //We will return a User object but the pattern doesnt really want to return anything
            {
                public string DisplayName { get; set; }
                public string Username { get; set; }
                public string Email { get; set; }
                public string Password { get; set; }

            }

        public class CommandValidator : AbstractValidator<Command> {
            public CommandValidator() {
                RuleFor(x => x.DisplayName).NotEmpty();
                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                //chain on different validators
                RuleFor(x => x.Password).Password();
            }
        }

        public class Handler : IRequestHandler<Command, User> {
            private readonly DataContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly IJwtGenerator _jwtGenerator;

            public Handler(DataContext context, UserManager<AppUser> userManager, IJwtGenerator jwtGenerator) {
                _context = context;
                _userManager = userManager;
                _jwtGenerator = jwtGenerator;
            }

            //Unit is empty Object from Mediatr
            public async Task<User> Handle(Command request, CancellationToken cancellationToken) {

                //check to see email doesn't exist in DB
                if (await _context.Users.AnyAsync(x => x.Email == request.Email))
                    throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });

                //check to see username doesnt exist in DB
                if (await _context.Users.AnyAsync(x => x.UserName == request.Username))
                    throw new RestException(HttpStatusCode.BadRequest, new { Username = "Username already exists" });

                //Create new App user
                var user = new AppUser {
                    DisplayName = request.DisplayName,
                    Email = request.Email,
                    UserName = request.Username
                };

                //await the usermanager to create user using identity
                var result = await _userManager.CreateAsync(user, request.Password);

                //If user is created successfully using identity
                //Return the user information
                if (result.Succeeded) {
                    return new User {
                        DisplayName = user.DisplayName,
                            Token = _jwtGenerator.CreateToken(user),
                            Username = user.UserName,
                            Image = null
                    };
                }

                //throw error 
                throw new Exception("Problem creating user");
            }
        }
    }
}