using Explorer.BuildingBlocks.Tests;

namespace Explorer.Tours.Tests;

public class BaseToursIntegrationTest : BaseWebIntegrationTest<ToursTestFactory>
{
    protected BaseToursIntegrationTest(ToursTestFactory factory) : base(factory)
    {
    }
}
