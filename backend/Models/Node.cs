namespace backend.Models
{
    public class Node : ZipEntry
    {
        public List<Node> Children { get; set; } = new List<Node>();
        public bool Opened { get; set; } = true;

        public Node() { }

        public Node(ZipEntry node)
        {
            Parent = node.Parent;
            Name = node.Name;
        }
    }
}
