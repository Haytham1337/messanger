using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions.BlockedUserExceptions
{
    public class BlockedUserNotExistException : BaseException
    {
        public BlockedUserNotExistException(string message, int statusCode) : base(message, statusCode)
        {

        }
    }
}
