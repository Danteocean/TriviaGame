namespace Domain.Exceptions;

public class InfrastructureException : Exception
{
    public InfrastructureException() : base()
    {

    }

    public InfrastructureException(String message,params Object[] args)
    {

    }
    public InfrastructureException(String message) : base(message)
    {
    }


    public InfrastructureException(String message, Exception innerException): base(message, innerException)
    {

    }
}
