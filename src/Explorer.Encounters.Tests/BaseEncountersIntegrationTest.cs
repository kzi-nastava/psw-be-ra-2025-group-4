using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Tests;

namespace Explorer.Encounters.Tests;

public class BaseEncountersIntegrationTest : BaseWebIntegrationTest<EncountersTestFactory>
{
    public BaseEncountersIntegrationTest(EncountersTestFactory factory) : base(factory) { }
}
