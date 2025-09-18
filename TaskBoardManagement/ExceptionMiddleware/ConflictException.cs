using TaskBoardManagement.ExceptionMiddleware;

public class ConflictException : DomainException
{


    public ConflictException(string entityName, Guid id)
             : base($"{entityName.ToUpper()}ALREADY_EXISTS", $"{entityName} with Id {id} already exist.")
    { }
}
