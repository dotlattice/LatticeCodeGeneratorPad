using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeCodeGenerator.Contract
{
    /// <summary>
    /// The result of compiling and running code at runtime.
    /// </summary>
    public interface ICodeGenerationResult
    {
        /// <summary>
        /// The code generated from the dumped objects resulting from executing the user's code, or null if there were no objects dumped successfully.
        /// </summary>
        string OutputCode { get; }

        /// <summary>
        /// The message from any error that prevented a normal result from being created.  
        /// This will be null if there were no errors.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// The details (including stack trace) of any error that prevented a normal result from being created.
        /// This will be null if there were no errors.
        /// </summary>
        string ErrorDetail { get; }
    }
}
