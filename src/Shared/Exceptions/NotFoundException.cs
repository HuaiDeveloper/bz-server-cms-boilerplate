using System.Net;

namespace Shared.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message, List<string>? errorMessages = null) 
        : base(message, errorMessages, HttpStatusCode.NotFound)
    {
        
    }
}