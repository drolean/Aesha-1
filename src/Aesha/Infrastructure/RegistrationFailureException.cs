using System;
using System.Runtime.Serialization;
using System.Text;

namespace Aesha.Infrastructure
{
    [Serializable]
    public class RegistrationFailureException : Exception
    {
        public RegistrationFailureException(string message) : base(message)
        {

        }

        protected RegistrationFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine("Registration Assertion failed due to the following:");
                sb.AppendLine(base.Message);
                return sb.ToString();
            }
        }
    }
}