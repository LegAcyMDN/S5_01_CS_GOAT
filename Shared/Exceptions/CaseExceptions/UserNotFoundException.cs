namespace Shared.Exceptions.CaseExceptions;

public class UserNotFoundException : CaseOpeningException
{
    public UserNotFoundException()
    {
        
    }

    public UserNotFoundException(string message) : base(message)
    {
        
    }

    public UserNotFoundException(string message, Exception inner) : base(message, inner)
    {
        
    }
}