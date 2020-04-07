using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions.BlockedUserExceptions
{
    public class BlockedUserAlreadyExistException : BaseException
    {
        public BlockedUserAlreadyExistException(string message, int statusCode) : base(message, statusCode)
        {

        }
    }
}
