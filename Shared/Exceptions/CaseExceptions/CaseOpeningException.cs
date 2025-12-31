namespace Shared.Exceptions.CaseExceptions;

public class CaseOpeningException : Exception
{
    public CaseOpeningException()
    {
        
    }

    public CaseOpeningException(string message) : base(message)
    {
        
    }



    public CaseOpeningException(string message, Exception inner) : base(message, inner)
    {
        
    }
    
}