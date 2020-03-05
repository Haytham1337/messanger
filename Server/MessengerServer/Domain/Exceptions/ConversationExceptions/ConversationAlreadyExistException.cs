using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions.ChatExceptions
{
    public class ConversationAlreadyExistException:BaseException
    {
        public ConversationAlreadyExistException(string message,int statusCode):base(message,statusCode)
        {

        }
    }
}
