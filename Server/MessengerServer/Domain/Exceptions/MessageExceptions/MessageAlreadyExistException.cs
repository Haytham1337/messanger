using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions.MessageExceptions
{
    public class MessageAlreadyExistException : BaseException
    {
        public MessageAlreadyExistException(string message, int statusCode) : base(message, statusCode)
        {

        }
    }
}
