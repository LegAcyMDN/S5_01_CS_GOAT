namespace Shared.Exceptions.CaseExceptions;

public class InvalidQuantityException : CaseOpeningException
{
    public InvalidQuantityException()
    {
        
    }

    public InvalidQuantityException(string message) : base(message)
    {
        
    }
    
    public InvalidQuantityException(string message, Exception inner) : base(message, inner)
    {
    }
}