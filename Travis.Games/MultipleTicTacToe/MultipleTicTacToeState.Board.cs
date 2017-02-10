using System;
using System.Linq;

namespace Travis.Games.MultipleTicTacToe
{
    /// <summary>
    /// Represents <see cref="MultipleTicTacToe"/> player.
    /// </summary>
    public enum MTTTPlayer
    {
        /// <summary>
        /// None.
        /// </summary>
        None = ' ',

        /// <summary>
        /// X player.
        /// </summary>
        XPlayer = 'X',

        /// <summary>
        /// Y player.
        /// </summary>
        YPlayer = 'Y'
    }

    /// <summary>
    /// Extensions to <see cref="MTTTPlayer"/> type.
    /// </summary>
    public static class MTTTPlayerExtensions
    {
        /// <summary>
        /// Returns opponent of player.
        /// </summary>
        /// <param name="player">Player whose opponent should be returned.</param>
        public static MTTTPlayer Opponent(this MTTTPlayer player)
        {
            return player == MTTTPlayer.XPlayer ? MTTTPlayer.YPlayer : player == MTTTPlayer.YPlayer ? MTTTPlayer.XPlayer : MTTTPlayer.None;
        }
    }

    public partial class MultipleTicTacToeState
    {
        /// <summary>
        /// Board associated with state.
        /// </summary>
        private MTTTPlayer[,] BoardData { get; set; }

        private const int Length = 9;

        /// <summary>
        /// Single board length.
        /// </summary>
        public const int Span = 3;

        /// <summary>
        /// Returns number of available boards.
        /// </summary>
        public const int BoardsNum = 9;

        /// <summary>
        /// Returns number of winning board.
        /// </summary>
        public const int WinningBoard = 4;

        /// <summary>
        /// Represents one of available boards.
        /// </summary>
        public class Board
        {
            /// <summary>
            /// Creates new instance of board.
            /// </summary>
            /// <param name="owner">Owner state.</param>
            /// <param name="number">Number of board.</param>
            /// <param name="minX">Min x span.</param>
            /// <param name="minY">Min y span.</param>
            public Board(MultipleTicTacToeState owner, int number, int minX, int minY)
            {
                _owner = owner;
                _minX = minX;
                _minY = minY;
                _number = number;
            }

            private MultipleTicTacToeState _owner;

            private int _minX;

            private int _minY;

            private int _number;

            /// <summary>
            /// Returns board number.
            /// </summary>
            public int Number => _number;

            /// <summary>
            /// Gets board field content on particular position.
            /// </summary>
            /// <param name="x">X position.</param>
            /// <param name="y">Y position.</param>
            public MTTTPlayer this[int x, int y]
            {
                get
                {
                    if (x >= Span)
                        throw new ArgumentOutOfRangeException(nameof(x));
                    if (y >= Span)
                        throw new ArgumentOutOfRangeException(nameof(y));
                    return _owner.BoardData[_minX + x, _minY + y];
                }
            }
        }

        private void SetBoardField(int boardNum, int x, int y, MTTTPlayer value)
        {
            BoardData[(boardNum % Span) * Span + x, (boardNum / Span) * Span] = value;
        }

        private Board[] _boards;

        private MTTTPlayer _winningPlayer;

        private bool? _isTerminal;

        private void InitBoards()
        {
            if (_boards == null)
            {
                _boards = new Board[BoardsNum];
                for (int i = 0; i < BoardsNum; i++)
                    _boards[i] = new Board(this, i, (i % Span) * Span, (i / Span) * Span);
            }
        }

        /// <summary>
        /// Returns board under given number.
        /// </summary>
        /// <param name="number">Number of board to return.</param>
        public Board GetBoard(int number)
        {
            InitBoards();
            return _boards[number];
        }

        private void ClearWinningBoardData()
        {
            _winningPlayer = MTTTPlayer.None;
            _isTerminal = null;
        }

        private bool CheckWinningBoardState()
        {
            if (!_isTerminal.HasValue)
            {
                var winningBoard = GetBoard(WinningBoard);
                int[] xsum = Enumerable.Repeat(0, Span).ToArray();
                int[] ysum = Enumerable.Repeat(0, Span).ToArray();
                int xskew = 0;
                int yskew = 0;
                int fieldsFilled = 0;
                for (int x = 0; x < Span; x++)
                    for (int y = 0; y < Span; y++)
                    {
                        int fieldVal = winningBoard[x, y] == MTTTPlayer.XPlayer ? 1 : winningBoard[x, y] == MTTTPlayer.YPlayer ? -1 : 0;
                        if (x == y)
                            xskew += fieldVal;
                        if (x == Span - 1 - y)
                            yskew += fieldVal;
                        xsum[x] += fieldVal;
                        xsum[y] += fieldVal;
                        if (fieldVal != 0)
                            fieldsFilled++;
                    }
                CheckValueForWinningPlayer(xskew);
                CheckValueForWinningPlayer(yskew);
                if (_winningPlayer == MTTTPlayer.None)
                {
                    for (int x = 0; x < Span; x++)
                        CheckValueForWinningPlayer(xsum[x]);
                }
                if (_winningPlayer == MTTTPlayer.None)
                {
                    for (int y = 0; y < Span; y++)
                        CheckValueForWinningPlayer(ysum[y]);
                }
                _isTerminal = _winningPlayer != MTTTPlayer.None || fieldsFilled == Length * Length;
            }
            return _isTerminal.Value;
        }

        private void CheckValueForWinningPlayer(int yskew)
        {
            if (_winningPlayer == MTTTPlayer.None && yskew == Span || yskew == -Span)
                _winningPlayer = yskew > 0 ? MTTTPlayer.XPlayer : MTTTPlayer.YPlayer;
        }
    }
}
