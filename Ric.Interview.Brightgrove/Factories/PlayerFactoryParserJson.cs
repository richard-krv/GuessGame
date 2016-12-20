using Newtonsoft.Json.Linq;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using System.Collections.Generic;
using System.Linq;

namespace Ric.Interview.Brightgrove.FruitBasket.Factories
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
