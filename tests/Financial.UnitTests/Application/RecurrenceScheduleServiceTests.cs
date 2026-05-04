using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using Financial.Domain.Errors;
using Financial.Domain.Repositories;
using Financial.Infrastructure.Persistence;
using Financial.Infrastructure.Services;
using FinancialControl.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Financial.UnitTests.Application
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class RecurrenceScheduleServiceTests : IDisposable
    {
        private readonly Mock<IRecurrenceScheduleRepository> _scheduleRepoMock;
        private readonly Mock<ITransactionRepository> _transactionRepoMock;
        private readonly ApplicationDbContext _dbContext;
        private readonly RecurrenceScheduleService _service;

        public RecurrenceScheduleServiceTests()
        {
            _scheduleRepoMock = new Mock<IRecurrenceScheduleRepository>();
            _transactionRepoMock = new Mock<ITransactionRepository>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);

            _service = new RecurrenceScheduleService(
                _scheduleRepoMock.Object,
                _transactionRepoMock.Object,
                _dbContext,
                NullLogger<RecurrenceScheduleService>.Instance);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidDTO_ShouldReturnSuccess()
        {
            // Arrange
            var dto = new CreateRecurrenceScheduleDTO(1, RecurrenceFrequency.Monthly, DateTime.UtcNow);
            var transaction = new Transaction(
                1, 1, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(100, Currency.BRL), "Test", DateTime.UtcNow, true);

            _transactionRepoMock
                .Setup(x => x.GetById(dto.TransactionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);

            _scheduleRepoMock
                .Setup(x => x.GetByTransactionId(dto.TransactionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RecurrenceSchedule)null!);

            _scheduleRepoMock
                .Setup(x => x.Create(It.IsAny<RecurrenceSchedule>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _scheduleRepoMock.Verify(x => x.Create(It.IsAny<RecurrenceSchedule>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithNullDTO_ShouldReturnFailure()
        {
            // Act
            var result = await _service.CreateAsync(null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task CreateAsync_WithNonExistingTransaction_ShouldReturnFailure()
        {
            // Arrange
            var dto = new CreateRecurrenceScheduleDTO(999, RecurrenceFrequency.Monthly, DateTime.UtcNow);

            _transactionRepoMock
                .Setup(x => x.GetById(dto.TransactionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Transaction?)null);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(RecurrenceScheduleErrors.InvalidTransaction.Description);
        }

        [Fact]
        public async Task CreateAsync_WithNonRecurringTransaction_ShouldReturnFailure()
        {
            // Arrange
            var dto = new CreateRecurrenceScheduleDTO(1, RecurrenceFrequency.Monthly, DateTime.UtcNow);
            var transaction = new Transaction(
                1, 1, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(100, Currency.BRL), "Test", DateTime.UtcNow, false);

            _transactionRepoMock
                .Setup(x => x.GetById(dto.TransactionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(RecurrenceScheduleErrors.TransactionNotRecurring.Description);
        }

        [Fact]
        public async Task CreateAsync_WithExistingSchedule_ShouldReturnFailure()
        {
            // Arrange
            var dto = new CreateRecurrenceScheduleDTO(1, RecurrenceFrequency.Monthly, DateTime.UtcNow);
            var transaction = new Transaction(
                1, 1, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(100, Currency.BRL), "Test", DateTime.UtcNow, true);
            var existingSchedule = new RecurrenceSchedule(1, RecurrenceFrequency.Monthly, DateTime.UtcNow);

            _transactionRepoMock
                .Setup(x => x.GetById(dto.TransactionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);

            _scheduleRepoMock
                .Setup(x => x.GetByTransactionId(dto.TransactionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingSchedule);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(RecurrenceScheduleErrors.ScheduleAlreadyExists.Description);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetByIdAsync(0);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(RecurrenceScheduleErrors.InvalidId.Description);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            _scheduleRepoMock
                .Setup(x => x.GetById(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RecurrenceSchedule?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithNullDTO_ShouldReturnFailure()
        {
            // Act
            var result = await _service.UpdateAsync(null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Arrange
            var dto = new UpdateRecurrenceScheduleDTO(0, RecurrenceFrequency.Monthly);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(RecurrenceScheduleErrors.InvalidId.Description);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            var dto = new UpdateRecurrenceScheduleDTO(999, RecurrenceFrequency.Monthly);

            _scheduleRepoMock
                .Setup(x => x.GetById(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RecurrenceSchedule?)null);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithValidDTO_ShouldReturnSuccess()
        {
            // Arrange
            var schedule = new RecurrenceSchedule(1, RecurrenceFrequency.Monthly, DateTime.UtcNow);
            var dto = new UpdateRecurrenceScheduleDTO(1, RecurrenceFrequency.Weekly, DateTime.UtcNow.AddYears(1), 12);

            _scheduleRepoMock
                .Setup(x => x.GetById(dto.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(schedule);

            _scheduleRepoMock
                .Setup(x => x.Update(It.IsAny<RecurrenceSchedule>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
            _scheduleRepoMock.Verify(x => x.Update(It.IsAny<RecurrenceSchedule>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.DeleteAsync(0);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(RecurrenceScheduleErrors.InvalidId.Description);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            _scheduleRepoMock
                .Setup(x => x.GetById(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RecurrenceSchedule?)null);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var schedule = new RecurrenceSchedule(1, RecurrenceFrequency.Monthly, DateTime.UtcNow);

            _scheduleRepoMock
                .Setup(x => x.GetById(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(schedule);

            _scheduleRepoMock
                .Setup(x => x.Delete(It.IsAny<RecurrenceSchedule>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _scheduleRepoMock.Verify(x => x.Delete(It.IsAny<RecurrenceSchedule>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DeactiveScheduleAsync Tests

        [Fact]
        public async Task DeactiveScheduleAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var schedule = new RecurrenceSchedule(1, RecurrenceFrequency.Monthly, DateTime.UtcNow);
            schedule.Activate();

            _scheduleRepoMock
                .Setup(x => x.GetById(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(schedule);

            _scheduleRepoMock
                .Setup(x => x.Update(It.IsAny<RecurrenceSchedule>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeactiveScheduleAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _scheduleRepoMock.Verify(x => x.Update(It.IsAny<RecurrenceSchedule>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeactiveScheduleAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            _scheduleRepoMock
                .Setup(x => x.GetById(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RecurrenceSchedule?)null);

            // Act
            var result = await _service.DeactiveScheduleAsync(999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region ActiveScheduleAsync Tests

        [Fact]
        public async Task ActiveScheduleAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var schedule = new RecurrenceSchedule(1, RecurrenceFrequency.Monthly, DateTime.UtcNow);

            _scheduleRepoMock
                .Setup(x => x.GetById(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(schedule);

            _scheduleRepoMock
                .Setup(x => x.Update(It.IsAny<RecurrenceSchedule>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.ActiveScheduleAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ActiveScheduleAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            _scheduleRepoMock
                .Setup(x => x.GetById(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RecurrenceSchedule?)null);

            // Act
            var result = await _service.ActiveScheduleAsync(999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetActiveByUserIdAsync Tests

        [Fact]
        public async Task GetActiveByUserIdAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetActiveByUserIdAsync(0);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(RecurrenceScheduleErrors.InvalidId.Description);
        }

        #endregion
    }
}
