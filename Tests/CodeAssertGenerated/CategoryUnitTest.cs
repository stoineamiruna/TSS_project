using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Developer_Toolbox.Models;
using Xunit;

namespace CodeAssertGenerated
{
public class CategoryUnitTest
{
    [Fact]
    public void Constructor_Test()
    {
        // Arrange & Act
        var category = new Category();
        // Assert
        Assert.NotNull(category);
        Assert.IsType<Category>(category);
    }

    [Fact]
    public void Properties_InitializedCorrectly()
    {
        // Arrange
        var category = new Category()
        {
            CategoryName = "Test String",
            UserId = "Test String",
            Logo = "Test String",
        };

        // Act & Assert
        Assert.Equal("Test String", category.CategoryName);
        Assert.Equal("Test String", category.UserId);
        Assert.Equal("Test String", category.Logo);
    }

    [Fact]
    public void CategoryName_ValidationAttributes()
    {
        // Verificam ca validarea Required se aplica
        var requiredAttribute = typeof(Category)
            .GetProperty("CategoryName")
            .GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;
        Assert.NotNull(requiredAttribute);
    }

    [Fact]
    public void Logo_ValidationAttributes()
    {
        // Verificam ca validarea Required se aplica
        var requiredAttribute = typeof(Category)
            .GetProperty("Logo")
            .GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;
        Assert.NotNull(requiredAttribute);
    }
}
}
