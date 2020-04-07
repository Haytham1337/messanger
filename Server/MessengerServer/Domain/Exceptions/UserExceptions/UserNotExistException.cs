using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions.UserExceptions
{
    public class UserNotExistException : BaseException
    {
        public UserNotExistException(string message, int statusCode) : base(message, statusCode)
        {

        }
    }
}
