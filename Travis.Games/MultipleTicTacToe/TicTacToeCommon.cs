using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travis.Logic.Extensions;

namespace Travis.Games.MultipleTicTacToe
{
    /// <summary>
    /// Represents Tic-Tac-Toe game field or player.
    /// </summary>
    public enum TicTacToeEntity
    {
        /// <summary>
        /// Empty field / no player.
        /// </summary>
        None = 0,

        /// <summary>
        /// Field captured by X player.
        /// </summary>
        X = 1,

        /// <summary>
        /// Field captured by O player.
        /// </summary>
        O = 2
    }

    /// <summary>
    /// Extension methods for <see cref="TicTacToeEntity"/>.
    /// </summary>
    public static class _TicTacToeFieldExtensions
    {
        /// <summary>
        /// Gets opponent for player.
        /// </summary>
        /// <param name="player"></param>
        public static TicTacToeEntity Opponent(this TicTacToeEntity player)
        {
            switch (player)
            {
                case TicTacToeEntity.None:
                    return TicTacToeEntity.None;
                case TicTacToeEntity.X:
                    return TicTacToeEntity.O;
                case TicTacToeEntity.O:
                    return TicTacToeEntity.X;
                default:
                    return TicTacToeEntity.None;
            }
        }
    }

    /// <summary>
    /// Represents Tic-Tac-Toe board.
    /// </summary>
    public class TicTacToeBoard
    {
        private TicTacToeEntity[,] Fields { get; set; }

        /// <summary>
        /// Board winner.
        /// </summary>
        public TicTacToeEntity? Winner { get; private set; }

        /// <summary>
        /// Number of captured fields.
        /// </summary>
        public int PlacedNum { get; private set; }

        /// <summary>
        /// Accesses field on given position.
        /// </summary>
        /// <param name="xpos">X position.</param>
        /// <param name="ypos">Y position.</param>
        public TicTacToeEntity this[int xpos, int ypos]
        {
            get
            {
                return Fields[xpos, ypos];
            }
            set
            {
                if (value == TicTacToeEntity.None) throw new InvalidOperationException("Couldn't place empty tile on board");
                if (Fields[xpos, ypos] != TicTacToeEntity.None) throw new InvalidOperationException("The place at position ({0}, {1}) is already occupied".FormatString(xpos, ypos));
                Fields[xpos, ypos] = value;
                PlacedNum++;
                if (PlacedNum > 2)
                    CheckLine(xpos, ypos);
                if (PlacedNum == 9 && !Winner.HasValue)
                    Winner = TicTacToeEntity.None;
            }
        }

        private void CheckLine(int xpos, int ypos)
        {
            TicTacToeEntity expected = Fields[xpos, ypos];
            CheckCol(expected, xpos);
            CheckRow(expected, ypos);
            CheckDiagonal(expected, xpos, ypos);
        }

        private void CheckCol(TicTacToeEntity expected, int xpos)
        {
            if (Winner.HasValue) return;
            for (int y = 0; y < Size; y++)
                if (Fields[xpos, y] != expected)
                    return;
            Winner = expected;
        }

        private void CheckRow(TicTacToeEntity expected, int ypos)
        {
            if (Winner.HasValue) return;
            for (int x = 0; x < Size; x++)
                if (Fields[x, ypos] != expected)
                    return;
            Winner = expected;
        }

        private void CheckDiagonal(TicTacToeEntity expected, int xpos, int ypos)
        {
            if (Winner.HasValue) return;
            if (xpos == ypos)
            {
                int i;
                for (i = 0; i < Size; i++)
                    if (Fields[i, i] != expected)
                        break;
                if (i == Size)
                    Winner = expected;
            }
            if (xpos == Size - 1 - ypos)
            {
                int i;
                for (i = 0; i < Size; i++)
                    if (Fields[i, Size - 1 - i] != expected)
                        break;
                if (i == Size)
                    Winner = expected;
            }
        }

        /// <summary>
        /// Returns board size.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Creates new instance of <see cref="TicTacToeBoard"/>.
        /// </summary>
        /// <param name="size">Size of board.</param>
        public TicTacToeBoard(int size)
        {
            Size = size;
            Fields = new TicTacToeEntity[Size, Size];
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    Fields[i, j] = TicTacToeEntity.None;
            Winner = null;
        }

        /// <summary>
        /// Returns string representation of board.
        /// </summary>
        public string[] Serialized
        {
            get
            {
                var result = new List<string>();
                for (int y = 0; y < Size; y++)
                {
                    result.Add(string.Join(" ", Enumerable.Range(0, Size).Select(x => Fields[x, y] == TicTacToeEntity.None ? " " : Fields[x, y].ToString())));
                }
                return result.ToArray();
            }
        }

        /// <summary>
        /// Returns total fields count.
        /// </summary>
        public int FieldsCount { get { return Size * Size; } }

        /// <summary>
        /// Returns board string representation.
        /// </summary>
        public override string ToString()
        {
            return string.Join("\n", this.Serialized);
        }

        /// <summary>
        /// Clones board.
        /// </summary>
        public TicTacToeBoard Clone()
        {
            return new TicTacToeBoard(Size)
            {
                Fields = Fields.CloneArray(),
                Size = Size,
                PlacedNum = PlacedNum,
                Winner = Winner
            };
        }
    }
}
