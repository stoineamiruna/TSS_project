using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Developer_Toolbox.Models;
using Xunit;

namespace CodeAssertGenerated
{
public class AnswerUnitTest
{
    [Fact]
    public void Constructor_Test()
    {
        // Arrange & Act
        var answer = new Answer();
        // Assert
        Assert.NotNull(answer);
        Assert.IsType<Answer>(answer);
    }

    [Fact]
    public void Properties_InitializedCorrectly()
    {
        // Arrange
        var answer = new Answer()
        {
            Date = new DateTime(2023, 1, 1),
            LikesNr = 42,
            DislikesNr = 42,
            Content = "Test String",
            UserId = "Test String",
            QuestionId = 42,
        };

        // Act & Assert
        Assert.Equal(new DateTime(2023, 1, 1), answer.Date);
        Assert.Equal(42, answer.LikesNr);
        Assert.Equal(42, answer.DislikesNr);
        Assert.Equal("Test String", answer.Content);
        Assert.Equal("Test String", answer.UserId);
        Assert.Equal(42, answer.QuestionId);
    }

    [Fact]
    public void Content_ValidationAttributes()
    {
        // Verificam ca validarea Required se aplica
        var requiredAttribute = typeof(Answer)
            .GetProperty("Content")
            .GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;
        Assert.NotNull(requiredAttribute);
    }
}
}
