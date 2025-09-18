public class MemberNotFoundException : Exception
{
    public MemberNotFoundException(Guid userId, Guid projectId)
        : base($"User {userId} is not a member of Project {projectId}.")
    {
    }
}

