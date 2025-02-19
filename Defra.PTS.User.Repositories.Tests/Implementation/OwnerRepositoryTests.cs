using Defra.PTS.User.Entities;
using Defra.PTS.User.Repositories;
using Defra.PTS.User.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace Defra.PTS.User.Repositories.Tests.Implementation
{
    public class OwnerRepositoryTests
    {
        private readonly DbContextOptions<UserDbContext> _options;
        private readonly UserDbContext _context;
        private readonly OwnerRepository _repository;

        public OwnerRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new UserDbContext(_options);
            _repository = new OwnerRepository(_context);
        }

        [Fact]
        public async Task GetOwnerByEmail_ReturnsOwner_WhenOwnerExists()
        {
            // Arrange
            var owner = new Owner { Email = "owner@example.com" };
            _context.Owner.Add(owner);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetOwnerByEmail("owner@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("owner@example.com", result.Email);
        }

        [Fact]
        public async Task DoesOwnerExists_ReturnsTrue_WhenOwnerExists()
        {
            // Arrange
            var owner = new Owner { Email = "owner@example.com" };
            _context.Owner.Add(owner);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DoesOwnerExists("owner@example.com");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetOwnerByEmail_ReturnsNull_WhenOwnerDoesNotExist()
        {
            // Act
            var result = await _repository.GetOwnerByEmail("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DoesOwnerExists_ReturnsFalse_WhenOwnerDoesNotExist()
        {
            // Act
            var result = await _repository.DoesOwnerExists("nonexistent@example.com");

            // Assert
            Assert.False(result);
        }
    }
}