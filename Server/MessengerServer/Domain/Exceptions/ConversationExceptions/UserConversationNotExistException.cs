namespace Domain.Exceptions.ConversationExceptions
{
    public class UserConversationNotExistException : BaseException
    {
        public UserConversationNotExistException(string message, int statusCode) : base(message, statusCode)
        {

        }
    }
}
