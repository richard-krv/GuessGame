using Newtonsoft.Json.Linq;
using Ric.GuessGame.Models;
using System.Collections.Generic;
using System.Linq;

namespace Ric.GuessGame.Factories
{
    public sealed class PlayerFactoryParserJson
    {
        public static IEnumerable<IParserPlayer> NewJsonPlayer(string json) 
        {
            var playerCollection = JObject.Parse(json);
            var players = playerCollection.SelectToken(PlayersJsonObjectName)
                .Select(p => p.ToObject<ParserPlayer>());
            return players;
        }

        public const string PlayersJsonObjectName = "players";
    }
}
