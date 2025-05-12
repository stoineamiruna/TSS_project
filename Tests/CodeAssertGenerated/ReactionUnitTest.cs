using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Developer_Toolbox.Models;
using Xunit;

namespace CodeAssertGenerated
{
public class ReactionUnitTest
{
    [Fact]
    public void Constructor_Test()
    {
        // Arrange & Act
        var reaction = new Reaction();
        // Assert
        Assert.NotNull(reaction);
        Assert.IsType<Reaction>(reaction);
    }

    [Fact]
    public void Properties_InitializedCorrectly()
    {
        // Arrange
        var reaction = new Reaction()
        {
            UserId = "Test String",
            QuestionId = 42,
            Liked = true,
            Disliked = true,
        };

        // Act & Assert
        Assert.Equal("Test String", reaction.UserId);
        Assert.Equal(42, reaction.QuestionId);
        Assert.Equal(true, reaction.Liked);
        Assert.Equal(true, reaction.Disliked);
    }
}
}
