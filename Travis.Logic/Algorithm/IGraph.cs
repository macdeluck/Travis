using System.Collections.Generic;

namespace Travis.Logic.Algorithm
{
    /// <summary>
    /// Represents graph edge.
    /// </summary>
    public interface IEdge
    {
        /// <summary>
        /// Gets input vertex.
        /// </summary>
        IVertex InVertex { get; }

        /// <summary>
        /// Gets output vertex.
        /// </summary>
        IVertex OutVertex { get; }

        /// <summary>
        /// Edge weight.
        /// </summary>
        double Weight { get; }
    }

    /// <summary>
    /// Represents graph vertex.
    /// </summary>
    public interface IVertex
    {
        /// <summary>
        /// Edges outgoing from vertex.
        /// </summary>
        IEnumerable<IEdge> Edges();
    }

    /// <summary>
    /// Interface for graph.
    /// </summary>
    public interface IGraph
    {
        /// <summary>
        /// Number of graph vertices.
        /// </summary>
        int NumOfVertexes { get; }

        /// <summary>
        /// Enumerates graph vertices.
        /// </summary>
        IEnumerable<IVertex> Vertices();

        /// <summary>
        /// Adds edge to graph.
        /// </summary>
        /// <param name="inVertex">Input edge vertex.</param>
        /// <param name="outVertex">Output edge vertex.</param>
        /// <param name="weight">Edge weight.</param>
        IEdge AddEdge(IVertex inVertex, IVertex outVertex, double weight);

        /// <summary>
        /// Removes edge from graph.
        /// </summary>
        /// <param name="edge">Edge to remove.</param>
        void RemoveEdge(IEdge edge);
    }
}
