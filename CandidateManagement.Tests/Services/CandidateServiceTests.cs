using CandidateManagement.Api.DTOs.Candidates;
using CandidateManagement.Api.Models;
using CandidateManagement.Api.Repositories.Interfaces;
using CandidateManagement.Api.Services;
using Moq;

namespace CandidateManagement.Tests.Services
{
    public class CandidateServiceTests
    {
        private readonly Mock<ICandidateRepository> _candidateRepositoryMock;
        private readonly Mock<ISkillRepository> _skillRepositoryMock;
        private readonly CandidateService _candidateService;

        public CandidateServiceTests()
        {
            _candidateRepositoryMock = new Mock<ICandidateRepository>();
            _skillRepositoryMock = new Mock<ISkillRepository>();

            _candidateService = new CandidateService(
                _candidateRepositoryMock.Object,
                _skillRepositoryMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesCandidate()
        {
            // Arrange
            var dto = new CreateCandidateDto
            {
                FullName = "Stefan Onjin",
                DateOfBirth = new DateOnly(1998, 4, 12),
                ContactNumber = "+381641112233",
                EmailAddress = "stefan.onjin@example.com",
                SkillIds = new List<int> { 1, 2 }
            };

            var skills = new List<Skill>
            {
                new Skill { Id = 1, SkillName = "C# programming" },
                new Skill { Id = 2, SkillName = "English language" }
            };

            _candidateRepositoryMock
                .Setup(repo => repo.EmailExistsAsync(dto.EmailAddress))
                .ReturnsAsync(false);

            _skillRepositoryMock
                .Setup(repo => repo.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(skills);

            _candidateRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Candidate>()))
                .Callback<Candidate>(candidate =>
                {
                    candidate.Id = 1;

                    foreach (var candidateSkill in candidate.CandidateSkills)
                    {
                        candidateSkill.Skill = skills.First(s => s.Id == candidateSkill.SkillId);
                    }
                })
                .Returns(Task.CompletedTask);

            _candidateRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _candidateRepositoryMock
                .Setup(repo => repo.GetByIdWithSkillsAsync(1))
                .ReturnsAsync((int id) =>
                {
                    return new Candidate
                    {
                        Id = 1,
                        FullName = dto.FullName,
                        DateOfBirth = dto.DateOfBirth,
                        ContactNumber = dto.ContactNumber,
                        EmailAddress = dto.EmailAddress,
                        CandidateSkills = skills.Select(skill => new CandidateSkill
                        {
                            CandidateId = 1,
                            SkillId = skill.Id,
                            Skill = skill
                        }).ToList()
                    };
                });

            // Act
            var result = await _candidateService.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Stefan Onjin", result.FullName);
            Assert.Equal("stefan.onjin@example.com", result.EmailAddress);
            Assert.Equal(2, result.Skills.Count);

            _candidateRepositoryMock.Verify(
                repo => repo.AddAsync(It.IsAny<Candidate>()),
                Times.Once);

            _candidateRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithExistingEmail_ThrowsError()
        {
            // Arrange
            var dto = new CreateCandidateDto
            {
                FullName = "Dragan Draganovic",
                DateOfBirth = new DateOnly(1998, 4, 12),
                ContactNumber = "+381641112233",
                EmailAddress = "dragan.draganovic@example.com",
                SkillIds = new List<int> { 1 }
            };

            _candidateRepositoryMock
                .Setup(repo => repo.EmailExistsAsync(dto.EmailAddress))
                .ReturnsAsync(true);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _candidateService.CreateAsync(dto));

            // Assert
            Assert.Equal("Candidate with this email already exists.", exception.Message);

            _candidateRepositoryMock.Verify(
                repo => repo.AddAsync(It.IsAny<Candidate>()),
                Times.Never);

            _candidateRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithMissingSkill_ThrowsError()
        {
            // Arrange
            var dto = new CreateCandidateDto
            {
                FullName = "Stanko Stankovic",
                DateOfBirth = new DateOnly(1996, 9, 25),
                ContactNumber = "+381642223344",
                EmailAddress = "stanko.stankovic@example.com",
                SkillIds = new List<int> { 1, 2, 99 }
            };

            var existingSkills = new List<Skill>
            {
                new Skill { Id = 1, SkillName = "Java programming" },
                new Skill { Id = 2, SkillName = "Database design" }
            };

            _candidateRepositoryMock
                .Setup(repo => repo.EmailExistsAsync(dto.EmailAddress))
                .ReturnsAsync(false);

            _skillRepositoryMock
                .Setup(repo => repo.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(existingSkills);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _candidateService.CreateAsync(dto));

            // Assert
            Assert.Equal("One or more skills do not exist.", exception.Message);

            _candidateRepositoryMock.Verify(
                repo => repo.AddAsync(It.IsAny<Candidate>()),
                Times.Never);

            _candidateRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenCandidateMissing_ReturnsFalse()
        {
            // Arrange
            _candidateRepositoryMock
                .Setup(repo => repo.GetByIdAsync(99))
                .ReturnsAsync((Candidate?)null);

            // Act
            var result = await _candidateService.DeleteAsync(99);

            // Assert
            Assert.False(result);

            _candidateRepositoryMock.Verify(
                repo => repo.Delete(It.IsAny<Candidate>()),
                Times.Never);

            _candidateRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenCandidateExists_DeletesIt()
        {
            // Arrange
            var candidate = new Candidate
            {
                Id = 1,
                FullName = "Dragan Draganovic",
                DateOfBirth = new DateOnly(2000, 1, 18),
                ContactNumber = "+381643334455",
                EmailAddress = "dragan.draganovic@example.com"
            };

            _candidateRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(candidate);

            _candidateRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _candidateService.DeleteAsync(1);

            // Assert
            Assert.True(result);

            _candidateRepositoryMock.Verify(
                repo => repo.Delete(candidate),
                Times.Once);

            _candidateRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Once);
        }
    }
}
