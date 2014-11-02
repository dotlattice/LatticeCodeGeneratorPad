using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeCodeGenerator.Contract
{
    /// <summary>
    /// The interface for a code generator that runs some user-supplied code at runtime.
    /// </summary>
    public interface ICodeGenerator
    {
        /// <summary>
        /// Any custom assembly files to include when generating code.
        /// </summary>
        ICollection<FileInfo> CustomAssemblyFiles { get; set; }

        /// <summary>
        /// Executes the specified code and the resulting.
        /// </summary>
        /// <param name="code">the C# code to execute</param>
        /// <returns>the string representation of an object tree for the specified code</returns>
        ICollection<ICodeGenerationResult> Execute(string code);
    }
}
