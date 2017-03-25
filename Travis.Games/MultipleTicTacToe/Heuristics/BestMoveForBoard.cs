using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Learning.Model;
using Travis.Logic.Model;

namespace Travis.Games.MultipleTicTacToe.Heuristics
{
    /// <summary>
    /// Optimal strategy for <see cref="MultipleTicTacToe"/> game.
    /// </summary>
    public class BestMoveForBoard : IDefaultPolicy
    {
        TicTacToeBoard _board;
        TicTacToeEntity _player;
        int[] _move;
        int[] _playerRows;
        int[] _playerCols;
        int[] _playerDiagonals;
        int[] _opponentRows;
        int[] _opponentCols;
        int[] _opponentDiagonals;

        private void InitRowsAndCols()
        {
            _playerRows = new int[_board.Size];
            _playerCols = new int[_board.Size];
            _playerDiagonals = new int[2];
            _opponentRows = new int[_board.Size];
            _opponentCols = new int[_board.Size];
            _opponentDiagonals = new int[2];

            for (int x = 0; x < _board.Size; x++)
                for (int y = 0; y < _board.Size; y++)
                {
                    var pl = _board[x, y];
                    if (pl == _player)
                    {
                        _playerRows[y]++;
                        _playerCols[x]++;
                        if (x == y)
                            _playerDiagonals[0]++;
                        if (x == _board.Size - 1 - y)
                            _playerDiagonals[1]++;
                    }
                    else if (pl == _player.Opponent())
                    {
                        _opponentRows[y]++;
                        _opponentCols[x]++;
                        if (x == y)
                            _opponentDiagonals[0]++;
                        if (x == _board.Size - 1 - y)
                            _opponentDiagonals[1]++;
                    }
                }
        }

        private void ChooseMove()
        {
            var tests = new Func<bool>[] 
            {
                Win,
                Block,
                Fork,
                BlockFork,
                // OppositeCornersOnly,
                Center,
                OppositeCorner,
                EmptyCorner,
                EmptySide
            };
            foreach (var test in tests)
                if (test())
                    return;
        }

        private bool OpenCorner()
        {
            if (_board.PlacedNum == 0)
            {
                _move = new[] { 0, 0 };
                return true;
            }
            return false;
        }

