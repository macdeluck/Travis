using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travis.Logic.Model;

namespace Travis.Games.GreedyNumbers
{
    /// <summary>
    /// Represents action for <see cref="GreedyNumbers"/> game.
    /// </summary>
    public class GreedyNumbersAction : IAction
    {
        public bool IsNoop { get; set; }

        public int PickValue { get; set; }

        public int ActionId { get; set; }

        public int ActorId { get; set; }
    }
}
