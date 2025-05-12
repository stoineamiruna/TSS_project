using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Developer_Toolbox.Models;
using Xunit;

namespace CodeAssertGenerated
{
public class LockedSolutionUnitTest
{
    [Fact]
    public void Constructor_Test()
    {
        // Arrange & Act
        var lockedsolution = new LockedSolution();
        // Assert
        Assert.NotNull(lockedsolution);
        Assert.IsType<LockedSolution>(lockedsolution);
    }

    [Fact]
    public void Properties_InitializedCorrectly()
    {
        // Arrange
        var lockedsolution = new LockedSolution()
        {
            SolutionCode = "Test String",
            Score = 42,
            LockedExerciseId = 42,
            UserId = "Test String",
            CreatedAt = new DateTime(2023, 1, 1),
        };

        // Act & Assert
        Assert.Equal("Test String", lockedsolution.SolutionCode);
        Assert.Equal(42, lockedsolution.Score);
        Assert.Equal(42, lockedsolution.LockedExerciseId);
        Assert.Equal("Test String", lockedsolution.UserId);
        Assert.Equal(new DateTime(2023, 1, 1), lockedsolution.CreatedAt);
    }
}
}
