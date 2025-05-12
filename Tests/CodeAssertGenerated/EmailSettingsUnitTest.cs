using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Developer_Toolbox.Models;
using Xunit;

namespace CodeAssertGenerated
{
public class EmailSettingsUnitTest
{
    [Fact]
    public void Constructor_Test()
    {
        // Arrange & Act
        var emailsettings = new EmailSettings();
        // Assert
        Assert.NotNull(emailsettings);
        Assert.IsType<EmailSettings>(emailsettings);
    }

    [Fact]
    public void Properties_InitializedCorrectly()
    {
        // Arrange
        var emailsettings = new EmailSettings()
        {
            SmtpServer = "Test String",
            SmtpPort = 42,
            GmailAddress = "Test String",
            GmailAppPassword = "Test String",
            SenderEmail = "Test String",
            SenderName = "Test String",
        };

        // Act & Assert
        Assert.Equal("Test String", emailsettings.SmtpServer);
        Assert.Equal(42, emailsettings.SmtpPort);
        Assert.Equal("Test String", emailsettings.GmailAddress);
        Assert.Equal("Test String", emailsettings.GmailAppPassword);
        Assert.Equal("Test String", emailsettings.SenderEmail);
        Assert.Equal("Test String", emailsettings.SenderName);
    }
}
}
