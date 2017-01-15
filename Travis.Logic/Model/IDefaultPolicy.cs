﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travis.Logic.Model
{
    /// <summary>
    /// Represents default policy.
    /// </summary>
    public interface IDefaultPolicy
    {
        /// <summary>
        /// Selects action for particular actor for given state below game tree.
        /// </summary>
        /// <param name="state">A state of problem.</param>
        /// <param name="actorId">Actor id action should be selected for.</param>
        IAction Invoke(IState state, int actorId);
    }
}
