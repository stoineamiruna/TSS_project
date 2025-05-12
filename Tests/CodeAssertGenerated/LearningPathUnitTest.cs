using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Developer_Toolbox.Models;
using Xunit;

namespace CodeAssertGenerated
{
public class LearningPathUnitTest
{
    [Fact]
    public void Constructor_Test()
    {
        // Arrange & Act
        var learningpath = new LearningPath();
        // Assert
        Assert.NotNull(learningpath);
        Assert.IsType<LearningPath>(learningpath);
    }

    [Fact]
    public void Properties_InitializedCorrectly()
    {
        // Arrange
        var learningpath = new LearningPath()
        {
            Name = "Test String",
            UserId = "Test String",
            Description = "Test String",
        };

        // Act & Assert
        Assert.Equal("Test String", learningpath.Name);
        Assert.Equal("Test String", learningpath.UserId);
        Assert.Equal("Test String", learningpath.Description);
    }

    [Fact]
    public void Name_ValidationAttributes()
    {
        // Verificam ca validarea Required se aplica
        var requiredAttribute = typeof(LearningPath)
            .GetProperty("Name")
            .GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;
        Assert.NotNull(requiredAttribute);
    }

    [Fact]
    public void Description_ValidationAttributes()
    {
        // Verificam ca validarea Required se aplica
        var requiredAttribute = typeof(LearningPath)
            .GetProperty("Description")
            .GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;
        Assert.NotNull(requiredAttribute);
    }
}
}
