using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Developer_Toolbox.Models;
using Xunit;

namespace CodeAssertGenerated
{
public class NotificationUnitTest
{
    [Fact]
    public void Constructor_Test()
    {
        // Arrange & Act
        var notification = new Notification();
        // Assert
        Assert.NotNull(notification);
        Assert.IsType<Notification>(notification);
    }

    [Fact]
    public void Properties_InitializedCorrectly()
    {
        // Arrange
        var notification = new Notification()
        {
            UserId = "Test String",
            Message = "Test String",
            Link = "Test String",
            IsRead = true,
            CreatedAt = new DateTime(2023, 1, 1),
        };

        // Act & Assert
        Assert.Equal("Test String", notification.UserId);
        Assert.Equal("Test String", notification.Message);
        Assert.Equal("Test String", notification.Link);
        Assert.Equal(true, notification.IsRead);
        Assert.Equal(new DateTime(2023, 1, 1), notification.CreatedAt);
    }

    [Fact]
    public void Message_ValidationAttributes()
    {
        // Verificam ca validarea Required se aplica
        var requiredAttribute = typeof(Notification)
            .GetProperty("Message")
            .GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;
        Assert.NotNull(requiredAttribute);
        // Verificam ca validarea MaxLength se aplica
        var maxLengthAttribute = typeof(Notification)
            .GetProperty("Message")
            .GetCustomAttributes(typeof(MaxLengthAttribute), false)
            .FirstOrDefault() as MaxLengthAttribute;
        Assert.NotNull(maxLengthAttribute);
        Assert.True(maxLengthAttribute.Length > 0);
    }

    [Fact]
    public void Link_ValidationAttributes()
    {
        // Verificam ca validarea MaxLength se aplica
        var maxLengthAttribute = typeof(Notification)
            .GetProperty("Link")
            .GetCustomAttributes(typeof(MaxLengthAttribute), false)
            .FirstOrDefault() as MaxLengthAttribute;
        Assert.NotNull(maxLengthAttribute);
        Assert.True(maxLengthAttribute.Length > 0);
    }
}
}
