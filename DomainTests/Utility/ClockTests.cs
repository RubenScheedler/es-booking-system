using Domain.Utility;
using Xunit;

namespace DomainTests.Utility;

public class ClockTests
{
    [Fact]
    public void Now_ReturnsCurrentDateTime()
    {
        // Arrange
        var clock = new Clock();

        // Act
        var result = clock.Now();

        // Assert
        // Using TimeSpan.FromSeconds(1) to allow for a small difference in milliseconds
        Assert.Equal(DateTime.Now, result, TimeSpan.FromSeconds(1)); 
    }
}