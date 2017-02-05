using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Travis.Console.Commandline;
using Travis.Logic.Learning.Model;

namespace Travis.Console
{
    /// <summary>
    /// Represents learning program.
    /// </summary>
    public class LearnProgram
    {
        /// <summary>
        /// Runs learn program.
        /// </summary>
        /// <param name="options">Learn program options.</param>
        public void Run(LearnCommandOptions options)
        {
            var context = ContextRegistry.GetContext();
            var type = context.GetType(options.BudgetProviderName);
            var typeConstructor = FindConstructor(type, options);
            if (typeConstructor == null)
                throw new Exception();
            var args = CastArguments(typeConstructor, options.BudgetArgumentList);
            var budget = context.GetObject<IBudgetProvider>(options.BudgetProviderName, args);
        }

        private object[] CastArguments(ConstructorInfo typeConstructor, IList<KeyValuePair<string, string>> budgetArgumentList)
        {
            var result = new object[budgetArgumentList.Count];
            var parameterInfos = typeConstructor.GetParameters();
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ChangeType(budgetArgumentList[i].Value, parameterInfos[i].ParameterType);
            }
            return result;
        }

        private static ConstructorInfo FindConstructor(Type type, LearnCommandOptions options)
        {
            var typeConstructors = type.GetConstructors().Where(ctor => ctor.IsPublic && MatchParameters(ctor.GetParameters(), options.BudgetArgumentList));
            return typeConstructors.FirstOrDefault();
        }

        private static bool MatchParameters(ParameterInfo[] parameters, IList<KeyValuePair<string, string>> arguments)
        {
            if (parameters.Length != arguments.Count)
                return false;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Name != arguments[i].Key)
                    return false;
            }
            return true;
        }
    }
}
