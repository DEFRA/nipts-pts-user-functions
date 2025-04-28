using Defra.PTS.User.Entities;
using Defra.PTS.User.Repositories;
using Defra.PTS.User.Repositories.Implementation;
using Model = Defra.PTS.User.Models;
using Entity = Defra.PTS.User.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace Defra.PTS.User.Repositories.Tests.Implementation
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<UserDbContext> _options;
        private readonly UserDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new UserDbContext(_options);
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task DoesUserExists_ReturnsTrue_WhenUserExists()
        {
            // Arrange
            var user = new Entity.User { Email = "test@example.com" };
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DoesUserExists("test@example.com");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetUser_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var user = new Entity.User { Email = "test2@example.com" };
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUser("test2@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test2@example.com", result.Email);
        }

        [Fact]
        public async Task GetUserDetail_ReturnsUserDetail_WhenUserExists()
        {
            // Arrange
            var address = new Address { Id = Guid.NewGuid(), AddressLineOne = "123 Main St" };
            var contactId = Guid.NewGuid();
            var user = new Entity.User { ContactId = contactId, AddressId = address.Id, FullName = "John Doe", Email = "john@example.com", Telephone = "1234567890" };
            _context.Address.Add(address);
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserDetail(contactId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.FullName);
            Assert.Equal("john@example.com", result.Email);
            Assert.Equal("123 Main St", result.AddressLineOne);
        }
    }
}