        private bool OppositeCornersOnly()
        {
            if (_board.PlacedNum == 2)
            {
                if (_board[0, 0] == _player && _board[2, 2] == _player.Opponent())
                {
                    _move = new[] { 2, 0 };
                    return true;
                }
                else if (_board[2, 0] == _player && _board[0, 2] == _player.Opponent())
                {
                    _move = new[] { 0, 0 };
                    return true;
                }
                else if (_board[2, 2] == _player && _board[0, 0] == _player.Opponent())
                {
                    _move = new[] { 2, 0 };
                    return true;
                }
                else if (_board[0, 2] == _player && _board[2, 0] == _player.Opponent())
                {
                    _move = new[] { 0, 0 };
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Chooses optimal move for given board and player.
        /// </summary>
        /// <param name="board">Board to seek for move on.</param>
        /// <param name="player">Player to move.</param>
        public int[] ChooseMove(TicTacToeBoard board, TicTacToeEntity player)
        {
            _board = board;
            _player = player;
            _move = null;
            InitRowsAndCols();
            ChooseMove();
            return _move;
        }

        private bool Win()
        {
            return WinningMove(_playerRows, _playerCols, _playerDiagonals);
        }

        private bool Block()
        {
            return WinningMove(_opponentRows, _opponentCols, _opponentDiagonals);
        }

        private bool Fork()
        {
            _move = GetFork(true);
            return _move != null;
        }

        private bool BlockFork()
        {
            if (BlockOppositeCornersFork())
                return true;
            _move = GetFork(false);
            return _move != null;
        }

        private bool BlockOppositeCornersFork()
        {
            if (_board.PlacedNum == 3 && 
                _board[1, 1] == _player &&
                (_opponentDiagonals[0] == 2 || _opponentDiagonals[1] == 2))
            {
                _move = new[] { 0, 1 };
                return true;
            }
            return false;
        }

        private bool Center()
        {
            if (_board[1, 1] == TicTacToeEntity.None)
                _move = new[] { 1, 1 };
            return _move != null;
        }

        private bool OppositeCorner()
        {
            var corners = new int[][] {
                new [] { 0, 0 },
                new [] { 2, 0 },
                new [] { 0, 2 },
                new [] { 2, 2 }
            };
            foreach (var corner in corners)
                if (OppositeCorner(corner[0], corner[1]))
                    return true;
            return false;
        }

        private bool EmptyCorner()
        {
            var corners = new int[][] {
                new [] { 0, 0 },
                new [] { 2, 0 },
                new [] { 0, 2 },
                new [] { 2, 2 }
            };
            return EmptyIn(corners);
        }

        private bool EmptySide()
        {
            var sides = new int[][] {
                new [] { 0, 1 },
                new [] { 1, 0 },
                new [] { 1, 2 },
                new [] { 2, 1 }
            };
            return EmptyIn(sides);
        }

        private bool EmptyIn(int[][] fieldsToCheck)
        {
            foreach (var field in fieldsToCheck)
                if (_board[field[0], field[1]] == TicTacToeEntity.None)
                {
                    _move = field;
                    return true;
                }
            return false;
        }

        private bool WinningMove(int[] rows, int[] cols, int[] diagonals)
        {
            for (int y = 0; y < rows.Length; y++)
                if (rows[y] == rows.Length - 1)
                {
                    _move = FirstEmptyInRow(y);
                    if (_move == null) continue;
                    return true;
                }
            for (int x = 0; x < cols.Length; x++)
                if (cols[x] == cols.Length - 1)
                {
                    _move = FirstEmptyInCol(x);
                    if (_move == null) continue;
                    return true;
                }
            for (int i = 0; i < diagonals.Length; i++)
                if (diagonals[i] == _board.Size - 1)
                {
                    _move = FirstEmptyInDiagonal(i);
                    if (_move == null) continue;
                    return true;
                }
            return false;
        }

        private int[] FirstEmptyInRow(int y)
        {
            for (int x = 0; x < _board.Size; x++)
                if (_board[x, y] == TicTacToeEntity.None)
                    return new[] { x, y };
            return null;
        }

        private int[] FirstEmptyInCol(int x)
        {
            for (int y = 0; y < _board.Size; y++)
                if (_board[x, y] == TicTacToeEntity.None)
                    return new[] { x, y };
            return null;
        }

        private int[] FirstEmptyInDiagonal(int diagNum)
        {
            for (int i = 0; i < _board.Size; i++)
            {
                var j = diagNum == 0 ? i : _board.Size - 1 - i;
                if (_board[i, j] == TicTacToeEntity.None)
                    return new[] { i, j };
            }
            return null;
        }

        private int[] GetFork(bool isPlayer)
        {
            for (int x = 0; x < _board.Size; x++)
                for (int y = 0; y < _board.Size; y++)
                    if (_board[x, y] == TicTacToeEntity.None)
                    {
                        if (CanForkAt(isPlayer, x, y))
                        {
                            return new[] { x, y };
                        }
                    }
            return null;
        }

        private bool CanForkAt(bool isPlayer, int x, int y)
        {
            if (CanForkRowAndCol(isPlayer, x, y))
                return true;
            if (CanForkRowAndDiagnal(isPlayer, x, y))
                return true;
            if (CanForkColAndDiagonal(isPlayer, x, y))
                return true;
            return false;
        }

        private bool CanForkRowAndCol(bool isPlayer, int x, int y)
        {
            if (isPlayer)
            {
                return _board[x, y] == TicTacToeEntity.None &&
                    _playerRows[y] == 1 &&
                    _playerCols[x] == 1 &&
                    _opponentRows[y] == 0 &&
                    _opponentCols[x] == 0;
            }
            else
            {
                return _board[x, y] == TicTacToeEntity.None &&
                    _opponentRows[y] == 1 &&
                    _opponentCols[x] == 1 &&
                    _playerRows[y] == 0 &&
                    _playerCols[x] == 0;
            }
        }

        private bool CanForkRowAndDiagnal(bool isPlayer, int x, int y)
        {
            var diagonals = DiagonalsFrom(x, y);
            if (!diagonals.Any())
                return false;
            foreach (var d in diagonals)
            {
                if (isPlayer)
                {
                    if (_board[x, y] == TicTacToeEntity.None &&
                        _playerRows[y] == 1 &&
                        _playerDiagonals[d] == 1 &&
                        _opponentRows[y] == 0 &&
                        _opponentDiagonals[d] == 0)
                        return true;
                }
                else
                {
                    if (_board[x, y] == TicTacToeEntity.None &&
                        _opponentRows[y] == 1 &&
                        _opponentDiagonals[d] == 1 &&
                        _playerRows[y] == 0 &&
                        _playerDiagonals[d] == 0)
                        return true;
                }
            }
            return false;
        }

        private bool CanForkColAndDiagonal(bool isPlayer, int x, int y)
        {
            var diagonals = DiagonalsFrom(x, y);
            if (!diagonals.Any())
                return false;
            foreach (var d in diagonals)
            {
                if (isPlayer)
                {
                    if (_board[x, y] == TicTacToeEntity.None &&
                        _playerCols[x] == 1 &&
                        _playerDiagonals[d] == 1 &&
                        _opponentCols[x] == 0 &&
                        _opponentDiagonals[d] == 0)
                        return true;
                }
                else
                {
                    if (_board[x, y] == TicTacToeEntity.None &&
                        _opponentCols[x] == 1 &&
                        _opponentDiagonals[d] == 1 &&
                        _playerCols[x] == 0 &&
                        _playerDiagonals[d] == 0)
                        return true;
                }
            }
            return false;
        }

        private IEnumerable<int> DiagonalsFrom(int x, int y)
        {
            if (x == y)
                yield return 0;
            if (x == _board.Size - 1 - y)
                yield return 1;
        }

        private bool OppositeCorner(int x, int y)
        {
            var nx = _board.Size - 1 - x;
            var ny = _board.Size - 1 - y;
            if (_board[x, y] == _player && _board[nx, ny] == TicTacToeEntity.None)
            {
                _move = new[] { nx, ny };
            }
            return _move != null;
        }

        /// <summary>
        /// Selects action for particular actor for given state below game tree.
        /// </summary>
        /// <param name="state">A state of game.</param>
        /// <param name="actorId">Actor id action should be selected for.</param>
        public IAction Invoke(IState state, int actorId)
        {
            var mstate = state as MultipleTicTacToeState;
            if (mstate.ControlPlayer != actorId)
                return mstate.GetActionsForActor(actorId).Values.First();
            ChooseMove(mstate.Boards[MultipleTicTacToeState.WinningBoard], mstate.ControlTicTacToePlayer);
            var actions = state.GetActionsForActor(actorId).Values.OfType<MultipleTicTacToeAction>()
                .Where(a => a.BoardNum == MultipleTicTacToeState.WinningBoard).ToList();
            return actions.First(a => a.PosX == _move[0] && a.PosY == _move[1]);
        }
    }
}
