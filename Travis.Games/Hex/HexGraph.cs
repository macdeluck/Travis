using System;
using System.Collections.Generic;
using Travis.Logic.Algorithm;
using Travis.Logic.Extensions;

namespace Travis.Games.Hex
{
    /// <summary>
    /// Graph for <see cref="HexBoard"/>.
    /// </summary>
    public class HexGraph : IGraph
    {
        private HexBoard _board;

        /// <summary>
        /// Graph weight function.
        /// </summary>
        public Func<HexBoard, IVertex, IVertex, double> WeightFunction { get; private set; }

        /// <summary>
        /// True, if graph edges are also considered as vertices.
        /// </summary>
        public bool IncludesEdges { get; private set; }

        /// <summary>
        /// Creates new instance of graph.
        /// </summary>
        /// <param name="board">Source graph board.</param>
        /// <param name="includesEdges">True, if graph edges should be also considered as vertices.</param>
        /// <param name="weightFunction">Graph weight function.</param>
        public HexGraph(HexBoard board, bool includesEdges, Func<HexBoard, IVertex, IVertex, double> weightFunction)
        {
            _board = board;
            WeightFunction = weightFunction;
            IncludesEdges = includesEdges;
        }

        /// <summary>
        /// Number of graph vertices.
        /// </summary>
        public int NumOfVertexes => _board.Size * _board.Size;


        /// <summary>
        /// Adds edge to graph. Not supported for <see cref="HexGraph"/>.
        /// </summary>
        /// <param name="inVertex">Input edge vertex.</param>
        /// <param name="outVertex">Output edge vertex.</param>
        /// <param name="weight">Edge weight.</param>
        public IEdge AddEdge(IVertex inVertex, IVertex outVertex, double weight)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes edge from graph. Not supported for <see cref="HexGraph"/>.
        /// </summary>
        /// <param name="edge">Edge to remove.</param>
        public void RemoveEdge(IEdge edge)
        {
            throw new NotSupportedException();
        }
        
        /// <summary>
        /// Enumerates graph vertices.
        /// </summary>
        public IEnumerable<IVertex> Vertices()
        {
            if (IncludesEdges)
                foreach (var hedge in Enums.Values<HexBoardEdge>())
                    yield return new HexVertex(this, hedge);
            for (int x = 0; x < _board.Size; x++)
                for (int y = 0; y < _board.Size; y++)
                    yield return VertexAt(x, y);
        }

        /// <summary>
        /// Returns vertex at given indices.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        public HexVertex VertexAt(int x, int y)
        {
            return new HexVertex(this, x, y);
        }

        /// <summary>
        /// Gets vertex for board edge.
        /// </summary>
        /// <param name="edge">Hex board edge.</param>
        public HexVertex VertexFromEdge(HexBoardEdge edge)
        {
            if (!IncludesEdges) throw new InvalidOperationException("Graph does not include edge vertexes");
            return new HexVertex(this, edge);
        }

        /// <summary>
        /// Represents hex graph vertex.
        /// </summary>
        public class HexVertex : IVertex
        {
            private HexGraph _parent;

            /// <summary>
            /// The x position of vertex.
            /// </summary>
            public int X { get; private set; }

            /// <summary>
            /// The y position of vertex.
            /// </summary>
            public int Y { get; private set; }

            /// <summary>
            /// Checks if vertice represents board edge.
            /// </summary>
            public bool IsBoardEdge { get; private set; }

            private HexBoardEdge _edge;

            /// <summary>
            /// Gets board edge from which vertice was created.
            /// </summary>
            public HexBoardEdge Edge
            {
                get
                {
                    if (!IsBoardEdge) throw new InvalidOperationException("Vertex is not edge");
                    return _edge;
                }
                private set
                {
                    _edge = value;
                    IsBoardEdge = true;
                }
            }

            /// <summary>
            /// Creates new instance of board-field vertex.
            /// </summary>
            /// <param name="parent">Parent graph.</param>
            /// <param name="x">The x position.</param>
            /// <param name="y">The y position.</param>
            public HexVertex(HexGraph parent, int x, int y)
            {
                _parent = parent;
                IsBoardEdge = false;
                X = x;
                Y = y;
            }

            /// <summary>
            /// Creates new instance of board-edge vertex.
            /// </summary>
            /// <param name="parent">Parent graph.</param>
            /// <param name="edge">Board edge from which vertex is created.</param>
            public HexVertex(HexGraph parent, HexBoardEdge edge)
            {
                _parent = parent;
                IsBoardEdge = true;
                Edge = edge;
            }

