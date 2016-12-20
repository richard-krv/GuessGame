using Ric.Interview.Brightgrove.FruitBasket.Factories.AbstractFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.Factories
{
    public interface IPlayerFactory
    {
        IPlayerRandrom CreatePlayerRandrom(IPlayerConstructorParams prm);
        IPlayerMemory CreatePlayerMemory(IPlayerConstructorParams prm);
        IPlayerThorough CreatePlayerThorough(IPlayerConstructorParams prm);
        IPlayerCheater CreatePlayerCheater(IPlayerConstructorParams prm);
        IPlayerThoroughCheater CreatePlayerThoroughCheater(IPlayerConstructorParams prm);
    }

}
