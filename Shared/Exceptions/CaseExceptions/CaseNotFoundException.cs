namespace Shared.Exceptions.CaseExceptions;

public class CaseNotFoundException : CaseOpeningException
{
    
    public CaseNotFoundException()
    {
    }

    public CaseNotFoundException(string message) : base(message)
    {
        
    }

    public CaseNotFoundException(string message, Exception inner) : base(message, inner)
    {
        
    }
    
}