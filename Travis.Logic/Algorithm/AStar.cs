using System;
using System.Collections.Generic;

namespace Travis.Logic.Algorithm
{
    /// <summary>
    /// Represents path found by <see cref="AStar{T}"/> algorithm.
    /// </summary>
    /// <typeparam name="T">Type of vertex.</typeparam>
    public class AStarPath<T>
    {
        /// <summary>
        /// Path found by algorithm.
        /// </summary>
        public List<T> Path { get; set; }

        /// <summary>
        /// Total path length.
        /// </summary>
        public double Length { get; set; }
    }

    /// <summary>
    /// Algorithm which finds the shortes path between vertices in graph.
    /// </summary>
    /// <typeparam name="T">Type of vertex.</typeparam>
    public class AStar<T>
        where T : IVertex
    {
        private IDictionary<T, T> _cameFrom;

        private Dictionary<T, double> _gScore;

        private Dictionary<T, double> _hScore;

        private Dictionary<T, double> _fScore;

        private MinHeap<T> _openQueue;

        private HashSet<T> _openSet;

        private HashSet<T> _closedSet;

        private class VertexComparer : Comparer<T>
        {
            private AStar<T> _parent;

            public VertexComparer(AStar<T> parent)
            {
                _parent = parent;
            }

            public override int Compare(T x, T y)
            {
                var firstVal = _parent._fScore.ContainsKey(x) ? _parent._fScore[x] : double.MaxValue;
                var secondVal = _parent._fScore.ContainsKey(y) ? _parent._fScore[y] : double.MaxValue;
                return firstVal.CompareTo(secondVal);
            }
        }

        /// <summary>
        /// Finds shortest path in graph between given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="heuristic"></param>
        public AStarPath<T> ShortestPath(T from, T to, Func<T, double> heuristic)
        {
            _gScore = new Dictionary<T, double>();
            _hScore = new Dictionary<T, double>();
            _fScore = new Dictionary<T, double>();
            _openQueue = new MinHeap<T>(new VertexComparer(this));
            _openSet = new HashSet<T>();
            _closedSet = new HashSet<T>();
            _cameFrom = new Dictionary<T, T>();
            _openSet.Add(from);
            _openQueue.Add(from);
            _gScore.Add(from, 0);
            _hScore.Add(from, heuristic(from));
            _fScore.Add(from, _gScore[from] + _hScore[from]);
            while (_openQueue.Count > 0)
            {
                T x = _openQueue.Dequeue();
                _openSet.Remove(x);
                _closedSet.Add(x);
                if (to.Equals(x))
                    return ReconstructPath(x);
                foreach (var edge in x.Edges())
                {
                    var y = (T)edge.OutVertex;
                    if (_closedSet.Contains(y))
                        continue;
                    var tentativeGScore = _gScore[x] + edge.Weight;
                    var tentativeIsBetter = false;
                    if (!_openSet.Contains(y))
                    {
                        _openQueue.Add(y);
                        _openSet.Add(y);
                        _hScore.Add(y, heuristic(y));
                        tentativeIsBetter = true;
                    }
                    else if (tentativeGScore < _gScore[y])
                    {
                        tentativeIsBetter = true;
                    }
                    if (tentativeIsBetter)
                    {
                        if (!_cameFrom.ContainsKey(y))
                            _cameFrom.Add(y, x);
                        _cameFrom[y] = x;
                        if (!_gScore.ContainsKey(y))
                            _gScore.Add(y, tentativeGScore);
                        _gScore[y] = tentativeGScore;
                        if (!_fScore.ContainsKey(y))
                            _fScore.Add(y, 0);
                        _fScore[y] = _gScore[y] + _hScore[y];
                    }
                }
            }
            return null;
        }

        private AStarPath<T> ReconstructPath(T node)
        {
            if (_cameFrom.ContainsKey(node))
            {
                var p = ReconstructPath(_cameFrom[node]);
                p.Path.Add(node);
                p.Length = _gScore[node];
                return p;
            }
            else return new AStarPath<T>()
            {
                Length = 0,
                Path = new List<T>() { node }
            };
        }
    }
}
