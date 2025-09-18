﻿namespace TaskBoardManagement.ExceptionMiddleware
{
    public class MemberAlreadyExistsException : DomainException
    {
        public MemberAlreadyExistsException(Guid userId, Guid projectId)
            : base("MEMBER_ALREADY_EXISTS", $"User {userId} is already a member of project {projectId}.") { }
    }
}
