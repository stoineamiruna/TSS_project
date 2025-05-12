using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Developer_Toolbox.Models;
using Xunit;

namespace CodeAssertGenerated
{
public class SolutionUnitTest
{
    [Fact]
    public void Constructor_Test()
    {
        // Arrange & Act
        var solution = new Solution();
        // Assert
        Assert.NotNull(solution);
        Assert.IsType<Solution>(solution);
    }

    [Fact]
    public void Properties_InitializedCorrectly()
    {
        // Arrange
        var solution = new Solution()
        {
            SolutionCode = "Test String",
            Score = 42,
            ExerciseId = 42,
            UserId = "Test String",
            CreatedAt = new DateTime(2023, 1, 1),
        };

        // Act & Assert
        Assert.Equal("Test String", solution.SolutionCode);
        Assert.Equal(42, solution.Score);
        Assert.Equal(42, solution.ExerciseId);
        Assert.Equal("Test String", solution.UserId);
        Assert.Equal(new DateTime(2023, 1, 1), solution.CreatedAt);
    }
}
}
