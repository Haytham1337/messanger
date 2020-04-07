using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions.MessageExceptions
{
    public class MessageInCorrectException : BaseException
    {
        public MessageInCorrectException(string message, int statusCode) : base(message, statusCode)
        {

        }
    }
}
