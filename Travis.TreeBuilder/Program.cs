using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travis.Games.GreedyNumbers;
using Travis.Logic;
using Travis.Logic.BudgetProviders;
using Travis.Logic.Model;

namespace Travis.TreeBuilder
{
    /// <summary>
    /// Class which contains program entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        public static void Main()
        {
            var processor = new TreeSearchProcessor();
            var tree = new TreeNode();
            var problem = new GreedyNumbers(2, new Dictionary<int, int>() { { 2, 3 }, { 7, 1 } });
            processor.Process(tree, problem, 1000);
        }
    }
}
