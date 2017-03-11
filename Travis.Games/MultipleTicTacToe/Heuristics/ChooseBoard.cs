using System;
using System.Linq;
using Travis.Logic.Extensions;
using Travis.Logic.Learning.Model;
using Travis.Logic.Model;

namespace Travis.Games.MultipleTicTacToe.Heuristics
{
    /// <summary>
    /// Heuristic which chooses unfinished board by player, and tries to win it.
    /// </summary>
    public class ChooseBoard : IDefaultPolicy
    {
        private IAction FinishBoard(TicTacToeEntity player, int actorId, int board, MultipleTicTacToeState mstate)
        {
            //Console.WriteLine(mstate.Boards[board]);
            //Console.WriteLine("======");
            var moveForBoardAlg = new BestMoveForBoard();
            var move = moveForBoardAlg.ChooseMove(mstate.Boards[board], player);
            var actions = mstate.GetActionsForActor(actorId);
            return actions.Values.OfType<MultipleTicTacToeAction>().FirstOrDefault(
                a => a.BoardNum == board &&
                a.PosX == move[0] &&
                a.PosY == move[1]);
        }
        
        private int NotFinishedBoard(TicTacToeEntity ticTacToePlayer, MultipleTicTacToeState mstate)
        {
            int i;
            for (i = 0; i < mstate.Boards.Length; i++)
                if (i != MultipleTicTacToeState.WinningBoard && !mstate.Boards[i].Winner.HasValue && mstate.Boards[i].PlacedNum > 0 && AnyFieldSetByPlayer(ticTacToePlayer, mstate.Boards[i]))
                    return i;
            i = MultipleTicTacToeState.WinningBoard;
            if (!mstate.Boards[i].Winner.HasValue && mstate.Boards[i].PlacedNum > 0 && AnyFieldSetByPlayer(ticTacToePlayer, mstate.Boards[i]))
                return i;
            return -1;
        }

        private bool AnyFieldSetByPlayer(TicTacToeEntity ticTacToePlayer, TicTacToeBoard ticTacToeBoard)
        {
            for (int x = 0; x < ticTacToeBoard.Size; x++)
                for (int y = 0; y < ticTacToeBoard.Size; y++)
                    if (ticTacToeBoard[x, y] == ticTacToePlayer)
                        return true;
            return false;
        }

        /// <summary>
        /// Selects action for particular actor for given state below game tree.
        /// </summary>
        /// <param name="state">A state of game.</param>
        /// <param name="actorId">Actor id action should be selected for.</param>
        public IAction Invoke(IState state, int actorId)
        {
            if (!(state is MultipleTicTacToeState)) throw new ArgumentException("State is does not belong to Multiple TicTacToe game");
            var mstate = state as MultipleTicTacToeState;
            if (mstate.ControlPlayer != actorId)
                return mstate.GetActionsForActor(actorId).Values.RandomElement();
            var player = actorId == 0 ? TicTacToeEntity.X : TicTacToeEntity.O;
            var board = NotFinishedBoard(player, mstate);
            if (board < 0)
                return mstate.GetActionsForActor(actorId).Values.RandomElement();
            else return FinishBoard(player, actorId, board, mstate);
        }
    }
}
