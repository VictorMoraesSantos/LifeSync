using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;
using LifeSyncApp.ViewModels.Financial;

namespace LifeSyncApp.Tests.ViewModels.Financial
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "ViewModel")]
    public class ManageTransactionViewModelTests
    {
        private readonly Mock<TransactionService> _mockTransactionService;
        private readonly Mock<CategoryService> _mockCategoryService;
        private readonly ManageTransactionViewModel _viewModel;

        public ManageTransactionViewModelTests()
        {
            _mockTransactionService = new Mock<TransactionService>();
            _mockCategoryService = new Mock<CategoryService>();
            _viewModel = new ManageTransactionViewModel(
                _mockTransactionService.Object,
                _mockCategoryService.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializePaymentMethods()
        {
            // Assert
            _viewModel.PaymentMethods.Should().NotBeEmpty();
            _viewModel.PaymentMethods.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public void Constructor_ShouldSetCashAsDefaultPaymentMethod()
        {
            // Assert
            var cashMethod = _viewModel.PaymentMethods.FirstOrDefault(p => p.Value == PaymentMethod.Cash);
            cashMethod.Should().NotBeNull();
            cashMethod!.IsSelected.Should().BeTrue();
        }

        [Fact]
        public void TogglePaymentMethod_WhenCalled_ShouldDeselectAllOthers()
        {
            // Arrange
            var pix = _viewModel.PaymentMethods.First(p => p.Value == PaymentMethod.Pix);
            var credit = _viewModel.PaymentMethods.First(p => p.Value == PaymentMethod.CreditCard);

            // Act
            _viewModel.TogglePaymentMethodCommand.Execute(pix);
            _viewModel.TogglePaymentMethodCommand.Execute(credit);

            // Assert
            pix.IsSelected.Should().BeFalse();
            credit.IsSelected.Should().BeTrue();
            _viewModel.PaymentMethod.Should().Be(PaymentMethod.CreditCard);
        }

        [Fact]
        public void TogglePaymentMethod_WhenCalled_ShouldUpdatePaymentMethodProperty()
        {
            // Arrange
            var debitCard = _viewModel.PaymentMethods.First(p => p.Value == PaymentMethod.DebitCard);

            // Act
            _viewModel.TogglePaymentMethodCommand.Execute(debitCard);

            // Assert
            _viewModel.PaymentMethod.Should().Be(PaymentMethod.DebitCard);
        }

        [Fact]
        public void Description_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ManageTransactionViewModel.Description))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.Description = "Test transaction";

            // Assert
            propertyChangedRaised.Should().BeTrue();
            _viewModel.Description.Should().Be("Test transaction");
        }

        [Fact]
        public void Amount_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ManageTransactionViewModel.Amount))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.Amount = 150.50m;

            // Assert
            propertyChangedRaised.Should().BeTrue();
            _viewModel.Amount.Should().Be(150.50m);
        }

        [Fact]
        public void Title_ShouldBeNovaTransacao_WhenNotEditing()
        {
            // Assert
            _viewModel.Title.Should().Be("Nova Transação");
            _viewModel.IsEditing.Should().BeFalse();
        }

        // Note: Additional tests for SaveCommand, CancelCommand, and InitializeAsync
        // would require more complex mocking of MAUI Application and navigation services.
        // These can be added as integration tests or with a proper test harness.
    }
}
