using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeCodeGenerator.CodeExecution
{
    /// <summary>
    /// The session in which code is executed.  This keeps track of any "dumped" objects.
    /// </summary>
    internal class CodeExecutionSession
    {
        #region Static

        private static readonly object staticSyncRoot = new object();
        private static CodeExecutionSession currentSession;

        /// <summary>
        /// Starts and returns a code execution session, 
        /// which will also be the session returned by the <c>CurrentSession</c> property.
        /// </summary>
        internal static CodeExecutionSession StartSession()
        {
            lock (staticSyncRoot)
            {
                currentSession = new CodeExecutionSession();
            }
            return currentSession;
        }

        /// <summary>
        /// Returns the last session that was started, or null if the last started session has been ended.
        /// </summary>
        public static CodeExecutionSession CurrentSession
        {
            get
            {
                return currentSession;
            }
        }

        /// <summary>
        /// Ends and returns the currently active session, or returns null if there is no active session.
        /// </summary>
        internal static CodeExecutionSession EndSession()
        {
            CodeExecutionSession session;
            lock (staticSyncRoot)
            {
                session = currentSession;
                currentSession = null;
            }
            return session;
        }

        #endregion

        private readonly ConcurrentBag<ObjectDumpEntry> entries = new ConcurrentBag<ObjectDumpEntry>();

        private CodeExecutionSession() { }

        /// <summary>
        /// The objects that have been dumped in the current session (through the <c>DumpObject</c> method).
        /// </summary>
        public ICollection<object> DumpedObjects
        {
            get { return entries.OrderBy(e => e.DumpTime).Select(e => e.Object).ToList(); }
        }

        /// <summary>
        /// Dumps the specified object (which adds it to the <c>DumpedObjects</c> list).
        /// </summary>
        public void DumpObject(object obj)
        {
            var entry = new ObjectDumpEntry(obj);
            entries.Add(entry);
        }

        /// <summary>
        /// An entry for a dumped object.  
        /// This allows us to compare and sort entries to get a consistent order using a <c>ConcurrentBag</c>.
        /// </summary>
        private class ObjectDumpEntry : IEquatable<ObjectDumpEntry>, IComparable<ObjectDumpEntry>
        {
            private readonly object obj;
            private readonly DateTimeOffset dumpTime;

            public ObjectDumpEntry(object obj)
            {
                this.obj = obj;
                this.dumpTime = DateTimeOffset.UtcNow;
            }

            public object Object { get { return obj; } }
            public DateTimeOffset DumpTime { get { return dumpTime; } }

            public override bool Equals(object obj)
            {
                return Equals(obj as ObjectDumpEntry);
            }

            public bool Equals(ObjectDumpEntry other)
            {
                if (other == null) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.obj, other.obj) && Equals(this.DumpTime, other.DumpTime);
            }

            public override int GetHashCode()
            {
                int hashCode = 7;
                unchecked
                {
                    hashCode = 31 * hashCode + (obj != null ? obj.GetHashCode() : 0);
                    hashCode = 31 * hashCode + dumpTime.GetHashCode();
                }
                return hashCode;
            }

            public int CompareTo(ObjectDumpEntry other)
            {
                return this.DumpTime.CompareTo(other.DumpTime);
            }
        }
    }
}
