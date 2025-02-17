using Application.DTOs.Account;
using Application.DTOs.Authenticate;
using Application.DTOs.Base;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Service;
using Domain.Entity;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Services
{
    public class AccountServiceTests
    {
        //Init data
        private readonly Account mockAccountData = new() { Id = 1, Email = "test@gmail.com" };
        private readonly List<Account> mockAccountList;
        //Inject service
        private IAccountService accountService;
        private readonly Mock<ITimeService> mockTimeService;
        private readonly Mock<ICacheService> mockCacheService;
        private readonly Mock<IClaimService> mockClaimService;
        private readonly Mock<IOptions<JwtSetting>> mockJwtSetting;
        private readonly Mock<IAccountRepo> mockAccountRepo;
        private readonly Mock<IUnitOfWork> mockUnitOfWork;
        private readonly Mock<DbSet<Account>> mockAccountQueryable;

        public AccountServiceTests()
        {
            var mockPassword = "123123";
            mockAccountData.PasswordHash = mockPassword.Hash();
            mockAccountList = [mockAccountData];
            mockTimeService = new Mock<ITimeService>();
            mockCacheService = new Mock<ICacheService>();
            mockClaimService = new Mock<IClaimService>();
            mockJwtSetting = new Mock<IOptions<JwtSetting>>();
            mockAccountRepo = new Mock<IAccountRepo>();
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockAccountQueryable = mockAccountList.AsQueryable().BuildMockDbSet();
            accountService = new AccountService(mockAccountRepo.Object, mockUnitOfWork.Object, mockJwtSetting.Object,
                                                mockTimeService.Object, mockCacheService.Object, mockClaimService.Object);
        }

        #region GetAccountInformationAsync
        [Fact]
        public async Task GetAccountInformationAsync_ShouldReturnAccountInformation_WhenUserIsAuthenticated()
        {
            //Arrange
            mockClaimService.Setup(claimService => claimService.GetCurrentUserId()).Returns(mockAccountData.Id);
            mockAccountRepo.Setup(repo => repo.FirstOrDefaultAsync(mockAccountData.Id)).ReturnsAsync(mockAccountData);

            //Act
            var result = await accountService.GetAccountInformationAsync();

            //Assert
            Assert.IsType<ResponseResult<AccountVM>>(result);
            Assert.NotNull(result.Data);
            Assert.Equal(mockAccountData.Id, result.Data.Id);
            Assert.True(result.IsSucceed);
        }

        [Fact]
        public async Task GetAccountInformationAsync_ShouldThrowUnauthorizedAccessException_WhenInvalidUser()
        {
            //Arrange
            mockClaimService.Setup(claimService => claimService.GetCurrentUserId()).Returns(-1);
            mockAccountRepo.Setup(repo => repo.FirstOrDefaultAsync(mockAccountData.Id)).ReturnsAsync((Account?)null);

            //Act
            await Assert.ThrowsAsync<UnauthorizedAccessException>(accountService.GetAccountInformationAsync);
        }
        #endregion

        #region AuthenticateAsync
        //[Fact]
        //public async Task AuthenticateAsync_ShouldReturnToken_WhenValidEmailAndPassword()
        //{
        //    //Arrange
        //    var now = DateTime.UtcNow;
        //    var authRequestDto = new AuthenticateRequest() { Email = mockAccountData.Email, Password = "123123" };
        //    //
        //    mockAccountRepo.Setup(repo => repo.GetAll(It.IsAny<bool>())).Returns(mockAccountQueryable.Object);
        //    mockTimeService.Setup(timeService => timeService.GetCurrentUtcDatetime()).Returns(now);
        //    mockCacheService.Setup(cacheService => cacheService.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
        //        .Verifiable();

        //    //Act
        //    var result = await accountService.AuthenticateAsync(authRequestDto);

        //    //Assert
        //    Assert.IsType<ResponseResult<AuthenticateResult>>(result);
        //    Assert.NotNull(result.Data);
        //    Assert.NotEmpty(result.Data.AccessToken);
        //    Assert.NotEmpty(result.Data.RefreshToken);
        //    Assert.True(result.IsSucceed);
        //    Assert.NotNull(result.Message);
        //}
        #endregion

        #region RegisterAsync
        [Fact]
        public async Task RegisterAsync_ShouldCreateAccount_WhenInputValid()
        {
            //Arrange
            var registerRequestDto = new RegisterRequest() { Email = mockAccountData.Email, Password = "123123" };
            mockAccountRepo.Setup(repo => repo.AddAsync(It.IsAny<Account>())).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(true);

            //Act
            var result = await accountService.RegisterAsync(registerRequestDto);

            //Assert
            mockAccountRepo.Verify(repo => repo.AddAsync(It.IsAny<Account>()), Times.Once());
            mockUnitOfWork.Verify(uow => uow.SaveChangeAsync(), Times.Once);

            Assert.IsType<ResponseResult<AccountVM>>(result);
            Assert.NotNull(result.Data);
            Assert.Equal(registerRequestDto.Email, result.Data.Email);
            Assert.True(result.IsSucceed);
            Assert.NotEmpty(result.Message);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowDbUpdateException_WhenSaveToDatabaseFailed()
        {

            //Arrange
            var registerRequestDto = new RegisterRequest() { Email = mockAccountData.Email, Password = "123123" };
            mockAccountRepo.Setup(repo => repo.AddAsync(It.IsAny<Account>())).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(false);

            //Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () => await accountService.RegisterAsync(registerRequestDto));
        }
        #endregion

        #region UpdateAccountAsync
        [Fact]
        public async Task UpdateAccountAsync_ShouldUpdateAccount_WhenInputValid()
        {
            //Arrange
            var accountUpdateRequestDto = new AccountUpdateRequest();
            mockClaimService.Setup(claimService => claimService.GetCurrentUserId()).Returns(mockAccountData.Id);
            mockAccountRepo.Setup(repo => repo.FirstOrDefaultAsync(mockAccountData.Id)).ReturnsAsync(mockAccountData);
            mockAccountRepo.Setup(repo => repo.Update(mockAccountData)).Verifiable();
            mockUnitOfWork.Setup(repo => repo.SaveChangeAsync()).ReturnsAsync(true);

            //Act
            var result = await accountService.UpdateAccountAsync(accountUpdateRequestDto);

            //Assert
            mockAccountRepo.Verify(repo => repo.Update(mockAccountData), Times.Once);
            mockUnitOfWork.Verify(repo => repo.SaveChangeAsync(), Times.Once);

            Assert.IsType<ResponseResult<AccountVM>>(result);
            Assert.NotNull(result.Data);
            Assert.Equal(mockAccountData.Email, result.Data.Email);
            Assert.True(result.IsSucceed);
            Assert.NotEmpty(result.Message);
        }

        [Fact]
        public async Task UpdateAccountAsync_ShouldThrowUnauthorizedAccessException_WhenInvalidUser()
        {
            //Arrange
            var accountUpdateRequestDto = new AccountUpdateRequest();
            mockClaimService.Setup(claimService => claimService.GetCurrentUserId()).Returns(-1);
            mockAccountRepo.Setup(repo => repo.FirstOrDefaultAsync(mockAccountData.Id)).ReturnsAsync((Account?)null);

            //Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await accountService.UpdateAccountAsync(accountUpdateRequestDto));
        }

        [Fact]
        public async Task UpdateAccountAsync_ShouldThrowDbUpdateException_WhenSaveToDatabaseFailed()
        {
            //Arrange
            var accountUpdateRequestDto = new AccountUpdateRequest();
            mockClaimService.Setup(claimService => claimService.GetCurrentUserId()).Returns(mockAccountData.Id);
            mockAccountRepo.Setup(repo => repo.FirstOrDefaultAsync(mockAccountData.Id)).ReturnsAsync(mockAccountData);
            mockAccountRepo.Setup(repo => repo.Update(mockAccountData)).Verifiable();
            mockUnitOfWork.Setup(repo => repo.SaveChangeAsync()).ReturnsAsync(false);

            //Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () => await accountService.UpdateAccountAsync(accountUpdateRequestDto));
        }
        #endregion

        #region ChangePasswordAsync
        [Fact]
        public async Task ChangePasswordAsync_ShouldUpdateAccountPassword_WhenPasswordIsValid()
        {
            //Arrange
            var changePassRequestDto = new AccountChangePasswordRequest() { OldPassword = "123123", NewPassword = "123123123" };
            mockClaimService.Setup(claimService => claimService.GetCurrentUserId()).Returns(mockAccountData.Id);
            mockAccountRepo.Setup(repo => repo.FirstOrDefaultAsync(mockAccountData.Id)).ReturnsAsync(mockAccountData);
            mockAccountRepo.Setup(repo => repo.Update(mockAccountData)).Verifiable();
            mockUnitOfWork.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(true);

            //Act
            var result = await accountService.ChangePasswordAsync(changePassRequestDto);

            //Assert
            mockAccountRepo.Verify(repo => repo.Update(mockAccountData), Times.Once);
            mockUnitOfWork.Verify(uow => uow.SaveChangeAsync(), Times.Once);

            Assert.IsType<ResponseResult<string>>(result);
            Assert.NotNull(result.Data);
            Assert.True(result.IsSucceed);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrowUnauthorizedAccessException_WhenInvalidUser()
        {
            //Arrange
            var changePassRequestDto = new AccountChangePasswordRequest();
            mockClaimService.Setup(claimService => claimService.GetCurrentUserId()).Returns(-1);
            mockAccountRepo.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<int>())).ReturnsAsync((Account?)null);

            //Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await accountService.ChangePasswordAsync(changePassRequestDto));
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrowDbUpdateException_WhenSaveToDatabaseFailed()
        {
            //Arrange
            var changePassRequestDto = new AccountChangePasswordRequest() { OldPassword = "123123", NewPassword = "123123123" };
            mockClaimService.Setup(claimService => claimService.GetCurrentUserId()).Returns(mockAccountData.Id);
            mockAccountRepo.Setup(repo => repo.FirstOrDefaultAsync(mockAccountData.Id)).ReturnsAsync(mockAccountData);
            mockAccountRepo.Setup(repo => repo.Update(mockAccountData)).Verifiable();
            mockUnitOfWork.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(false);

            //Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () => await accountService.ChangePasswordAsync(changePassRequestDto));
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnIncorrectMessage_WhenIncorrectOldPassword()
        {
            //Arrange
            var changePassRequestDto = new AccountChangePasswordRequest() { OldPassword = "123" };
            mockClaimService.Setup(claimService => claimService.GetCurrentUserId()).Returns(mockAccountData.Id);
            mockAccountRepo.Setup(repo => repo.FirstOrDefaultAsync(mockAccountData.Id)).ReturnsAsync(mockAccountData);

            //Act
            var result = await accountService.ChangePasswordAsync(changePassRequestDto);

            //Assert
            Assert.IsType<ResponseResult<string>>(result);
            Assert.False(result.IsSucceed);
            Assert.NotEmpty(result.Message);
        }
        #endregion
    }
}
