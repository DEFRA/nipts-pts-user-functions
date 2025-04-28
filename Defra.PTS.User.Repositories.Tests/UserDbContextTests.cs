using Defra.PTS.User.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Defra.PTS.User.Repositories.Tests
{
    public class UserDbContextTests
    {
        [Fact]
        public void CanCreateUserDbContext()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Act
            using (var context = new UserDbContext(options))
            {
                // Assert
                Assert.NotNull(context);
                Assert.NotNull(context.User);
                Assert.NotNull(context.Owner);
                Assert.NotNull(context.Address);
            }
        }
    }
}