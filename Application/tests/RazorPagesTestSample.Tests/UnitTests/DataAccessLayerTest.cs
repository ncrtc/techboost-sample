using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using RazorPagesTestSample.Data;

namespace RazorPagesTestSample.Tests.UnitTests
{
    public class DataAccessLayerTest
    {
        [Fact]
        public async Task GetMessagesAsync_MessagesAreReturned()
        {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                var expectedMessages = AppDbContext.GetSeedingMessages();
                await db.AddRangeAsync(expectedMessages);
                await db.SaveChangesAsync();

                // Act
                var result = await db.GetMessagesAsync();

                // Assert
                var actualMessages = Assert.IsAssignableFrom<List<Message>>(result);
                Assert.Equal(
                    expectedMessages.OrderBy(m => m.Id).Select(m => m.Text), 
                    actualMessages.OrderBy(m => m.Id).Select(m => m.Text));
            }
        }

        [Fact]
        public async Task AddMessageAsync_MessageIsAdded()
        {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                var recId = 10;
                var expectedMessage = new Message() { Id = recId, Text = "Message" };

                // Act
                await db.AddMessageAsync(expectedMessage);

                // Assert
                var actualMessage = await db.FindAsync<Message>(recId);
                Assert.Equal(expectedMessage, actualMessage);
            }
        }

        [Fact]
        public async Task DeleteAllMessagesAsync_MessagesAreDeleted()
        {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                var seedMessages = AppDbContext.GetSeedingMessages();
                await db.AddRangeAsync(seedMessages);
                await db.SaveChangesAsync();

                // Act
                await db.DeleteAllMessagesAsync();

                // Assert
                Assert.Empty(await db.Messages.AsNoTracking().ToListAsync());
            }
        }

        [Fact]
        public async Task DeleteMessageAsync_MessageIsDeleted_WhenMessageIsFound()
        {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                #region snippet1
                // Arrange
                var seedMessages = AppDbContext.GetSeedingMessages();
                await db.AddRangeAsync(seedMessages);
                await db.SaveChangesAsync();
                var recId = 1;
                var expectedMessages = 
                    seedMessages.Where(message => message.Id != recId).ToList();
                #endregion

                #region snippet2
                // Act
                await db.DeleteMessageAsync(recId);
                #endregion

                #region snippet3
                // Assert
                var actualMessages = await db.Messages.AsNoTracking().ToListAsync();
                Assert.Equal(
                    expectedMessages.OrderBy(m => m.Id).Select(m => m.Text), 
                    actualMessages.OrderBy(m => m.Id).Select(m => m.Text));
                #endregion
            }
        }

        #region snippet4
        [Fact]
        public async Task DeleteMessageAsync_NoMessageIsDeleted_WhenMessageIsNotFound()
        {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                var expectedMessages = AppDbContext.GetSeedingMessages();
                await db.AddRangeAsync(expectedMessages);
                await db.SaveChangesAsync();
                var recId = 4;

                // Act
                try
                {
                    await db.DeleteMessageAsync(recId);
                }
                catch
                {
                    // recId doesn't exist
                }

                // Assert
                var actualMessages = await db.Messages.AsNoTracking().ToListAsync();
                Assert.Equal(
                    expectedMessages.OrderBy(m => m.Id).Select(m => m.Text), 
                    actualMessages.OrderBy(m => m.Id).Select(m => m.Text));
            }
        }
        #endregion
    }

    public class MessageTest
    {
        [Fact]
        public void IdProperty_SetsAndGetsCorrectly()
        {
            // Arrange
            var message = new Message();
            var id = 1;

            // Act
            message.Id = id;

            // Assert
            Assert.Equal(id, message.Id);
        }

        [Fact]
        public void TextProperty_SetsAndGetsCorrectly()
        {
            // Arrange
            var message = new Message();
            var text = "Test message";

            // Act
            message.Text = text;

            // Assert
            Assert.Equal(text, message.Text);
        }

        [Fact]
        public void TextProperty_RequiredAttribute_ThrowsException_WhenNull()
        {
            // Arrange
            var message = new Message();

            // Act
            var validationContext = new ValidationContext(message);
            var result = Validator.TryValidateObject(message, validationContext, null, true);

            // Assert
            Assert.False(result, "Expected validation to fail when Text is null");
        }

        [Fact]
        public void TextProperty_StringLengthAttribute_ThrowsException_WhenTooLong()
        {
            // Arrange
            var message = new Message();
            var longText = new string('a', 251);

            // Act
            message.Text = longText;
            var validationContext = new ValidationContext(message);
            var result = Validator.TryValidateObject(message, validationContext, null, true);

            // Assert
            Assert.False(result, "Expected validation to fail when Text is too long");
        }

        [Fact]
        public void TextProperty_StringLengthAttribute_199()
        {
            // Arrange
            var message = new Message();
            var longText = new string('a', 199);

            // Act
            message.Text = longText;
            var validationContext = new ValidationContext(message);
            var result = Validator.TryValidateObject(message, validationContext, null, true);

            // Assert
            Assert.True(result, "Expected validation to pass when Text is 199 characters long");

        }
        
        [Fact]
        public void TextProperty_StringLengthAttribute_200()
        {
            // Arrange
            var message = new Message();
            var longText = new string('a', 200);

            // Act
            message.Text = longText;
            var validationContext = new ValidationContext(message);
            var result = Validator.TryValidateObject(message, validationContext, null, true);

            // Assert
            Assert.True(result, "Expected validation to pass when Text is 200 characters long");
        }
        
        [Fact]
        public void TextProperty_StringLengthAttribute_201()
        {
            // Arrange
            var message = new Message();
            var longText = new string('a', 201);

            // Act
            message.Text = longText;
            var validationContext = new ValidationContext(message);
            var result = Validator.TryValidateObject(message, validationContext, null, true);

            // Assert
            Assert.True(result, "Expected validation to pass when Text is 201 characters long");
        }

        [Fact]
        public void TextProperty_StringLengthAttribute_249()
        {
            // Arrange
            var message = new Message();
            var longText = new string('a', 249);

            // Act
            message.Text = longText;
            var validationContext = new ValidationContext(message);
            var result = Validator.TryValidateObject(message, validationContext, null, true);

            // Assert
            Assert.True(result, "Expected validation to pass when Text is 249 characters long");
        }

        [Fact]
        public void TextProperty_StringLengthAttribute_250()
        {
            // Arrange
            var message = new Message();
            var longText = new string('a', 250);

            // Act
            message.Text = longText;
            var validationContext = new ValidationContext(message);
            var result = Validator.TryValidateObject(message, validationContext, null, true);

            // Assert
            Assert.True(result, "Expected validation to pass when Text is 250 characters long");
        }
    }
}
