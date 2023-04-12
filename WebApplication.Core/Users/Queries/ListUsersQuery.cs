using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using WebApplication.Core.Common.Models;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Queries
{
    public class ListUsersQuery : IRequest<PaginatedDto<IEnumerable<UserDto>>>
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; } = 10;

        public class Validator : AbstractValidator<ListUsersQuery>
        {
            public Validator()
            {
                RuleFor(x => x.PageNumber).GreaterThan(0).WithState(x => new ArgumentOutOfRangeException(null, "'Id' must be greater than '0'."));
            }
        }

        public class Handler : IRequestHandler<ListUsersQuery, PaginatedDto<IEnumerable<UserDto>>>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;
            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            public async Task<PaginatedDto<IEnumerable<UserDto>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
            {
                var validator = new Validator();
                validator.Validate(request, options => options.ThrowOnFailures());

                IEnumerable<User> listUsers = await _userService.GetPaginatedAsync(request.PageNumber, request.ItemsPerPage);

                List<UserDto> listUsersDto = _mapper.Map<List<UserDto>>(listUsers);

                int usersCount = await _userService.CountAsync();

                // Calculate the starting index of the page
                int startIndex = request.ItemsPerPage * (request.PageNumber - 1);

                // Calculate the end index of the page
                int endIndex = startIndex + request.ItemsPerPage;

                PaginatedDto<IEnumerable<UserDto>> result = new PaginatedDto<IEnumerable<UserDto>>()
                {
                    Data = listUsersDto,
                    HasNextPage = endIndex < usersCount
                };

                return result;
                //throw new NotImplementedException("Implement a way to get a paginated list of all the users in the database.");
            }
        }
    }
}
