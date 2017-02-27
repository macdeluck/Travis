namespace Travis.Logic.Algorithm
{
    /// <summary>
    /// Represents union-find structure.
    /// </summary>
    /// <typeparam name="T">Type of element to union-find.</typeparam>
    public class UnionFind<T>
    {
        /// <summary>
        /// Value of union-find instance.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Parent of union-find instance.
        /// </summary>
        public UnionFind<T> Parent { get; private set; }

        /// <summary>
        /// Creates new union-find instance.
        /// </summary>
        /// <param name="value"></param>
        public UnionFind(T value)
        {
            Value = value;
            Parent = null;
        }

        /// <summary>
        /// Finds root of union-find structure.
        /// </summary>
        public UnionFind<T> Find()
        {
            if (Parent == null)
                return this;
            return Parent.Find();
        }

        /// <summary>
        /// Unions two structures.
        /// </summary>
        /// <param name="other">Other union-find structure.</param>
        public UnionFind<T> Union(UnionFind<T> other)
        {
            var xRoot = other.Find();
            var yRoot = Find();
            if (!ReferenceEquals(xRoot, yRoot))
                xRoot.Parent = yRoot;
            return yRoot;
        }

        /// <summary>
        /// Returns string representation of union find structure.
        /// </summary>
        public override string ToString()
        {
            return$"UnionFind{{{Value}}}";
        }
    }
}
