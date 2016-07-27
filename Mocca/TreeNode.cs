using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocca {
    class TreeNode : IEnumerable<TreeNode> {
        private readonly Dictionary<string, TreeNode> _children =
                                            new Dictionary<string, TreeNode>();

        public readonly string ID;
        public TreeNode Parent { get; private set; }

        public TreeNode(string id) {
            this.ID = id;
        }

        public TreeNode GetChild(string id) {
            return this._children[id];
        }

        public void Add(TreeNode item) {
            if (item.Parent != null) {
                item.Parent._children.Remove(item.ID);
            }

            item.Parent = this;
            this._children.Add(item.ID, item);
        }

        public IEnumerator<TreeNode> GetEnumerator() {
            return this._children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public int Count {
            get { return this._children.Count; }
        }

        public static TreeNode BuildTree(string tree) {
            var lines = tree.Split(new[] { Environment.NewLine },
                                   StringSplitOptions.RemoveEmptyEntries);

            var result = new TreeNode("TreeRoot");
            var list = new List<TreeNode> { result };

            foreach (var line in lines) {
                var trimmedLine = line.Trim();
                var indent = line.Length - trimmedLine.Length;

                var child = new TreeNode(trimmedLine);
                list[indent].Add(child);

                if (indent + 1 < list.Count) {
                    list[indent + 1] = child;
                } else {
                    list.Add(child);
                }
            }

            return result;
        }

        public static string BuildString(TreeNode tree) {
            var sb = new StringBuilder();

            BuildString(sb, tree, 0);

            return sb.ToString();
        }

        private static void BuildString(StringBuilder sb, TreeNode node, int depth) {
            sb.AppendLine(node.ID.PadLeft(node.ID.Length + depth));

            foreach (var child in node) {
                BuildString(sb, child, depth + 1);
            }
        }
    }
}
