using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Developer_Toolbox.Models;
using Xunit;

namespace CodeAssertGenerated
{
public class TagUnitTest
{
    [Fact]
    public void Constructor_Test()
    {
        // Arrange & Act
        var tag = new Tag();
        // Assert
        Assert.NotNull(tag);
        Assert.IsType<Tag>(tag);
    }

    [Fact]
    public void Properties_InitializedCorrectly()
    {
        // Arrange
        var tag = new Tag()
        {
            Name = "Test String",
        };

        // Act & Assert
        Assert.Equal("Test String", tag.Name);
    }

    [Fact]
    public void Name_ValidationAttributes()
    {
        // Verificam ca validarea Required se aplica
        var requiredAttribute = typeof(Tag)
            .GetProperty("Name")
            .GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;
        Assert.NotNull(requiredAttribute);
    }
}
}
