using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LatticeCodeGenerator.Contract.Exceptions
{
    /// <summary>
    /// An exception thrown when compiling user-supplied code at runtime fails.
    /// </summary>
    [Serializable]
    public class CompilationException : Exception
    {
        public CompilationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CompilationException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
