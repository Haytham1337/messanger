namespace Domain.Exceptions.UserExceptions
{
    public class UserNotHaveRigthsException:BaseException
    {
        public UserNotHaveRigthsException(string message,int statusCode):base(message,statusCode)
        {

        }
    }
}
