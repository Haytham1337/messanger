using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions.MessageExceptions
{
    public class MessageNotExistException : BaseException
    {
        public MessageNotExistException(string message, int statusCode) : base(message, statusCode)
        {

        }
    }
}
