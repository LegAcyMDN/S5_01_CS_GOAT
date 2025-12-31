namespace Shared.Exceptions.CaseExceptions;

public class InvalidPromoCodeException : CaseOpeningException
{
    public InvalidPromoCodeException()
    {
        
    }
    public InvalidPromoCodeException(string message) : base(message)
    {
        
    }
    
    public InvalidPromoCodeException(string message, Exception inner) : base(message, inner)
    {
        
    }
    
}