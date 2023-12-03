using Accounts.Common;
using Accounts.DataAccess.Contracts;
using Accounts.Domain.Models;
using Accounts.Service.Contracts;
using Common.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Accounts.Service.UnitTests
{
    [TestFixture]
    public class AccountServiceTest
    {
        private Mock<IAccountRepository> accountRepositoryMock;
        private Mock<IConfiguration> configurationMock;
        private Mock<IQueueMessageService> queueMessageServiceMock;
        private Mock<ILogger<AccountService>> loggerMock;

        private IAccountService accountService;

        [SetUp]
        public void Setup()
        {
            accountRepositoryMock = new Mock<IAccountRepository>();
            configurationMock = new Mock<IConfiguration>();
            queueMessageServiceMock = new Mock<IQueueMessageService>();
            loggerMock = new Mock<ILogger<AccountService>>();

            configurationMock
                .Setup(conf => conf[Constants.TRANSACTION_INSERT_QUEUE_SETTINGS_QUEUE])
                .Returns("YourTransactionInsertQueue");

            accountService = new AccountService(
                accountRepositoryMock.Object,
                configurationMock.Object,
                queueMessageServiceMock.Object,
                loggerMock.Object
            );
        }

        [Test]
        public async Task CreateAccount_WithInitialCreditGreaterThanZero_QueueMessageCalledOnce()
        {
            // Arrange
            var customerId = 1001;
            var initialCredit = 100;
            var result = (result: DBErrorCode.SUCCESS, accountId: 1); // Example result

            accountRepositoryMock
                .Setup(repo => repo.CreateAccount(customerId, initialCredit))
                .Returns(Task.FromResult(result));

            // Act
            await accountService.CreateAccount(customerId, initialCredit);

            // Assert
            queueMessageServiceMock.Verify(
                q => q.QueueMessage(It.IsAny<EventMessage>(), It.IsAny<string>(), false),
                Times.Once
            );
        }

        [Test]
        public async Task CreateAccount_WithInitialCreditZeroOrLess_NoQueueMessageCalled()
        {
            // Arrange
            var customerId = 1001;
            var initialCredit = 0M;
            var result = (result: DBErrorCode.SUCCESS, accountId: 1); // Example result

            accountRepositoryMock
                .Setup(repo => repo.CreateAccount(customerId, initialCredit))
                .Returns(Task.FromResult(result));

            // Act
            await accountService.CreateAccount(customerId, initialCredit);

            // Assert
            queueMessageServiceMock.Verify(
                q => q.QueueMessage(It.IsAny<EventMessage>(), It.IsAny<string>(), false),
                Times.Never // Ensure that QueueMessage was not called
            );
        }

        [Test]
        public async Task CreateAccount_SuccessfulAccountCreationWithInitialCredit_ZeroInitialCreditAndSuccessStatusReturned()
        {
            // Arrange
            var customerId = 1001;
            var initialCredit = 0M;
            var result = (result: DBErrorCode.SUCCESS, accountId: 1); // Example result

            accountRepositoryMock
                .Setup(repo => repo.CreateAccount(customerId, initialCredit))
                .Returns(Task.FromResult(result));

            // Act
            var (resultCode, accountId) = await accountService.CreateAccount(customerId, initialCredit);

            // Assert
            Assert.AreEqual(DBErrorCode.SUCCESS, resultCode);
            Assert.AreEqual(1, accountId);
            queueMessageServiceMock.Verify(
                q => q.QueueMessage(It.IsAny<EventMessage>(), It.IsAny<string>(), false),
                Times.Never // Ensure that QueueMessage was not called
            );
        }

        [Test]
        public async Task CreateAccount_SuccessfulAccountCreationWithInitialCredit_NonZeroInitialCreditAndSuccessStatusReturned()
        {
            // Arrange
            var customerId = 1001;
            var initialCredit = 50M; // Non-zero initial credit
            var result = (result: DBErrorCode.SUCCESS, accountId: 1); 

            accountRepositoryMock
                .Setup(repo => repo.CreateAccount(customerId, initialCredit))
                .Returns(Task.FromResult(result));

            // Act
            var (resultCode, accountId) = await accountService.CreateAccount(customerId, initialCredit);

            // Assert
            Assert.AreEqual(DBErrorCode.SUCCESS, resultCode);
            Assert.AreEqual(1, accountId);
            queueMessageServiceMock.Verify(
                q => q.QueueMessage(It.IsAny<EventMessage>(), It.IsAny<string>(), false),
                Times.Once // Ensure that QueueMessage was called once
            );
        }

        [Test]
        public async Task CreateAccount_UnsuccessfulAccountCreation_ZeroInitialCreditAndErrorStatusReturned()
        {
            // Arrange
            var customerId = 1001;
            var initialCredit = 0M;

            accountRepositoryMock
                .Setup(repo => repo.CreateAccount(customerId, initialCredit))
                .ReturnsAsync((DBErrorCode.ACCOUNT_ALREADY_EXISTS, accountId: 0));

            // Act
            var (resultCode, accountId) = await accountService.CreateAccount(customerId, initialCredit);

            // Assert
            Assert.AreEqual(DBErrorCode.ACCOUNT_ALREADY_EXISTS, resultCode);
            Assert.AreEqual(0, accountId); 
            queueMessageServiceMock.Verify(
                q => q.QueueMessage(It.IsAny<EventMessage>(), It.IsAny<string>(), false),
                Times.Never
            );
        }

        [Test]
        public async Task CreateAccount_UnsuccessfulAccountCreation_NonZeroInitialCreditAndErrorStatusReturned()
        {
            // Arrange
            var customerId = 1001;
            var initialCredit = 100M;

            accountRepositoryMock
                .Setup(repo => repo.CreateAccount(customerId, initialCredit))
                .ReturnsAsync((DBErrorCode.ACCOUNT_ALREADY_EXISTS, accountId: 0));

            // Act
            var (resultCode, accountId) = await accountService.CreateAccount(customerId, initialCredit);

            // Assert
            Assert.AreEqual(DBErrorCode.ACCOUNT_ALREADY_EXISTS, resultCode);
            Assert.AreEqual(0, accountId); // Ensure no account ID is returned
            queueMessageServiceMock.Verify(
                q => q.QueueMessage(It.IsAny<EventMessage>(), It.IsAny<string>(), false),
                Times.Never 
            );
        }

        [Test]
        public async Task RemoveAccount_ReturnsCorrectResult()
        {
            // Arrange
            var customerId = 1001;
            var accountId = 456;
            var expectedResult = 1; // Indicating successful removal

            accountRepositoryMock
                .Setup(repo => repo.RemoveAccount(customerId, accountId))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await accountService.RemoveAccount(customerId, accountId);

            // Assert
            Assert.AreEqual(expectedResult, result);
            accountRepositoryMock.Verify(repo => repo.RemoveAccount(customerId, accountId), Times.Once);
        }

        [Test]
        public async Task GetCustomerInfo_ReturnsValidCustomerInfo()
        {
            // Arrange
            var customerId = 1001;
            var expectedCustomerInfo = new CustomerInfoObj
            {
                // ... Populate this object with expected customer information for the given ID ...
            };

            accountRepositoryMock
                .Setup(repo => repo.GetCustomerInfo(customerId))
                .ReturnsAsync(expectedCustomerInfo);

            // Act
            var customerInfo = await accountService.GetCustomerInfo(customerId);

            // Assert
            Assert.IsNotNull(customerInfo); // Ensure customer info is not null
                                            // Perform additional assertions on customerInfo properties as needed
                                            // For example:
            Assert.AreEqual(expectedCustomerInfo.Name, customerInfo.Name);
            Assert.AreEqual(expectedCustomerInfo.Surname, customerInfo.Surname);

            accountRepositoryMock.Verify(repo => repo.GetCustomerInfo(customerId), Times.Once);
        }
    }
}