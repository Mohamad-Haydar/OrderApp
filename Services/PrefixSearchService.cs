namespace OrderApp.Services
{
    public class PrefixSearchService<T>
    {
        private class TrieNode
        {
            public Dictionary<char, TrieNode> Children { get; } = new();
            public List<T> Items { get; } = new();
            public bool IsEndOfWord => Items.Count > 0;
        }

        private readonly TrieNode _root = new();
        private readonly Func<T, string> _keySelector;

        public PrefixSearchService(Func<T, string> keySelector)
        {
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public void Build(IEnumerable<T> items)
        {
            _root.Children.Clear();
            _root.Items.Clear();

            foreach (var item in items)
            {
                var key = _keySelector(item)?.ToLowerInvariant() ?? string.Empty;
                var node = _root;
                foreach (var c in key)
                {
                    if (!node.Children.ContainsKey(c))
                        node.Children[c] = new TrieNode();
                    node = node.Children[c];
                }

                node.Items.Add(item);   
            }
        }

        public List<T> Search(string? prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return CollectAll(_root); // return everything

            var node = _root;
            prefix = prefix.ToLowerInvariant();

            foreach (var c in prefix)
            {
                if (!node.Children.TryGetValue(c, out node))
                    return [];
            }

            return CollectAll(node);
        }

        private List<T> CollectAll(TrieNode node)
        {
            var results = new List<T>();

            if (node.IsEndOfWord)
                results.AddRange(node.Items);

            foreach (var child in node.Children.Values)
                results.AddRange(CollectAll(child));

            return results;
        }
    }
}
