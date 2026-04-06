using Xunit;

namespace Graphify.Integration.Tests;

public class SampleIntegrationTests
{
    [Fact]
    public void SampleIntegrationTest_ShouldPass()
    {
        // Arrange
        var expected = true;

        // Act
        var actual = true;

        // Assert
        Assert.Equal(expected, actual);
    }
}
