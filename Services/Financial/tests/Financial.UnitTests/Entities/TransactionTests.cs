using Financial.Domain.Entities;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.UnitTests.Entities
{
    public class TransactionTests
    {
        // Helper method to create a valid Money object for tests
        private Money CreateValidMoney()
        {
            // Assuming Money has a constructor like Money(decimal value, string currencyCode)
            // Or a static factory method. Adjust based on your actual Money implementation.
            return Money.Create(100, Currency.USD);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateTransaction()
        {
            // Arrange
            var userId = 1;
            var categoryId = 10;
            var paymentMethod = PaymentMethod.CreditCard;
            var transactionType = TransactionType.Expense;
            var amount = CreateValidMoney();
            var description = "Groceries";
            var transactionDate = DateTime.Now;
            var isRecurring = true;

            // Act
            var transaction = new Transaction(
                userId,
                categoryId,
                paymentMethod,
                transactionType,
                amount,
                description,
                transactionDate,
                isRecurring);

            // Assert
            Assert.NotNull(transaction);
            Assert.Equal(userId, transaction.UserId);
            Assert.Equal(categoryId, transaction.CategoryId);
            Assert.Equal(paymentMethod, transaction.PaymentMethod);
            Assert.Equal(transactionType, transaction.TransactionType);
            Assert.Equal(amount, transaction.Amount);
            Assert.Equal(description, transaction.Description);
            Assert.Equal(DateTime.SpecifyKind(transactionDate, DateTimeKind.Utc), transaction.TransactionDate);
            Assert.Equal(isRecurring, transaction.IsRecurring);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_WithInvalidUserId_ShouldThrowArgumentOutOfRangeException(int invalidUserId)
        {
            // Arrange
            var categoryId = 10;
            var paymentMethod = PaymentMethod.CreditCard;
            var transactionType = TransactionType.Expense;
            var amount = CreateValidMoney();
            var description = "Groceries";
            var transactionDate = DateTime.Now;

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Transaction(
                invalidUserId,
                categoryId,
                paymentMethod,
                transactionType,
                amount,
                description,
                transactionDate));

            Assert.Equal("userId", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNullAmount_ShouldThrowArgumentNullException()
        {
            // Arrange
            var userId = 1;
            var categoryId = 10;
            var paymentMethod = PaymentMethod.CreditCard;
            var transactionType = TransactionType.Expense;
            Money amount = null; // Null amount
            var description = "Groceries";
            var transactionDate = DateTime.Now;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new Transaction(
                userId,
                categoryId,
                paymentMethod,
                transactionType,
                amount,
                description,
                transactionDate));

            Assert.Equal("amount", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        public void Constructor_WithNullDescription_ShouldThrowArgumentNullException(string invalidDescription)
        {
            // Arrange
            var userId = 1;
            var categoryId = 10;
            var paymentMethod = PaymentMethod.CreditCard;
            var transactionType = TransactionType.Expense;
            var amount = CreateValidMoney();
            var transactionDate = DateTime.Now;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new Transaction(
                userId,
                categoryId,
                paymentMethod,
                transactionType,
                amount,
                invalidDescription,
                transactionDate));

            Assert.Equal("description", exception.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_WithWhiteSpaceDescription_ShouldThrowArgumentException(string invalidDescription)
        {
            // Arrange
            var userId = 1;
            var categoryId = 10;
            var paymentMethod = PaymentMethod.CreditCard;
            var transactionType = TransactionType.Expense;
            var amount = CreateValidMoney();
            var transactionDate = DateTime.Now;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Transaction(
                userId,
                categoryId,
                paymentMethod,
                transactionType,
                amount,
                invalidDescription,
                transactionDate));

            Assert.Equal("description", exception.ParamName);
        }

        [Fact]
        public void Constructor_TransactionDate_ShouldBeUtcKind()
        {
            // Arrange
            var userId = 1;
            var amount = CreateValidMoney();
            var description = "Test";
            var transactionDate = DateTime.Now; // Local time

            // Act
            var transaction = new Transaction(
                userId,
                null,
                PaymentMethod.Cash,
                TransactionType.Income,
                amount,
                description,
                transactionDate);

            // Assert
            Assert.Equal(DateTimeKind.Utc, transaction.TransactionDate.Kind);
            Assert.Equal(DateTime.SpecifyKind(transactionDate, DateTimeKind.Utc), transaction.TransactionDate);
        }

        [Fact]
        public void Constructor_IsRecurring_ShouldDefaultToFalseWhenNotProvided()
        {
            // Arrange
            var userId = 1;
            var amount = CreateValidMoney();
            var description = "Test";
            var transactionDate = DateTime.Now;

            // Act
            var transaction = new Transaction(
                userId,
                null,
                PaymentMethod.Cash,
                TransactionType.Income,
                amount,
                description,
                transactionDate);

            // Assert
            Assert.False(transaction.IsRecurring);
        }

        [Fact]
        public void Constructor_IsRecurring_ShouldBeSetCorrectlyWhenProvided()
        {
            // Arrange
            var userId = 1;
            var amount = CreateValidMoney();
            var description = "Test";
            var transactionDate = DateTime.Now;
            var isRecurring = true;

            // Act
            var transaction = new Transaction(
                userId,
                null,
                PaymentMethod.Cash,
                TransactionType.Income,
                amount,
                description,
                transactionDate,
                isRecurring);

            // Assert
            Assert.True(transaction.IsRecurring);
        }

        [Fact]
        public void Constructor_WithNullCategoryId_ShouldCreateTransaction()
        {
            // Arrange
            var userId = 1;
            int? categoryId = null; // Null category ID
            var paymentMethod = PaymentMethod.CreditCard;
            var transactionType = TransactionType.Expense;
            var amount = CreateValidMoney();
            var description = "Groceries";
            var transactionDate = DateTime.Now;

            // Act
            var transaction = new Transaction(
                userId,
                categoryId,
                paymentMethod,
                transactionType,
                amount,
                description,
                transactionDate);

            // Assert
            Assert.NotNull(transaction);
            Assert.Null(transaction.CategoryId);
        }

        #endregion

        #region Update Method Tests

        [Fact]
        public void Update_WithValidParameters_ShouldUpdateTransaction()
        {
            // Arrange
            var initialTransaction = new Transaction(
                1, 10, PaymentMethod.CreditCard, TransactionType.Expense, CreateValidMoney(), "Old Description", DateTime.Now.AddDays(-1));

            var newCategoryId = 20;
            var newPaymentMethod = PaymentMethod.DebitCard;
            var newTransactionType = TransactionType.Income;
            var newAmount = Money.Create(200, Currency.EUR);
            var newDescription = "New Description";
            var newTransactionDate = DateTime.Now;
            var newIsRecurring = true;

            // Act
            initialTransaction.Update(
                newCategoryId,
                newPaymentMethod,
                newTransactionType,
                newAmount,
                newDescription,
                newTransactionDate,
                newIsRecurring);

            // Assert
            Assert.Equal(newCategoryId, initialTransaction.CategoryId);
            Assert.Equal(newPaymentMethod, initialTransaction.PaymentMethod);
            Assert.Equal(newTransactionType, initialTransaction.TransactionType);
            Assert.Equal(newAmount, initialTransaction.Amount);
            Assert.Equal(newDescription, initialTransaction.Description);
            Assert.Equal(DateTime.SpecifyKind(newTransactionDate, DateTimeKind.Utc), initialTransaction.TransactionDate);
            Assert.Equal(newIsRecurring, initialTransaction.IsRecurring);
        }

        [Fact]
        public void Update_WithNullAmount_ShouldThrowArgumentNullException()
        {
            // Arrange
            var transaction = new Transaction(
                1, 10, PaymentMethod.CreditCard, TransactionType.Expense, CreateValidMoney(), "Description", DateTime.Now);
            Money nullAmount = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => transaction.Update(
                10, PaymentMethod.DebitCard, TransactionType.Income, nullAmount, "New Description", DateTime.Now));

            Assert.Equal("amount", exception.ParamName);
        }

        [Fact]
        public void Update_WithNullDescription_ShouldThrowArgumentNullException()
        {
            // Arrange
            var transaction = new Transaction(
                1,
                10,
                PaymentMethod.CreditCard,
                TransactionType.Expense,
                CreateValidMoney(),
                "Description",
                DateTime.Now);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => transaction.Update(
                10,
                PaymentMethod.DebitCard,
                TransactionType.Income,
                CreateValidMoney(),
                null,
                DateTime.Now));

            Assert.Equal("description", exception.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Update_WithWhiteSpaceDescription_ShouldThrowArgumentException(string invalidDescription)
        {
            // Arrange
            var transaction = new Transaction(
                1,
                10,
                PaymentMethod.CreditCard,
                TransactionType.Expense,
                CreateValidMoney(),
                "Description",
                DateTime.Now);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => transaction.Update(
                10,
                PaymentMethod.DebitCard,
                TransactionType.Income,
                CreateValidMoney(),
                invalidDescription,
                DateTime.Now));

            Assert.Equal("description", exception.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Update_WithInvalidCategoryId_ShouldThrowArgumentOutOfRangeException(int invalidCategoryId)
        {
            // Arrange
            var transaction = new Transaction(
                1,
                10,
                PaymentMethod.CreditCard,
                TransactionType.Expense,
                CreateValidMoney(),
                "Description",
                DateTime.Now);

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => transaction.Update(
                invalidCategoryId,
                PaymentMethod.DebitCard,
                TransactionType.Income,
                CreateValidMoney(),
                "New Description",
                DateTime.Now));

            Assert.Equal("categoryId.Value", exception.ParamName);
        }

        [Fact]
        public void Update_TransactionDate_ShouldBeUtcKind()
        {
            // Arrange
            var transaction = new Transaction(
                1, null, PaymentMethod.Cash, TransactionType.Income, CreateValidMoney(), "Description", DateTime.Now.AddDays(-1));
            var newTransactionDate = DateTime.Now; // Local time

            // Act
            transaction.Update(
                null, PaymentMethod.Cash, TransactionType.Income, CreateValidMoney(), "New Description", newTransactionDate);

            // Assert
            Assert.Equal(DateTimeKind.Utc, transaction.TransactionDate.Kind);
            Assert.Equal(DateTime.SpecifyKind(newTransactionDate, DateTimeKind.Utc), transaction.TransactionDate);
        }

        [Fact]
        public void Update_IsRecurring_ShouldDefaultToFalseWhenNotProvided()
        {
            // Arrange
            var transaction = new Transaction(
                1, null, PaymentMethod.Cash, TransactionType.Income, CreateValidMoney(), "Description", DateTime.Now.AddDays(-1));

            // Act
            transaction.Update(
                null, PaymentMethod.Cash, TransactionType.Income, CreateValidMoney(), "New Description", DateTime.Now);

            // Assert
            Assert.False(transaction.IsRecurring);
        }

        [Fact]
        public void Update_IsRecurring_ShouldBeSetCorrectlyWhenProvided()
        {
            // Arrange
            var transaction = new Transaction(
                1, null, PaymentMethod.Cash, TransactionType.Income, CreateValidMoney(), "Description", DateTime.Now.AddDays(-1));
            var newIsRecurring = true;

            // Act
            transaction.Update(
                null, PaymentMethod.Cash, TransactionType.Income, CreateValidMoney(), "New Description", DateTime.Now, newIsRecurring);

            // Assert
            Assert.True(transaction.IsRecurring);
        }

        [Fact]
        public void Update_WithNullCategoryId_ShouldSetCategoryIdToNull()
        {
            // Arrange
            var transaction = new Transaction(
                1, 10, PaymentMethod.CreditCard, TransactionType.Expense, CreateValidMoney(), "Description", DateTime.Now);
            int? newCategoryId = null;

            // Act
            transaction.Update(
                newCategoryId, PaymentMethod.DebitCard, TransactionType.Income, CreateValidMoney(), "New Description", DateTime.Now);

            // Assert
            Assert.Null(transaction.CategoryId);
        }

        #endregion
    }
}