﻿using System.Net;

namespace Shared.Exceptions;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message, List<string>? errorMessages = null)
        : base(message, errorMessages, HttpStatusCode.Unauthorized)
    {
    }
}