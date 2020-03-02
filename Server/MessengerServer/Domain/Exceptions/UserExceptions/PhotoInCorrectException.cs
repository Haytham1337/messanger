using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions.UserExceptions
{
    public class PhotoInCorrectException:BaseException
    {
        public PhotoInCorrectException(string message,int statusCode):base(message,statusCode)
        {

        }
    }
}