            /// <summary>
            /// Edges outgoing from vertex.
            /// </summary>
            public IEnumerable<IEdge> Edges()
            {
                if (!IsBoardEdge)
                    return RegularVertexEdges();
                else return EdgeVertexEdges();
            }

            private IEnumerable<IEdge> RegularVertexEdges()
            {
                var xs = new[] { X - 1, X, X + 1 };
                var ys = new[] { Y - 1, Y, Y + 1 };
                foreach (var x in xs)
                    foreach (var y in ys)
                    {
                        IEnumerable<HexBoardEdge> edges;
                        if (_parent._board.Adjacent(X, Y, x, y))
                        {
                            var v2 = new HexVertex(_parent, x, y);
                            yield return new HexEdge(_parent, this, v2);
                        }
                        else if (_parent.IncludesEdges && _parent._board.AdjacentEdges(X, Y, out edges))
                        {
                            foreach (var hedge in edges)
                            {
                                var v2 = new HexVertex(_parent, hedge);
                                yield return new HexEdge(_parent, this, v2);
                            }
                        }
                    }
            }

            private IEnumerable<IEdge> EdgeVertexEdges()
            {
                if (!_parent.IncludesEdges) throw new InvalidOperationException("Graph does not include edge vertexes");
                Func<int, HexVertex> vCreator;
                switch (Edge)
                {
                    case HexBoardEdge.Left:
                        vCreator = i => new HexVertex(_parent, 0, i);
                        break;
                    case HexBoardEdge.Right:
                        vCreator = i => new HexVertex(_parent, _parent._board.Size - 1, i);
                        break;
                    case HexBoardEdge.Top:
                        vCreator = i => new HexVertex(_parent, i, 0);
                        break;
                    case HexBoardEdge.Bottom:
                        vCreator = i => new HexVertex(_parent, i, _parent._board.Size - 1);
                        break;
                    default:
                        throw new InvalidOperationException("Fatal: Unknown edge");
                }
                for (int i = 0; i < _parent._board.Size; i++)
                {
                    var v2 = vCreator(i);
                    yield return new HexEdge(_parent, this, v2);
                }
            }

            /// <summary>
            /// Returns vertice hash.
            /// </summary>
            public override int GetHashCode()
            {
                return 31 * (int)_edge + 13 * X ^ Y;
            }

            /// <summary>
            /// Checks if object is equal with other.
            /// </summary>
            /// <param name="obj">Other object to compare to.</param>
            public override bool Equals(object obj)
            {
                if (!(obj is HexVertex)) return false;
                var other = obj as HexVertex;
                return ReferenceEquals(other._parent, _parent) &&
                    ((!IsBoardEdge && !other.IsBoardEdge && other.X == X && other.Y == Y) ||
                     (IsBoardEdge && other.IsBoardEdge && other.Edge.Equals(Edge)));
            }

            /// <summary>
            /// Returns string representation of vertex.
            /// </summary>
            public override string ToString()
            {
                if (IsBoardEdge)
                    return "Edge: {0}".FormatString(Edge);
                return "{2} ({0}, {1})".FormatString(X, Y, _parent._board[X, Y]);
            }
        }

        /// <summary>
        /// Represents hex graph edge.
        /// </summary>
        public class HexEdge : IEdge
        {
            private HexGraph _parent;

            /// <summary>
            /// Gets input vertex.
            /// </summary>
            public IVertex InVertex { get; private set; }


            /// <summary>
            /// Gets output vertex.
            /// </summary>
            public IVertex OutVertex { get; private set; }
            
            /// <summary>
            /// Edge weight.
            /// </summary>
            public double Weight
            {
                get
                {
                    return _parent.WeightFunction(_parent._board, InVertex as HexVertex, OutVertex as HexVertex);
                }
            }

            /// <summary>
            /// Creates new edge instance.
            /// </summary>
            /// <param name="parent">Parent graph.</param>
            /// <param name="inVertex">Input edge vertex.</param>
            /// <param name="outVertex">Output edge vertex.</param>
            public HexEdge(HexGraph parent, HexVertex inVertex, HexVertex outVertex)
            {
                _parent = parent;
                InVertex = inVertex;
                OutVertex = outVertex;
            }

            /// <summary>
            /// Returns string representation of object.
            /// </summary>
            public override string ToString()
            {
                return "[{0}] -> [{1}] ({2})".FormatString(InVertex, OutVertex, Weight);
            }
        }
    }

}
