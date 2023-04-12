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

        public class Validator : AbstractValidator<PostUserQuery>
        {
            public Validator()
            {
                RuleFor(x => x.GivenNames).NotEqual("").WithState(x => "'Given Names' must not be empty.;");
                RuleFor(x => x.LastName).NotEqual("").WithState(x => "'Last Name' must not be empty.;");
                RuleFor(x => x.EmailAddress).NotEqual("").WithState(x => "'Email Address' must not be empty.;");
                RuleFor(x => x.MobileNumber).NotEqual("").WithState(x => "'Mobile Number' must not be empty.");
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

                Validator validator = new Validator();
                var valResult = validator.Validate(request);

                foreach (var failure in valResult.Errors)
                {
                    if (failure.CustomState is string)
                    {
                        badRequestMessage = badRequestMessage + failure.CustomState;
                        isBadRequest = true;
                    }
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
                        EmailAddress = request.EmailAddress != null ? request.EmailAddress : "",
                        MobileNumber = request.MobileNumber != null ? request.MobileNumber : "",
                    }
                };
                User newUser = await _userService.AddAsync(user, cancellationToken);
                UserDto result = _mapper.Map<UserDto>(newUser);

                return result;
            }
        }
    }
}
