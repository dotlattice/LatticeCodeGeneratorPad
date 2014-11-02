using LatticeCodeGenerator.Contract;
using Roslyn.Compilers;
using Roslyn.Scripting.CSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LatticeCodeGenerator.CodeExecution
{
    /// <summary>
    /// Executes C# code and returns any objects that are "dumped" in our execution session.
    /// </summary>
    internal class CSharpCodeExecutor
    {
        private readonly ScriptEngine scriptEngine;
        private readonly List<FileInfo> customAssemblyFiles = new List<FileInfo>();

        public CSharpCodeExecutor()
        {
            this.scriptEngine = CreateScriptEngine();
        }

        private static ScriptEngine CreateScriptEngine()
        {
            var scriptEngine = new ScriptEngine();

            var standardAssemblies = new[] {
                Assembly.GetAssembly(typeof(object)),
                Assembly.GetAssembly(typeof(Enumerable)),
                Assembly.GetAssembly(typeof(LatticeCodeGenerator.Runtime.ObjectExtensions))
            };
            foreach (var assembly in standardAssemblies.Distinct())
            {
                scriptEngine.AddReference(assembly);
            }

            var standardNamespaces = new[] {
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "System.Text",
                "System.Threading.Tasks",
                typeof(LatticeCodeGenerator.Runtime.ObjectExtensions).Namespace,
            };
            foreach (var ns in standardNamespaces)
            {
                scriptEngine.ImportNamespace(ns);
            }

            return scriptEngine;
        }

        /// <summary>
        /// The files for any custom assemblies to include when executing the code.
        /// </summary>
        public ICollection<FileInfo> CustomAssemblyFiles
        {
            get { return customAssemblyFiles.AsReadOnly(); }
            set
            {
                customAssemblyFiles.Clear();
                if (value != null) customAssemblyFiles.AddRange(value);
            }
        }

        /// <summary>
        /// Executes the specified C# code and returns any "dumped" objects.
        /// </summary>
        public ICollection<object> ExecuteCode(string code)
        {
            CodeExecutionSession.StartSession();

            var roslynSession = scriptEngine.CreateSession();
            foreach (var assemblyFile in customAssemblyFiles)
            {
                roslynSession.AddReference(assemblyFile.FullName);
            }

            var codeBuilder = new StringBuilder();
            codeBuilder.AppendLine("using Console = LatticeCodeGenerator.Runtime.Console;");
            codeBuilder.Append(code);

            try
            {
                roslynSession.Execute(codeBuilder.ToString());
            }
            catch (CompilationErrorException ex)
            {
                throw new LatticeCodeGenerator.Contract.Exceptions.CompilationException(ex.Message, ex);
            }

            var codeGenerationSession = CodeExecutionSession.EndSession();
            return codeGenerationSession.DumpedObjects;
        }
    }
}
