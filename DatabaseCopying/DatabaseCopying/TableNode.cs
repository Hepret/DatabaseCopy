using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopying
{
    partial class DatabaseCopy
    {
        private class TableNode
        {
            public List<TableNode> Parents;
            bool copied;
            public string Name { get; private set; }

            public TableNode(string name, List<TableNode> parents = null)
            {
                Name = name;
                Parents = parents ?? new List<TableNode>();
                copied = false;
            }

            public static Queue<TableNode> getQueue(List<TableNode> nodes)
            {
                Queue<TableNode> queue = new Queue<TableNode>();
                foreach (TableNode node in nodes)
                {
                    func(node);
                }
                void func(TableNode node)
                {
                    if (node.copied) return;

                    if (node.Parents != null)
                    {
                        foreach (TableNode parent in node.Parents)
                        {
                            if (!parent.copied) func(parent);
                        }
                    }
                    node.copied = true;
                    queue.Enqueue(node);

                }
                return queue;
            }
            public void AddParent(TableNode parent)
            {
                Parents.Add(parent);
            }
        }
    }
}
