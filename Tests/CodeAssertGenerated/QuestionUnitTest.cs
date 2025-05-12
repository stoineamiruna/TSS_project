using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Developer_Toolbox.Models;
using Xunit;

namespace CodeAssertGenerated
{
public class QuestionUnitTest
{
    [Fact]
    public void Constructor_Test()
    {
        // Arrange & Act
        var question = new Question();
        // Assert
        Assert.NotNull(question);
        Assert.IsType<Question>(question);
    }

    [Fact]
    public void Properties_InitializedCorrectly()
    {
        // Arrange
        var question = new Question()
        {
            UserId = "Test String",
            Title = "Test String",
            Description = "Test String",
            CreatedDate = new DateTime(2023, 1, 1),
            LikesNr = 42,
            DislikesNr = 42,
        };

        // Act & Assert
        Assert.Equal("Test String", question.UserId);
        Assert.Equal("Test String", question.Title);
        Assert.Equal("Test String", question.Description);
        Assert.Equal(new DateTime(2023, 1, 1), question.CreatedDate);
        Assert.Equal(42, question.LikesNr);
        Assert.Equal(42, question.DislikesNr);
    }

    [Fact]
    public void Title_ValidationAttributes()
    {
        // Verificam ca validarea Required se aplica
        var requiredAttribute = typeof(Question)
            .GetProperty("Title")
            .GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;
        Assert.NotNull(requiredAttribute);
        // Verificam ca validarea StringLength se aplica
        var stringLengthAttribute = typeof(Question)
            .GetProperty("Title")
            .GetCustomAttributes(typeof(StringLengthAttribute), false)
            .FirstOrDefault() as StringLengthAttribute;
        Assert.NotNull(stringLengthAttribute);
        Assert.True(stringLengthAttribute.MaximumLength > 0);
    }

    [Fact]
    public void Description_ValidationAttributes()
    {
        // Verificam ca validarea Required se aplica
        var requiredAttribute = typeof(Question)
            .GetProperty("Description")
            .GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;
        Assert.NotNull(requiredAttribute);
    }
}
}
