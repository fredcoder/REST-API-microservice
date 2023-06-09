﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Core.Common.Models;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Core.Users.Queries;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync(
            [FromQuery] GetUserQuery query,
            CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // create a controller route (at /Find) that can retrieve a list of matching users using the `FindUsersQuery`
        [HttpGet("Find")]
        [ProducesResponseType(typeof(PaginatedDto<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync(
            [FromQuery] FindUsersQuery query,
            CancellationToken cancellationToken)
        {
            IEnumerable<UserDto> result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // create a route (at /List) that can retrieve a paginated list of users using the `ListUsersQuery`
        [HttpGet("List")]
        [ProducesResponseType(typeof(PaginatedDto<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync(
            [FromQuery] ListUsersQuery query,
            CancellationToken cancellationToken)
        {
            PaginatedDto<IEnumerable<UserDto>> result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        // TODO: create a route that can update a user using the `CreateUserCommand`
        [HttpPut]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PutUserAsync(
                [FromBody] PutUserQuery query,
                CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // create a route that can create an existing user using the `UpdateUserCommand`
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostUserAsync(
            [FromBody] PostUserQuery query,
            CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(query, cancellationToken);
            var location = Url.Action(nameof(GetUserAsync), new { id = result.UserId }) ?? $"/Users?id={result.UserId}";
            return Created(location, result);
        }

        // TODO: create a route that can delete an existing user using the `DeleteUserCommand`
        [HttpDelete]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserAsync(
            [FromQuery] DeleteUserQuery query,
            CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
