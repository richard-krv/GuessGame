using Ric.Interview.Brightgrove.FruitBasket.Factories;

namespace Ric.Interview.Brightgrove.FruitBasket
{
    public class Player
    {
    }

   

public class PlayerFactorySimple : IPlayerFactory
{
    public IPlayerRandrom  CreatePlayerRandrom() { return new PlayerRandrom(); }
    public IPlayerMemory  CreatePlayerMemory() { return new ObjA1(); }
    public IPlayerThorough  CreatePlayerThorough() { return new ObjB1(); }
    public IPlayerCheater  CreatePlayerCheater() { return new ObjA1(); }
    public IPlayerThoroughCheater  CreatePlayerThoroughCheater() { return new ObjA1(); }
}

}