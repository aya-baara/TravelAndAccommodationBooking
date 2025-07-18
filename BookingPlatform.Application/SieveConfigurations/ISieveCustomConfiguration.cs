using Sieve.Services;

namespace BookingPlatform.Application.SieveConfigurations;

public interface ISieveCustomConfiguration
{
    void Apply(SievePropertyMapper mapper);
}

