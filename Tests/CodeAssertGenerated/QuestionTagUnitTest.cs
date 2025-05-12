using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Developer_Toolbox.Models;
using Xunit;

namespace CodeAssertGenerated
{
public class QuestionTagUnitTest
{
    [Fact]
    public void Constructor_Test()
    {
        // Arrange & Act
        var questiontag = new QuestionTag();
        // Assert
        Assert.NotNull(questiontag);
        Assert.IsType<QuestionTag>(questiontag);
    }

    [Fact]
    public void Properties_InitializedCorrectly()
    {
        // Arrange
        var questiontag = new QuestionTag()
        {
            QuestionId = 42,
            TagId = 42,
        };

        // Act & Assert
        Assert.Equal(42, questiontag.QuestionId);
        Assert.Equal(42, questiontag.TagId);
    }
}
}
