using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Commands
{
    public class DeleteUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }

        public class Validator : AbstractValidator<DeleteUserCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0);
            }
        }

        public class Handler : IRequestHandler<DeleteUserCommand, UserDto>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            /// <inheritdoc />
            public async Task<UserDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                User? deletedUser = await _userService.DeleteAsync(request.Id, cancellationToken);

                if (deletedUser is default(User)) throw new NotFoundException($"The user '{request.Id}' could not be found.");

                return _mapper.Map<UserDto>(deletedUser);
            }
        }
    }
}
