using LatticeCodeGenerator.Contract;
using LatticeCodeGenerator.CodeExecution;
using LatticeObjectTree.CodeGenerators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeCodeGenerator
{
    /// <summary>
    /// Executes C# code at runtime and generates C# code from its output.
    /// </summary>
    public class ObjectTreeCSharpCodeGenerator : MarshalByRefObject, ICodeGenerator
    {
        private readonly CSharpCodeExecutor codeExecutor = new CSharpCodeExecutor();

        /// <summary>
        /// The files containing any custom assemblies to include when executing the C# code.
        /// </summary>
        public ICollection<FileInfo> CustomAssemblyFiles
        {
            get { return codeExecutor.CustomAssemblyFiles; }
            set { codeExecutor.CustomAssemblyFiles = value; }
        }

        /// <summary>
        /// Executes the specified C# code and returns the results of generating code from any dumped objects.
        /// </summary>
        public ICollection<ICodeGenerationResult> Execute(string code)
        {
            var dumpedObjects = codeExecutor.ExecuteCode(code);
            if (dumpedObjects == null || !dumpedObjects.Any())
            {
                return null;
            }

            var codeGenerator = new CSharpObjectCodeGenerator();

            var results = new List<ICodeGenerationResult>(dumpedObjects.Count);
            foreach (var obj in dumpedObjects)
            {
                try
                {
                    var objectCode = codeGenerator.GenerateCode(obj);
                    var objectResult = new CodeGenerationResult(objectCode);
                    results.Add(objectResult);
                }
                catch (Exception ex)
                {
                    var errorResult = new CodeGenerationResult(ex);
                    results.Add(errorResult);
                }
            }

            return results;
        }
    }
}
