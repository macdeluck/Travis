using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Extensions;

namespace Travis.Logic.Algorithm
{
    /// <summary>
    /// Represents matrix graph.
    /// </summary>
    public class MatrixGraph : IGraph
    {
        private double[,] _connectionMatrix;

        private List<MatrixVertex> _vertices;

        /// <summary>
        /// Creates new matrix graph.
        /// </summary>
        /// <param name="numOfVertexes">Number of vertices.</param>
        public MatrixGraph(int numOfVertexes)
        {
            _connectionMatrix = new double[numOfVertexes, numOfVertexes];
            for (int i = 0; i < numOfVertexes; i++)
                for (int j = 0; j < numOfVertexes; j++)
                    _connectionMatrix[i, j] = -1;
            _vertices = Enumerable.Range(0, numOfVertexes).Select(i => new MatrixVertex(this, i)).ToList();
        }

        /// <summary>
        /// Number of graph vertices.
        /// </summary>
        public int NumOfVertexes => _vertices.Count;
        
        /// <summary>
        /// Adds edge to graph.
        /// </summary>
        /// <param name="inVertex">Input edge vertex.</param>
        /// <param name="outVertex">Output edge vertex.</param>
        /// <param name="weight">Edge weight.</param>
        public IEdge AddEdge(IVertex inVertex, IVertex outVertex, double weight)
        {
            if (weight <= 0)
                throw new ArgumentException("Edge weights must be positive");
            CheckVertex(inVertex);
            CheckVertex(outVertex);
            var minVertex = inVertex as MatrixVertex;
            var moutVertex = outVertex as MatrixVertex;
            _connectionMatrix[minVertex.Num, moutVertex.Num] = weight;
            return EdgeAt(minVertex.Num, moutVertex.Num);
        }

        private IEdge EdgeAt(int inVertex, int outVertex)
        {
            if (!EdgeExists(inVertex, outVertex))
                return null;
            return new MatrixEdge(this, VertexAt(inVertex) as MatrixVertex, VertexAt(outVertex) as MatrixVertex);
        }

        /// <summary>
        /// Checks if edge exists between vertices with index.
        /// </summary>
        /// <param name="inVertex">Input graph vertex.</param>
        /// <param name="outVertex">Output graph vertex.</param>
        public bool EdgeExists(int inVertex, int outVertex)
        {
            return _connectionMatrix[inVertex, outVertex] > 0;
        }

        /// <summary>
        /// Removes edge from graph.
        /// </summary>
        /// <param name="edge">Edge to remove.</param>
        public void RemoveEdge(IEdge edge)
        {
            CheckVertex(edge.InVertex);
            CheckVertex(edge.OutVertex);
            _connectionMatrix[(edge.InVertex as MatrixVertex).Num, (edge.OutVertex as MatrixVertex).Num] = -1;
        }

        private void CheckVertex(IVertex vertex)
        {
            if (!(vertex is MatrixVertex))
                throw new ArgumentException("Given vertex is not MatrixVertex");
            var minVertex = vertex as MatrixVertex;
            if (minVertex._parent != this)
                throw new ArgumentException("Given vertex does not belong to this graph");
        }

        /// <summary>
        /// Accesses vertex at index.
        /// </summary>
        /// <param name="vertexNum">Number of vertex to access.</param>
        public IVertex this[int vertexNum]
        {
            get
            {
                return VertexAt(vertexNum);
            }
        }

        private IVertex VertexAt(int vertexNum)
        {
            if (vertexNum < 0 || vertexNum >= NumOfVertexes)
                throw new IndexOutOfRangeException();
            return _vertices[vertexNum];
        }

        /// <summary>
        /// Enumerates graph vertices.
        /// </summary>
        public IEnumerable<IVertex> Vertices()
        {
            return _vertices;
        }

        /// <summary>
        /// Accesses edge by giving vertices numbers.
        /// </summary>
        /// <param name="inNum">Number of input vertice.</param>
        /// <param name="outNum">Number of output vertice.</param>
        public IEdge this[int inNum, int outNum]
        {
            get
            {
                return EdgeAt(inNum, outNum);
            }
        }

        private class MatrixEdge : IEdge
        {
            public MatrixGraph _parent;
            public MatrixVertex _inVertex;
            public MatrixVertex _outVertex;

            public MatrixEdge(MatrixGraph parent, MatrixVertex inVertex, MatrixVertex outVertex)
            {
                _parent = parent;
                _inVertex = inVertex;
                _outVertex = outVertex;
            }

            public IVertex InVertex
            {
                get { return _inVertex; }
            }

            public IVertex OutVertex
            {
                get { return _outVertex; }
            }

            public double Weight
            {
                get
                {
                    if (IsRemoved) throw new InvalidOperationException("Edge has been removed from graph");
                    return _parent._connectionMatrix[_inVertex.Num, _outVertex.Num];
                }
                set
                {
                    if (IsRemoved) throw new InvalidOperationException("Edge has been removed from graph");
                    if (value <= 0)
                        throw new ArgumentException("Edge weights must be positive");
                    _parent._connectionMatrix[_inVertex.Num, _outVertex.Num] = value;
                }
            }

            public bool IsRemoved { get { return _parent.EdgeExists(_inVertex.Num, _outVertex.Num); } }

            public override bool Equals(object obj)
            {
                if (!(obj is MatrixEdge)) return false;
                var other = obj as MatrixEdge;
                return ReferenceEquals(_parent, other._parent) && other.InVertex.Equals(InVertex) && other.OutVertex.Equals(OutVertex);
            }

            public override int GetHashCode()
            {
                return 13 * InVertex.GetHashCode() ^ OutVertex.GetHashCode();
            }

            public override string ToString()
            {
                if (IsRemoved) return "RemovedEdge";
                return "{0} -> {1} ({2})".FormatString(InVertex, OutVertex, Weight);
            }
        }

        private class MatrixVertex : IVertex
        {
            public int Num { get; private set; }

            public MatrixGraph _parent;

            public MatrixVertex(MatrixGraph parent, int num)
            {
                _parent = parent;
                Num = num;
            }

            public IEnumerable<IEdge> Edges()
            {
                for (int i = 0; i < _parent.NumOfVertexes; i++)
                {
                    var weight = _parent._connectionMatrix[Num, i];
                    if (weight > 0)
                        yield return new MatrixEdge(_parent, this, _parent.VertexAt(i) as MatrixVertex);
                }
            }

            public IEdge EdgeTo(IVertex other)
            {
                if (!(other is MatrixVertex)) return null;
                var mother = other as MatrixVertex;
                if (!ReferenceEquals(mother._parent, _parent)) return null;
                return _parent.EdgeAt(Num, mother.Num);
            }

            public override int GetHashCode()
            {
                return Num.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is MatrixVertex)) return false;
                var other = obj as MatrixVertex;
                return ReferenceEquals(_parent, other._parent) && other.Num.Equals(Num);
            }

            public override string ToString()
            {
                return Num.ToString();
            }
        }
    }
}
