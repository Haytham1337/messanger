namespace Domain.Exceptions.ChatExceptions
{
    public class ConversationNotExistException : BaseException
    {
        public ConversationNotExistException(string message, int statusCode) : base(message, statusCode)
        {

        }
    }
}
