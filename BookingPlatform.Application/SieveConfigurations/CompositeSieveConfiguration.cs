using Sieve.Services;

namespace BookingPlatform.Application.SieveConfigurations;

public class CompositeSieveConfiguration : ISieveConfiguration
{
    private readonly IEnumerable<ISieveCustomConfiguration> _customConfigs;

    public CompositeSieveConfiguration(IEnumerable<ISieveCustomConfiguration> customConfigs)
    {
        _customConfigs = customConfigs;
    }

    public void Configure(SievePropertyMapper mapper)
    {
        foreach (var config in _customConfigs)
        {
            config.Apply(mapper);
        }
    }
}
