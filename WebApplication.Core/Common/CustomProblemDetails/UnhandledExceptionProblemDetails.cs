using System;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;

namespace WebApplication.Core.Common.CustomProblemDetails
{
    public class UnhandledExceptionProblemDetails : StatusCodeProblemDetails
    {
        /// <inheritdoc />
        public UnhandledExceptionProblemDetails(Exception ex) : base(StatusCodes.Status500InternalServerError)
        {
            Detail = string.IsNullOrWhiteSpace(ex.Message)
                ? "An unexpected error has occured."
                : ex.Message;
        }
    }
}
