using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Travis.Logic.Extensions;

namespace Travis.Games.Hex
{
    /// <summary>
    /// Represents hex game field content or player indicator.
    /// </summary>
    public enum HexEntity
    {
        /// <summary>
        /// No player / empty hex field.
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Red player / field captured by red player.
        /// </summary>
        Red = 1,

        /// <summary>
        /// Black player / field captured by black player.
        /// </summary>
        Black = 2
    }

    /// <summary>
    /// Represents hex board edge.
    /// </summary>
    public enum HexBoardEdge
    {
        /// <summary>
        /// Left board edge.
        /// </summary>
        Left,

        /// <summary>
        /// Right board edge.
        /// </summary>
        Right,

        /// <summary>
        /// Top board edge.
        /// </summary>
        Top,

        /// <summary>
        /// Bottom board edge.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Represents hex board.
    /// </summary>
    public class HexBoard
    {
        /// <summary>
        /// Board fields.
        /// </summary>
        private HexEntity[,] Fields { get; set; }
        
        /// <summary>
        /// Creates new board with given size.
        /// </summary>
        /// <param name="size">The size of board.</param>
        public HexBoard(int size)
        {
            Size = size;
            Fields = new HexEntity[Size, Size];
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    Fields[i, j] = HexEntity.Empty;
        }

        /// <summary>
        /// Gets size of board.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Clones itself.
        /// </summary>
        public HexBoard Clone()
        {
            return new HexBoard(Size)
            {
                Fields = Fields.CloneArray(),
            };
        }

        /// <summary>
        /// Accesses board field on given position.
        /// </summary>
        /// <param name="x">The x index of position.</param>
        /// <param name="y">The y index of position.</param>
        public HexEntity this[int x, int y]
        {
            get
            {
                return Fields[x, y];
            }
            set
            {
                if (Fields[x, y] != HexEntity.Empty) throw new ArgumentException("Field ({0}, {1}) is already occupied".FormatString(x, y));
                if (value == HexEntity.Empty) throw new ArgumentException("HexField cannot be cleared");
                Fields[x, y] = value;
            }
        }

        /// <summary>
        /// Checks if field with given position lays within board.
        /// </summary>
        /// <param name="x">The x index of position.</param>
        /// <param name="y">The y index of position.</param>
        public bool OnBoard(int x, int y)
        {
            return x >= 0 && x < Size && y >= 0 && y < Size;
        }

        /// <summary>
        /// Checks if fields with given positions are adjacent.
        /// </summary>
        /// <param name="x1">The x index of first position.</param>
        /// <param name="y1">The y index of first position.</param>
        /// <param name="x2">The x index of second position.</param>
        /// <param name="y2">The y index of second position.</param>
        public bool Adjacent(int x1, int y1, int x2, int y2)
        {
            if (!OnBoard(x1, y1) || !OnBoard(x2, y2))
                return false;
            if (x1 == x2 && y1 == y2) return false;
            if (Math.Abs(x1 - x2) == 0 || Math.Abs(y1 - y2) == 0)
                return true;
            if (x2 - x1 == 1 && y1 - y2 == 1)
                return true;
            if (x1 - x2 == 1 && y2 - y1 == 1)
                return true;
            return false;
        }

        /// <summary>
        /// Checks if there is path between fields with given positions.
        /// </summary>
        /// <param name="x1">The x index of first position.</param>
        /// <param name="y1">The y index of first position.</param>
        /// <param name="x2">The x index of second position.</param>
        /// <param name="y2">The y index of second position.</param>
        public bool Connected(int x1, int y1, int x2, int y2)
        {
            return Adjacent(x1, y1, x2, y2) && Fields[x1, y1] == Fields[x2, y2];
        }
        
        /// <summary>
        /// Returns string representation of board.
        /// </summary>
        public string[] Serialize()
        {
            var result = new string[Size + 1];
            result[0] = "  " + Enumerable.Range(0, Size).JoinString(" ");
            for (int y = 0; y < Size; y++)
                result[y + 1] = $"{y}" + Enumerable.Range(0, Size).Select(x => Fields[x, y] == HexEntity.Empty ? " " : Fields[x, y] == HexEntity.Red ? "R" : "B").JoinString(" ");
            return result;
        }

        /// <summary>
        /// Returns string representation of board.
        /// </summary>
        public override string ToString()
        {
            return Serialize().JoinString("\n");
        }

        /// <summary>
        /// Returns edges for whose field is adjacent.
        /// </summary>
        /// <param name="x">The x position of field.</param>
        /// <param name="y">The y position of field.</param>
        /// <param name="edges">Edges for whose field is adjacent.</param>
        public bool AdjacentEdges(int x, int y, out IEnumerable<HexBoardEdge> edges)
        {
            var listedges = new List<HexBoardEdge>(4);
            if (x <= 0)
                listedges.Add(HexBoardEdge.Left);
            if (x >= Size - 1)
                listedges.Add(HexBoardEdge.Right);
            if (y <= 0)
                listedges.Add(HexBoardEdge.Top);
            if (y >= Size - 1)
                listedges.Add(HexBoardEdge.Bottom);
            edges = listedges;
            return listedges.Any();
        }
    }
}
