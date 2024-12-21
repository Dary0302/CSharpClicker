using CSharpClicker.Web.Domain;
using CSharpClicker.Web.DomainServices;
using FluentAssertions;

namespace CSharpClicker.Web.Tests.DomainServices.Tests;

[TestFixture]
public class BoostsProfitCalculationExtensionsTests
{
    [Test]
    public void GetProfit_ShouldCalculateProfitForAutoBoosts()
    {
        var userBoosts = new List<UserBoost>
        {
            new UserBoost { Boost = new Boost { IsAuto = true, Profit = 10 }, Quantity = 2 },
            new UserBoost { Boost = new Boost { IsAuto = true, Profit = 5 }, Quantity = 3 },
            new UserBoost { Boost = new Boost { IsAuto = false, Profit = 20 }, Quantity = 1 }
        };

        var profit = userBoosts.GetProfit(true);

        profit.Should().Be(35);
    }

    [Test]
    public void GetProfit_ShouldCalculateProfitForNonAutoBoosts()
    {
        var userBoosts = new List<UserBoost>
        {
            new UserBoost { Boost = new Boost { IsAuto = true, Profit = 10 }, Quantity = 2 },
            new UserBoost { Boost = new Boost { IsAuto = false, Profit = 5 }, Quantity = 3 },
            new UserBoost { Boost = new Boost { IsAuto = false, Profit = 20 }, Quantity = 1 }
        };

        var profit = userBoosts.GetProfit();

        profit.Should().Be(36);
    }
}