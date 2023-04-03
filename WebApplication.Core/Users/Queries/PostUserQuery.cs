using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Queries
{
    public class PostUserQuery : IRequest<UserDto>
    {
        public int Id { get; set; }
        public string GivenNames { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }
        public string? EmailAddress { get; set; }

        public class Validator : AbstractValidator<PutUserQuery>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0);
            }
        }

        public class Handler : IRequestHandler<PostUserQuery, UserDto>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            /// <inheritdoc />
            public async Task<UserDto> Handle(PostUserQuery request, CancellationToken cancellationToken)
            {
                bool isBadRequest = false;
                string badRequestMessage = string.Empty;

                if (request.GivenNames == "")
                {
                    badRequestMessage = badRequestMessage + "'Given Names' must not be empty.;";
                    isBadRequest = true;
                }
                if (request.LastName == "")
                {
                    badRequestMessage = badRequestMessage + "'Last Name' must not be empty.;";
                    isBadRequest = true;
                }
                if (request.EmailAddress == "")
                {
                    badRequestMessage = badRequestMessage + "'Email Address' must not be empty.;";
                    isBadRequest = true;
                }
                if (request.MobileNumber == "")
                {
                    badRequestMessage = badRequestMessage + "'Mobile Number' must not be empty.";
                    isBadRequest = true;
                }

                if (isBadRequest)
                {
                    throw new ArgumentOutOfRangeException(null, badRequestMessage);
                }

                User user = new User()
                {
                    GivenNames = request.GivenNames,
                    LastName = request.LastName,
                    ContactDetail = new ContactDetail()
                    {
                        EmailAddress = request.EmailAddress,
                        MobileNumber = request.MobileNumber,
                    }
                };
                User newUser = await _userService.AddAsync(user, cancellationToken);
                UserDto result = _mapper.Map<UserDto>(newUser);

                return result;
            }
        }
    }
}
