using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.User.Models.CustomException
{
    public class AddressFunctionException : Exception
    {
        public AddressFunctionException() { }

        public AddressFunctionException(string message) : base(message) { }

        public AddressFunctionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
