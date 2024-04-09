using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Services.Implementations;
using TMS.Services.Models;

namespace Tests.Services.ServiceTests
{
    public class EmailServiceTests
    {
        private readonly Mock<IOptions<EmailSettings>> _emailSettings;
        private readonly EmailService _emailService;

        public EmailServiceTests()
        {
            _emailSettings = new Mock<IOptions<EmailSettings>>();
            _emailSettings.Setup(x => x.Value).Returns(new EmailSettings
            {
                SmtpServer = "smtp-relay.brevo.com",
                Port = 587,
                Login = "ivan.n.mihaylov@gmail.com",
                SmtpKey = "paf9QWn05mhOVKLZ",
                EnableSsl = true
            });

            _emailService = new EmailService(_emailSettings.Object);
        }

        [Fact]
        public async Task SendEmailAsync_ShouldReturnTrue()
        {
            //Arrange
            var email = "test@gmail.com";
            var subject = "Test Email";
            var message = "This is a test email";

            //Act
            await _emailService.SendEmailAsync(email, subject, message);

            //Assert
            Assert.True(true);
        }

    }
}
