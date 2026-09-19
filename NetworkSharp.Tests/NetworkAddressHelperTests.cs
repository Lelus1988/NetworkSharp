using NetworkSharp.Services;
using Xunit;

public class NetworkAddressHelperTests
{
    [Fact]
    public void GetCandidateAddresses_Should_Return_Expected_Range_For_24_Subnet()
    {
        var addresses = NetworkAddressHelper.GetCandidateAddresses("192.168.178.0/24").Take(3).ToList();

        Assert.Equal("192.168.178.1", addresses[0]);
        Assert.Equal("192.168.178.2", addresses[1]);
        Assert.Equal("192.168.178.3", addresses[2]);
    }

    [Fact]
    public void GetCandidateAddresses_Should_Return_Empty_For_Invalid_Subnet()
    {
        var addresses = NetworkAddressHelper.GetCandidateAddresses("invalid");

        Assert.Empty(addresses);
    }

    [Fact]
    public void GetBestServer_Should_Ignore_Unreachable_Servers()
    {
        var servers = new List<SpeedTestServer>
        {
            new() { Id = "offline", Name = "Offline", Host = "offline.example", Ping = -1 },
            new() { Id = "slow", Name = "Slow", Host = "slow.example", Ping = 50 },
            new() { Id = "best", Name = "Best", Host = "best.example", Ping = 12 }
        };

        var best = SpeedTestService.GetBestServer(servers);

        Assert.NotNull(best);
        Assert.Equal("best", best!.Id);
    }
}
