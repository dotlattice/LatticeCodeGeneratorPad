using LatticeCodeGenerator.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeCodeGenerator
{
    /// <summary>
    /// The result of generating code after executing some C# code.
    /// </summary>
    /// <remarks>
    /// This needs to be serializable so it can be passed between AppDomains.  
    /// That's also why it only contains strings for exceptions instead of Exception objects (since some exceptions may not be serializable).
    /// </remarks>
    [Serializable]
    public class CodeGenerationResult : ICodeGenerationResult
    {
        /// <summary>
        /// Constructs a successful result.
        /// </summary>
        internal CodeGenerationResult(string code)
        {
            if (code == null) throw new ArgumentNullException("code");
            this.OutputCode = code;
        }

        /// <summary>
        /// Constructs a failed result.
        /// </summary>
        internal CodeGenerationResult(Exception ex)
        {
            if (ex == null) throw new ArgumentNullException("ex");
            this.ErrorMessage = ex.Message;
            this.ErrorDetail = ex.ToString();
        }

        public string OutputCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public string ErrorDetail { get; private set; }
    }
}
