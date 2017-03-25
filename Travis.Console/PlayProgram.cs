using CommandLine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Travis.Logic.Contest;
using Travis.Logic.Extensions;
using Travis.Logic.Learning.Model;
using Travis.Logic.MCTS;
using Travis.Logic.Model;

namespace Travis.Console
{
    public class PlayOptions
    {
        [Option("game", Required = true, HelpText = "Name of game to learn")]
        public string GameName { get; set; }
        
        [Option("budget", Required = false, HelpText = "Budget provider name")]
        public string BudgetProviderName { get; set; }

        [Option("num", Required = false, HelpText = "Number of games on single side")]
        public int? NumOfGames { get; set; } = null;

        [OptionList("budget-arguments", Required = false, HelpText = "Budget provider arguments")]
        public IList<string> BudgetProviderArguments { get; set; }

        public List<KeyValuePair<string, string>> BudgetProviderArgumentList => BudgetProviderArguments.Select(a => a.ToKeyValuePair()).ToList();

        [Option("player1", Required = true, HelpText = "Name of player 1")]
        public string P1Name { get; set; }

        [Option("player1-buildstring", Required = false, HelpText = "Player 1 build string")]
        public string P1BuildString { get; set; }

        [Option("player2", Required = true, HelpText = "Name of player 2")]
        public string P2Name { get; set; }

        [Option("player2-buildstring", Required = false, HelpText = "Player 2 build string")]
        public string P2BuildString { get; set; }

        public List<ActorData> GetActorData()
        {
            var actorData = new List<ActorData>();
            if (P1Name.HasValue())
                actorData.Add(ParseActorData(P1Name, P1BuildString));
            if (P2Name.HasValue())
                actorData.Add(ParseActorData(P2Name, P2BuildString));
            return actorData;
        }

        private ActorData ParseActorData(string name, string buildStr)
        {
            var aData = new ActorData() { Name = name };
            if (buildStr.HasValue())
            {
                var index = buildStr.IndexOf('-');
                string namesList = index >= 0 ? buildStr.Substring(0, index) : buildStr;
                string buildString = index >= 0 && index < buildStr.Length - 1 ? buildStr.Substring(index + 1) : null;
                aData.BuildNames = namesList.Split(',').ToList();
                if (buildString.HasValue())
                    aData.BuildObjects = new DictionaryStringParser().Parse(buildString);
            }
            return aData;
        }
    }

    class DictionaryStringParser
    {
        Dictionary<string, Tuple<string, List<KeyValuePair<string, string>>>> result;

        int index = 0;

        string str;

        public Dictionary<string, Tuple<string, List<KeyValuePair<string, string>>>> Parse(string buildString)
        {
            if (buildString == null) throw new ArgumentNullException(nameof(buildString));
            result = new Dictionary<string, Tuple<string, List<KeyValuePair<string, string>>>>();
            str = buildString;
            index = 0;
            ParseDictionary();
            return result;
        }

        private bool Advance()
        {
            return ++index < str.Length;
        }

        private string NextWord()
        {
            SkipSpace();
            if (index < str.Length && !IsWordCharacter(str[index]))
                throw new FormatException($"Unexpected character {str[index]} at position {index}");
            var startIndex = index;
            while (index < str.Length && IsWordCharacter(str[index]))
                Advance();
            return str.Substring(startIndex, index - startIndex);
        }

        private bool IsWordCharacter(char c)
        {
            return char.IsLetterOrDigit(c) || c == '.' || c =='/';
        }

        private char EatCharacter(params char[] chars)
        {
            SkipSpace();
            if (index < str.Length && !chars.Contains(str[index]))
                throw new FormatException($"Unexpected character {str[index]} at position {index}");
            var result = str[index];
            Advance();
            return result;
        }

        private void SkipSpace()
        {
            while (index < str.Length && char.IsWhiteSpace(str[index]))
                Advance();
        }

        private void ParseDictionary()
        {
            while (index < str.Length)
            {
                var key = NextWord().Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (key.Length == 0 || key.Length > 2)
                    throw new Exception("Bad object name specification");
                var keyName = key[0];
                var keyType = key.Length > 1 ? key[1] : key[0];
                var args = new List<KeyValuePair<string, string>>();
                if (EatCharacter(':', ';') == ':')
                {
                    do
                    {
                        var newKey = NextWord();
                        EatCharacter('=');
                        var value = NextWord();
                        args.Add(new KeyValuePair<string, string>(newKey, value));
                    }
                    while (index < str.Length && EatCharacter(',', ';') != ';');
                }
                result.Add(keyName, Tuple.Create(keyType, args));
            }
        }
    }

    public class ActorData
    {
        public string Name { get; set; }

        public List<string> BuildNames { get; set; } = new List<string>();

        public Dictionary<string, Tuple<string, List<KeyValuePair<string, string>>>> BuildObjects { get; set; } = new Dictionary<string, Tuple<string, List<KeyValuePair<string, string>>>>();
    }

