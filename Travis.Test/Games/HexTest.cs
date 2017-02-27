using Microsoft.VisualStudio.TestTools.UnitTesting;
using Travis.Games.Hex;

namespace Travis.Test.Games
{
    [TestClass]
    public class HexTest
    {
        [TestMethod]
        public void Hex_WinnerRed()
        {
            var board = new HexBoard(9);
            board[4, 4] = HexEntity.Red;
            board[4, 0] = HexEntity.Red;
            board[4, 2] = HexEntity.Red;
            board[4, 1] = HexEntity.Red;
            board[4, 3] = HexEntity.Red;
            board[4, 5] = HexEntity.Red;
            board[4, 6] = HexEntity.Red;
            board[4, 8] = HexEntity.Red;
            board[4, 7] = HexEntity.Red;
            var hexState = new HexState(0, board);
            Assert.IsTrue(hexState.IsTerminal);
        }

        [TestMethod]
        public void Hex_WinnerBlack()
        {
            var board = new HexBoard(9);
            board[0, 4] = HexEntity.Black;
            board[1, 4] = HexEntity.Black;
            board[2, 4] = HexEntity.Black;
            board[3, 4] = HexEntity.Black;
            board[4, 4] = HexEntity.Black;
            board[5, 4] = HexEntity.Black;
            board[6, 4] = HexEntity.Black;
            board[7, 4] = HexEntity.Black;
            board[8, 4] = HexEntity.Black;
            var hexState = new HexState(0, board);
            Assert.IsTrue(hexState.IsTerminal);
        }

        [TestMethod]
        public void Hex_CheckWinner()
        {
            var board = new HexBoard(9);
            var b = new char[,]
            {
                { 'B', 'B', 'B', 'B', 'B', 'R', 'B', 'R', 'B' },
                { 'R', 'R', 'R', 'B', 'B', 'B', 'B', 'R', 'B' },
                { 'B', 'R', 'R', 'R', 'R', 'R', 'R', 'B', 'B' },
                { 'R', 'B', 'R', 'R', 'B', 'B', 'B', 'B', 'B' },
                { 'B', 'R', 'R', 'B', 'R', 'R', 'R', 'B', 'R' },
                { 'R', 'B', 'R', 'R', 'R', 'B', 'R', 'R', 'R' },
                { 'B', 'R', 'B', 'B', 'R', 'B', 'R', 'R', 'R' },
                { 'B', 'B', 'B', 'R', 'B', 'R', 'B', 'R', 'B' },
                { 'B', 'R', 'B', 'B', 'R', ' ', ' ', ' ', ' ' }
            };
            for (int x = 0; x < b.GetLength(0); x++)
                for (int y = 0; y < b.GetLength(1); y++)
                    if (b[x, y] != ' ')
                        board[y, x] = b[x, y] == 'B' ? HexEntity.Black : HexEntity.Red;
            var state = new HexState(0, board);
            Assert.IsTrue(state.IsTerminal);
            Assert.AreEqual(HexEntity.Red, board.Winner);
        }
    }
}
