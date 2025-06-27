using Financial.Domain.Entities;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.UnitTests.Entities
{
    public class TransactionTests
    {
        private Money CreateValidMoney(int amount = 100, Currency currency = Currency.USD)
            => Money.Create(amount, currency);

        #region Constructor Tests

        [Fact(DisplayName = "Dado parâmetros válidos, Quando criar transação, Então deve criar corretamente")]
        public void Deve_Criar_Transacao_Com_Parametros_Validos()
        {
            // Arrange
            int userId = 1;
            int? categoryId = 10;
            PaymentMethod paymentMethod = PaymentMethod.CreditCard;
            TransactionType transactionType = TransactionType.Expense;
            Money amount = CreateValidMoney();
            string description = "Test Transaction";
            DateTime transactionDate = DateTime.Now;

            // Act
            var transaction = new Transaction(
                userId, categoryId, paymentMethod, transactionType, amount, description, transactionDate, true);

            // Assert
            Assert.Equal(userId, transaction.UserId);
            Assert.Equal(categoryId, transaction.CategoryId);
            Assert.Equal(paymentMethod, transaction.PaymentMethod);
            Assert.Equal(transactionType, transaction.TransactionType);
            Assert.Equal(amount, transaction.Amount);
            Assert.Equal(description, transaction.Description);
            Assert.Equal(DateTime.SpecifyKind(transactionDate, DateTimeKind.Utc), transaction.TransactionDate);
            Assert.True(transaction.IsRecurring);
        }

        [Theory(DisplayName = "Dado userId inválido, Quando criar transação, Então deve lançar ArgumentOutOfRangeException")]
        [InlineData(0)]
        [InlineData(-1)]
        public void Deve_Lancar_ArgumentOutOfRangeException_Para_UserId_Invalido(int invalidUserId)
        {
            // Arrange
            int? categoryId = 10;
            PaymentMethod paymentMethod = PaymentMethod.CreditCard;
            TransactionType transactionType = TransactionType.Expense;
            Money amount = CreateValidMoney();
            string description = "Test Transaction";
            DateTime transactionDate = DateTime.Now;

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Transaction(
                invalidUserId, categoryId, paymentMethod, transactionType, amount, description, transactionDate));
            Assert.Equal("userId", exception.ParamName);
        }

        [Fact(DisplayName = "Dado amount nulo, Quando criar transação, Então deve lançar ArgumentNullException")]
        public void Deve_Lancar_ArgumentNullException_Para_Amount_Nulo()
        {
            // Arrange
            int userId = 1;
            int? categoryId = 10;
            PaymentMethod paymentMethod = PaymentMethod.CreditCard;
            TransactionType transactionType = TransactionType.Expense;
            Money amount = null;
            string description = "Test Transaction";
            DateTime transactionDate = DateTime.Now;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new Transaction(
                userId, categoryId, paymentMethod, transactionType, amount, description, transactionDate));
            Assert.Equal("amount", exception.ParamName);
        }

        [Fact(DisplayName = "Dado description nulo, Quando criar transação, Então deve lançar ArgumentNullException")]
        public void Deve_Lancar_ArgumentNullException_Para_Description_Nulo()
        {
            // Arrange
            int userId = 1;
            int? categoryId = 10;
            PaymentMethod paymentMethod = PaymentMethod.CreditCard;
            TransactionType transactionType = TransactionType.Expense;
            Money amount = CreateValidMoney();
            string? description = null;
            DateTime transactionDate = DateTime.Now;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new Transaction(
                userId, categoryId, paymentMethod, transactionType, amount, description!, transactionDate));
            Assert.Equal("description", exception.ParamName);
        }

        [Fact(DisplayName = "Dado description vazio, Quando criar transação, Então deve lançar ArgumentException")]
        public void Deve_Lancar_ArgumentException_Para_Description_Vazio()
        {
            // Arrange
            int userId = 1;
            int? categoryId = 10;
            PaymentMethod paymentMethod = PaymentMethod.CreditCard;
            TransactionType transactionType = TransactionType.Expense;
            Money amount = CreateValidMoney();
            string description = string.Empty;
            DateTime transactionDate = DateTime.Now;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Transaction(
                userId, categoryId, paymentMethod, transactionType, amount, description, transactionDate));
            Assert.Equal("description", exception.ParamName);
        }

        [Fact(DisplayName = "Dado description com espaço em branco, Quando criar transação, Então deve lançar ArgumentException")]
        public void Deve_Lancar_ArgumentException_Para_Description_Whitespace()
        {
            // Arrange
            int userId = 1;
            int? categoryId = 10;
            PaymentMethod paymentMethod = PaymentMethod.CreditCard;
            TransactionType transactionType = TransactionType.Expense;
            Money amount = CreateValidMoney();
            string description = " ";
            DateTime transactionDate = DateTime.Now;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Transaction(
                userId, categoryId, paymentMethod, transactionType, amount, description, transactionDate));
            Assert.Equal("description", exception.ParamName);
        }

        [Fact(DisplayName = "Dado data local, Quando criar transação, Então TransactionDate deve ser convertido para Utc")]
        public void Deve_Converter_TransactionDate_Para_Utc_No_Construtor()
        {
            // Arrange
            int userId = 1;
            int? categoryId = null;
            PaymentMethod paymentMethod = PaymentMethod.Cash;
            TransactionType transactionType = TransactionType.Income;
            Money amount = CreateValidMoney();
            string description = "Test Transaction";
            DateTime localTime = DateTime.Now;

            // Act
            var transaction = new Transaction(
                userId, categoryId, paymentMethod, transactionType, amount, description, localTime);

            // Assert
            Assert.Equal(DateTimeKind.Utc, transaction.TransactionDate.Kind);
            Assert.Equal(DateTime.SpecifyKind(localTime, DateTimeKind.Utc), transaction.TransactionDate);
        }

        #endregion

        #region Update Method Tests

        [Fact(DisplayName = "Dado parâmetros válidos, Quando atualizar transação, Então deve atualizar corretamente")]
        public void Deve_Atualizar_Transacao_Com_Parametros_Validos()
        {
            // Arrange
            var transaction = new Transaction(
                1, 10, PaymentMethod.CreditCard, TransactionType.Expense, CreateValidMoney(), "Old Description", DateTime.Now.AddDays(-1));

            int? newCategoryId = 20;
            PaymentMethod newPaymentMethod = PaymentMethod.DebitCard;
            TransactionType newTransactionType = TransactionType.Income;
            Money newAmount = Money.Create(200, Currency.EUR);
            string newDescription = "New Description";
            DateTime newTransactionDate = DateTime.Now;
            bool newIsRecurring = true;

            // Act
            transaction.Update(
                newCategoryId, newPaymentMethod, newTransactionType, newAmount, newDescription, newTransactionDate, newIsRecurring);

            // Assert
            Assert.Equal(newCategoryId, transaction.CategoryId);
            Assert.Equal(newPaymentMethod, transaction.PaymentMethod);
            Assert.Equal(newTransactionType, transaction.TransactionType);
            Assert.Equal(newAmount, transaction.Amount);
            Assert.Equal(newDescription, transaction.Description);
            Assert.Equal(DateTime.SpecifyKind(newTransactionDate, DateTimeKind.Utc), transaction.TransactionDate);
            Assert.Equal(newIsRecurring, transaction.IsRecurring);
        }

        [Fact(DisplayName = "Dado amount nulo, Quando atualizar transação, Então deve lançar ArgumentNullException")]
        public void Deve_Lancar_ArgumentNullException_Para_Amount_Nulo_No_Update()
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

        [Fact(DisplayName = "Dado description nulo, Quando atualizar transação, Então deve lançar ArgumentNullException")]
        public void Deve_Lancar_ArgumentNullException_Para_Description_Nulo_No_Update()
        {
            // Arrange
            var transaction = new Transaction(
                1, 10, PaymentMethod.CreditCard, TransactionType.Expense, CreateValidMoney(), "Description", DateTime.Now);
            string? description = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => transaction.Update(
                10, PaymentMethod.DebitCard, TransactionType.Income, CreateValidMoney(), description!, DateTime.Now));
            Assert.Equal("description", exception.ParamName);
        }

        [Fact(DisplayName = "Dado description vazio, Quando atualizar transação, Então deve lançar ArgumentException")]
        public void Deve_Lancar_ArgumentException_Para_Description_Vazio_No_Update()
        {
            // Arrange
            var transaction = new Transaction(
                1, 10, PaymentMethod.CreditCard, TransactionType.Expense, CreateValidMoney(), "Description", DateTime.Now);
            string description = string.Empty;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => transaction.Update(
                10, PaymentMethod.DebitCard, TransactionType.Income, CreateValidMoney(), description, DateTime.Now));
            Assert.Equal("description", exception.ParamName);
        }

        [Fact(DisplayName = "Dado description com espaço em branco, Quando atualizar transação, Então deve lançar ArgumentException")]
        public void Deve_Lancar_ArgumentException_Para_Description_Whitespace_No_Update()
        {
            // Arrange
            var transaction = new Transaction(
                1, 10, PaymentMethod.CreditCard, TransactionType.Expense, CreateValidMoney(), "Description", DateTime.Now);
            string description = " ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => transaction.Update(
                10, PaymentMethod.DebitCard, TransactionType.Income, CreateValidMoney(), description, DateTime.Now));
            Assert.Equal("description", exception.ParamName);
        }

        [Theory(DisplayName = "Dado categoryId inválido, Quando atualizar transação, Então deve lançar ArgumentOutOfRangeException")]
        [InlineData(0)]
        [InlineData(-5)]
        public void Deve_Lancar_ArgumentOutOfRangeException_Para_CategoryId_Invalido_No_Update(int invalidCategoryId)
        {
            // Arrange
            var transaction = new Transaction(
                1, 10, PaymentMethod.CreditCard, TransactionType.Expense, CreateValidMoney(), "Description", DateTime.Now);

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => transaction.Update(
                invalidCategoryId, PaymentMethod.DebitCard, TransactionType.Income, CreateValidMoney(), "New Description", DateTime.Now));
            Assert.Equal("categoryId.Value", exception.ParamName);
        }

        [Fact(DisplayName = "Dado data local, Quando atualizar transação, Então TransactionDate deve ser convertido para Utc")]
        public void Deve_Converter_TransactionDate_Para_Utc_No_Update()
        {
            // Arrange
            var transaction = new Transaction(
                1, null, PaymentMethod.Cash, TransactionType.Income, CreateValidMoney(), "Description", DateTime.Now.AddDays(-1));
            DateTime localTime = DateTime.Now;

            // Act
            transaction.Update(
                null, PaymentMethod.Cash, TransactionType.Income, CreateValidMoney(), "New Description", localTime);

            // Assert
            Assert.Equal(DateTimeKind.Utc, transaction.TransactionDate.Kind);
            Assert.Equal(DateTime.SpecifyKind(localTime, DateTimeKind.Utc), transaction.TransactionDate);
        }

        #endregion
    }
}