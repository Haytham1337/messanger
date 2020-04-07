namespace Domain.Exceptions.UserExceptions
{
    public class AccessTokenIsNotValidException : BaseException
    {
        public AccessTokenIsNotValidException(string message, int statusCode) : base(message, statusCode)
        {

        }
    }
}
