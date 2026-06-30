using CandidateManagement.Api.DTOs.Skills;
using CandidateManagement.Api.Models;
using CandidateManagement.Api.Repositories.Interfaces;
using CandidateManagement.Api.Services;
using CandidateManagement.Api.Services.Interfaces;
using Moq;

namespace CandidateManagement.Tests.Services
{
    public class SkillServiceTests
    {
        private readonly Mock<ISkillRepository> _skillRepositoryMock;
        private readonly Mock<IActivityLogService> _activityLogServiceMock;
        private readonly Mock<IOutboxService> _outboxServiceMock;
        private readonly Mock<ITransactionManager> _transactionManagerMock;
        private readonly SkillService _skillService;

        public SkillServiceTests()
        {
            _skillRepositoryMock = new Mock<ISkillRepository>();
            _activityLogServiceMock = new Mock<IActivityLogService>();
            _outboxServiceMock = new Mock<IOutboxService>();
            _transactionManagerMock = new Mock<ITransactionManager>();

            _transactionManagerMock
                .Setup(manager => manager.ExecuteAsync(It.IsAny<Func<Task<SkillResponseDto>>>()))
                .Returns<Func<Task<SkillResponseDto>>>(action => action());

            _transactionManagerMock
                .Setup(manager => manager.ExecuteAsync(It.IsAny<Func<Task<bool>>>()))
                .Returns<Func<Task<bool>>>(action => action());

            _skillService = new SkillService(
                _skillRepositoryMock.Object,
                _activityLogServiceMock.Object,
                _outboxServiceMock.Object,
                _transactionManagerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenSkillIsNew_CreatesSkill()
        {
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

            var result = await _skillService.CreateAsync(dto);

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
            var dto = new CreateSkillDto
            {
                SkillName = "English language"
            };

            _skillRepositoryMock
                .Setup(repo => repo.SkillNameExistsAsync("English language"))
                .ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _skillService.CreateAsync(dto));

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
            _skillRepositoryMock
                .Setup(repo => repo.GetByIdAsync(99))
                .ReturnsAsync((Skill?)null);

            var result = await _skillService.DeleteAsync(99);

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

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _skillService.DeleteAsync(1));

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

            var result = await _skillService.DeleteAsync(1);

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
