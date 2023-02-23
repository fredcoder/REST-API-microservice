using System;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;

namespace WebApplication.Core.Common.CustomProblemDetails
{
    public class NotFoundProblemDetails : StatusCodeProblemDetails
    {
        /// <inheritdoc />
        public NotFoundProblemDetails(Exception ex) : base(StatusCodes.Status404NotFound)
        {
            Detail = string.IsNullOrWhiteSpace(ex.Message)
                ? "The requested resource was not found."
                : ex.Message;
        }
    }
}
