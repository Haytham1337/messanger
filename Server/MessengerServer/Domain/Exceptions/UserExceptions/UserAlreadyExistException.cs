namespace Domain.Exceptions.UserExceptions
{
    public class UserAlreadyExistException : BaseException
    {
        public UserAlreadyExistException(string message, int statusCode) : base(message, statusCode)
        {

        }
    }
}
