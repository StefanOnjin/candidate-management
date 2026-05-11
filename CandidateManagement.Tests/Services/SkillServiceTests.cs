using CandidateManagement.Api.DTOs.Skills;
using CandidateManagement.Api.Models;
using CandidateManagement.Api.Repositories.Interfaces;
using CandidateManagement.Api.Services;
using Moq;

namespace CandidateManagement.Tests.Services
{
    public class SkillServiceTests
    {
        private readonly Mock<ISkillRepository> _skillRepositoryMock;
        private readonly SkillService _skillService;

        public SkillServiceTests()
        {
            _skillRepositoryMock = new Mock<ISkillRepository>();
            _skillService = new SkillService(_skillRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenSkillIsNew_CreatesSkill()
        {
            // Arrange
            var dto = new CreateSkillDto
            {
                SkillName = "C# programming"
            };

            _skillRepositoryMock
                .Setup(repo => repo.SkillNameExistsAsync("C# programming"))
                .ReturnsAsync(false);

            _skillRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Skill>()))
                .Returns(Task.CompletedTask);

            _skillRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _skillService.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("C# programming", result.SkillName);

            _skillRepositoryMock.Verify(
                repo => repo.AddAsync(It.IsAny<Skill>()),
                Times.Once);

            _skillRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenSkillExists_ThrowsError()
        {
            // Arrange
            var dto = new CreateSkillDto
            {
                SkillName = "English language"
            };

            _skillRepositoryMock
                .Setup(repo => repo.SkillNameExistsAsync("English language"))
                .ReturnsAsync(true);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _skillService.CreateAsync(dto));

            // Assert
            Assert.Equal("Skill with this name already exists.", exception.Message);

            _skillRepositoryMock.Verify(
                repo => repo.AddAsync(It.IsAny<Skill>()),
                Times.Never);

            _skillRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenSkillMissing_ReturnsFalse()
        {
            // Arrange
            _skillRepositoryMock
                .Setup(repo => repo.GetByIdAsync(99))
                .ReturnsAsync((Skill?)null);

            // Act
            var result = await _skillService.DeleteAsync(99);

            // Assert
            Assert.False(result);

            _skillRepositoryMock.Verify(
                repo => repo.Delete(It.IsAny<Skill>()),
                Times.Never);

            _skillRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenSkillIsAssigned_ThrowsError()
        {
            // Arrange
            var skill = new Skill
            {
                Id = 1,
                SkillName = "C# programming"
            };

            _skillRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(skill);

            _skillRepositoryMock
                .Setup(repo => repo.IsSkillAssignedToAnyCandidateAsync(1))
                .ReturnsAsync(true);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _skillService.DeleteAsync(1));

            // Assert
            Assert.Equal(
                "Skill cannot be deleted because it is asigned to one or more candidates.",
                exception.Message);

            _skillRepositoryMock.Verify(
                repo => repo.Delete(It.IsAny<Skill>()),
                Times.Never);

            _skillRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenSkillIsFree_DeletesIt()
        {
            // Arrange
            var skill = new Skill
            {
                Id = 1,
                SkillName = "German language"
            };

            _skillRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(skill);

            _skillRepositoryMock
                .Setup(repo => repo.IsSkillAssignedToAnyCandidateAsync(1))
                .ReturnsAsync(false);

            _skillRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _skillService.DeleteAsync(1);

            // Assert
            Assert.True(result);

            _skillRepositoryMock.Verify(
                repo => repo.Delete(skill),
                Times.Once);

            _skillRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Once);
        }
    }
}
