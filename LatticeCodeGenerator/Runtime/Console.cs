using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeCodeGenerator.Runtime
{
    /// <summary>
    /// A class with an interface consistent with the normal <c>System.Console</c> class 
    /// that dumps the object instead of outputting it to the console.
    /// </summary>
    public static class Console
    {
        #region WriteLine

        public static void WriteLine()
        {
            WriteObjectLine(null);
        }
        public static void WriteLine(object value)
        {
            WriteObjectLine(value);
        }
        public static void WriteLine(string value)
        {
            WriteObjectLine(value);
        }
        public static void WriteLine(string format, params object[] arg)
        {
            if (arg == null)
            {
                WriteObjectLine(format);
                return;
            }
            WriteObjectLine(string.Format(format, arg));
        }
        public static void WriteLine(bool value)
        {
            WriteObjectLine(value);
        }
        public static void WriteLine(char value)
        {
            WriteObjectLine(value);
        }
        public static void WriteLine(char[] buffer)
        {
            WriteObjectLine(buffer);
        }
        public static void WriteLine(char[] buffer, int index, int count)
        {
            WriteObjectLine(buffer.Skip(index).Take(count).ToArray());
        }
        public static void WriteLine(decimal value)
        {
            WriteObjectLine(value);
        }
        public static void WriteLine(double value)
        {
            WriteObjectLine(value);
        }
        public static void WriteLine(float value)
        {
            WriteObjectLine(value);
        }
        public static void WriteLine(int value)
        {
            WriteObjectLine(value);
        }
        public static void WriteLine(uint value)
        {
            WriteObjectLine(value);
        }
        public static void WriteLine(long value)
        {
            WriteObjectLine(value);
        }
        public static void WriteLine(ulong value)
        {
            WriteObjectLine(value);
        }
        private static void WriteObjectLine(object value)
        {
            ObjectExtensions.Dump(value);
        }

        #endregion

        #region Write

        public static void Write(string value)
        {
            WriteObject(value);
        }
        public static void Write(string format, params object[] arg)
        {
            if (arg == null)
            {
                WriteObject(format);
                return;
            }
            WriteObject(string.Format(format, arg));
        }
        public static void Write(bool value)
        {
            WriteObject(value);
        }
        public static void Write(char value)
        {
            WriteObject(value);
        }
        public static void Write(char[] buffer)
        {
            WriteObject(buffer);
        }
        public static void Write(char[] buffer, int index, int count)
        {
            WriteObject(buffer.Skip(index).Take(count).ToArray());
        }
        public static void Write(double value)
        {
            WriteObject(value);
        }
        public static void Write(decimal value)
        {
            WriteObject(value);
        }
        public static void Write(float value)
        {
            WriteObject(value);
        }
        public static void Write(int value)
        {
            WriteObject(value);
        }
        public static void Write(uint value)
        {
            WriteObject(value);
        }
        public static void Write(long value)
        {
            WriteObject(value);
        }
        public static void Write(ulong value)
        {
            WriteObject(value);
        }
        public static void Write(object value)
        {
            WriteObject(value);
        }
        private static void WriteObject(object value)
        {
            ObjectExtensions.Dump(value);
        }

        #endregion
    }
}
