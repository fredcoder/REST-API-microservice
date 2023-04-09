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
    public class PutUserQuery : IRequest<UserDto>
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

        public class Handler : IRequestHandler<PutUserQuery, UserDto>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            /// <inheritdoc />
            public async Task<UserDto> Handle(PutUserQuery request, CancellationToken cancellationToken)
            {
                bool isBadRequest = false;
                string badRequestMessage = string.Empty;

                if (request.Id <= 0)
                {
                    badRequestMessage = badRequestMessage + "'Id' must be greater than '0'.;";
                    isBadRequest = true;
                }
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

                List<User>? users = (List<User>)await _userService.FindAsync(request.GivenNames, request.LastName, cancellationToken);

                if (users.Count == 0)
                    throw new NotFoundException($"The user '{request.Id}' could not be found.");

                User? updateUser = users[0];

                updateUser.GivenNames = request?.GivenNames;
                updateUser.LastName = request?.LastName;
                updateUser.ContactDetail.EmailAddress = request?.EmailAddress;
                updateUser.ContactDetail.MobileNumber = request?.MobileNumber;

                User? user = await _userService.UpdateAsync(updateUser, cancellationToken);

                UserDto result = _mapper.Map<UserDto>(user);

                return result;
            }
        }
    }
}
