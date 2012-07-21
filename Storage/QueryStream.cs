using System;
using System.Collections.Generic;

namespace Tazqon.Storage
{
    struct QueryStream : IDisposable
    {
        /// <summary>
        /// Stack of working Querys.
        /// </summary>
        public Stack<Query> Querys;

        /// <summary>
        /// Pushes an query on the top of the stack.
        /// </summary>
        /// <param name="Query"></param>
        public void Push(Query Query)
        {
            if (Querys == null)
            {
                Querys = new Stack<Query>();
            }

            Querys.Push(Query);
        }

        /// <summary>
        /// Returns and activates the last query added.
        /// </summary>
        public QueryObject Pop()
        {
            QueryObject Obj = new QueryObject();
            Obj.Push(Querys.Pop());
            return Obj;
        }

        /// <summary>
        /// Poppes from static classes.
        /// </summary>
        /// <param name="Output"></param>
        public void Pop(out object Output)
        {
            QueryObject.Push(Querys.Pop(), out Output);
        }

        /// <summary>
        /// Reverses the stack.
        /// </summary>
        public void Lock()
        {
            var Stack = new Stack<Query>();

            foreach (var Query in this.Querys)
            {
                Stack.Push(Query);
            }

            this.Querys = Stack;
        }

        /// <summary>
        /// Added for using()
        /// </summary>
        public void Dispose()
        {
            foreach (var Query in Querys)
            {
                Query.Dispose();
            }

            this.Querys = null;
        }
    }
}
