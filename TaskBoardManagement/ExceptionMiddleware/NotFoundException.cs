namespace TaskBoardManagement.ExceptionMiddleware
{
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string entityName, Guid id)
            : base($"{entityName.ToUpper()}_NOT_FOUND", $"{entityName} with Id {id} does not exist.")
        { }
    }
}

