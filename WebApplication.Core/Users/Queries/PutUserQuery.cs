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
                RuleFor(x => x.Id).GreaterThan(0).WithState(x => "'Id' must be greater than '0'.;");
                RuleFor(x => x.GivenNames).NotEqual("").WithState(x => "'Given Names' must not be empty.;");
                RuleFor(x => x.LastName).NotEqual("").WithState(x => "'Last Name' must not be empty.;");
                RuleFor(x => x.EmailAddress).NotEqual("").WithState(x => "'Email Address' must not be empty.;");
                RuleFor(x => x.MobileNumber).NotEqual("").WithState(x => "'Mobile Number' must not be empty.");
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

                User? updateUser = await _userService.GetAsync(request.Id, cancellationToken);

                if (updateUser == null)
                    throw new NotFoundException($"The user '{request.Id}' could not be found.");

                updateUser.GivenNames = request?.GivenNames;
                updateUser.LastName = request?.LastName;
                updateUser.ContactDetail = new ContactDetail()
                {
                    EmailAddress = request?.EmailAddress,
                    MobileNumber = request?.MobileNumber,
                };


                User? user = await _userService.UpdateAsync(updateUser, cancellationToken);

                UserDto result = _mapper.Map<UserDto>(user);

                return result;
            }
        }
    }
}