    class PlayProgram
    {
        /// <summary>
        /// Runs play program.
        /// </summary>
        /// <param name="options">Learn program options.</param>
        public void Run(PlayOptions options)
        {
            var processor = new MatchmakingProcessor();
            var game = TravisInit.Current.GetObject<IGame>(options.GameName);

            IBudgetProvider budgetProvider;
            if (!options.BudgetProviderName.HasValue())
                budgetProvider = new IterationBasedBudgetProvider(1000);
            else
            {
                if (!options.BudgetProviderArgumentList.Any())
                    budgetProvider = TravisInit.Current.GetObject<IBudgetProvider>(options.BudgetProviderName);
                else budgetProvider = TravisInit.Current.GetObject<IBudgetProvider>(options.BudgetProviderName, options.BudgetProviderArgumentList);
            }

            var actorData = options.GetActorData();
            if (actorData.Count != game.NumberOfActors)
                throw new InvalidOperationException($"Invalid number of actors. Specified {actorData.Count}, expected {game.NumberOfActors}");
            var actors = new List<IActor>(game.NumberOfActors);
            foreach (var adata in actorData)
                actors.Add(BuildActor(adata, actors.Count, budgetProvider));

            var reversedActors = new List<IActor>(game.NumberOfActors);
            foreach (var adata in actorData.Reverse<ActorData>())
                reversedActors.Add(BuildActor(adata, reversedActors.Count, budgetProvider));

            bool sidesSwitched = false;
            var movesList = new List<string>();
            processor.MatchStarted += (g, s, a) =>
            {
                movesList.Clear();
            };
            processor.StateTransition += (g, s, a) =>
            {
                //if (budgetProvider is TimeBasedBudgetProvider)
                //    System.Console.WriteLine($"Iterations: {(budgetProvider as TimeBasedBudgetProvider).IterationsElapsed}");
                movesList.Add(s.ToString());
            };
            processor.MatchFinished += (g, s) =>
            {
                movesList.Add(s.ToString());
                var p = s.GetPayoffs();
                if (!sidesSwitched)
                    System.Console.WriteLine($"{p[0].ToString(CultureInfo.InvariantCulture)}, {p[1].ToString(CultureInfo.InvariantCulture)}");
                else System.Console.WriteLine($"{p[1].ToString(CultureInfo.InvariantCulture)}, {p[0].ToString(CultureInfo.InvariantCulture)}");
                if (p[sidesSwitched ? 1 : 0] == 0.0)
                    foreach (var l in movesList)
                        System.Console.WriteLine(l);
            };
            if (!options.NumOfGames.HasValue)
                processor.Process(game, actors);
            else
            {
                for (int i = 0; i < options.NumOfGames; i++)
                {
                    sidesSwitched = false;
                    processor.Process(game, actors);
                    sidesSwitched = true;
                    processor.Process(game, reversedActors);
                }
            }
        }

        private IActor BuildActor(ActorData data, int actorId, IBudgetProvider budgetProvider)
        {
            var actor = TravisInit.Current.GetObject<IActor>(data.Name);

            if (actor is MCTSActor)
            {
                var mctsActor = actor as MCTSActor;
                mctsActor.PlayTimeBudget = budgetProvider;
                var objects = BuildObjects(data.BuildObjects);
                if (data.BuildNames.Any())
                {
                    var policies = data.BuildNames.Select(name => objects.ContainsKey(name)
                                    ? objects[name] as IDefaultPolicy
                                    : TravisInit.Current.GetObject<IDefaultPolicy>(name)).ToList();
                    if (actorId > 0)
                        policies.Reverse();
                    mctsActor.ActionSelectors = policies.Select((p, i) => new { Key = i, Value = p })
                        .ToDictionary(kv => kv.Key, kv => new ActionSelector()
                        { TreePolicy = new UCT(), DefaultPolicy = kv.Value });
                }
            }
            return actor;
        }

        private Dictionary<string, object> BuildObjects(Dictionary<string, Tuple<string, List<KeyValuePair<string, string>>>> buildObjects)
        {
            var objects = new Dictionary<string, object>();
            var objectsToBuild = new Queue<string>(buildObjects.Keys);
            var objectsToBuildNextRound = new List<string>();
            var lastObjectsToBuildNextRoundCount = 0;
            while (objectsToBuild.Any() || objectsToBuildNextRound.Any())
            {
                if (!objectsToBuild.Any())
                {
                    if (lastObjectsToBuildNextRoundCount == objectsToBuildNextRound.Count)
                        throw new Exception($"Unable to build {lastObjectsToBuildNextRoundCount} objects");
                    foreach (var o in objectsToBuildNextRound)
                        objectsToBuild.Enqueue(o);
                    objectsToBuildNextRound.Clear();
                }

                bool skipObject = false;
                var objToBuild = objectsToBuild.Dequeue();
                var obj = TravisInit.Current.GetObject(buildObjects[objToBuild].Item1);
                var objType = obj.GetType();
                foreach (var prop in buildObjects[objToBuild].Item2)
                {
                    var propInfo = objType.GetProperty(prop.Key);
                    if (propInfo == null)
                        throw new Exception($"Unknown property {prop.Key} for object of type {objType.Name}");
                    var setMethod = propInfo.GetSetMethod();
                    if (setMethod == null)
                        throw new Exception($"Property {prop.Key} for object of type {objType.Name} has not accessible set method");
                    try
                    {
                        setMethod.Invoke(obj, new object[] { Convert.ChangeType(prop.Value, propInfo.PropertyType, CultureInfo.InvariantCulture) });
                    }
                    catch
                    {
                        if (!buildObjects.ContainsKey(prop.Value))
                            objects.Add(prop.Value, TravisInit.Current.GetObject(prop.Value));
                        if (objects.ContainsKey(prop.Value))
                            setMethod.Invoke(obj, new object[] { objects[prop.Value] });
                        else
                        {
                            skipObject = true;
                            objectsToBuildNextRound.Add(objToBuild);
                            break;
                        }
                    }
                }
                if (!skipObject)
                    objects.Add(objToBuild, obj);
            }
            return objects;
        }
    }
}
