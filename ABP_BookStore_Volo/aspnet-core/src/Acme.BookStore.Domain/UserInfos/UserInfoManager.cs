using Acme.BookStore.Authors;
using Acme.BookStore.User;
using Acme.BookStore.UserInfos;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;

namespace Acme.BookStore.UserInfos
{
    public class UserInfoManager : DomainService
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IUserRepository _userRepository;
        public UserInfoManager(
            IUserInfoRepository userInfoRepository,
            IUserRepository userRepository)
        {
            _userInfoRepository = userInfoRepository;
            _userRepository = userRepository;
        }

        public async Task<UserInfo> CreateAsync(
            [NotNull] string firstName,
            [NotNull] string lastName,
            string avatarPath,
            DateTime birthDate,
            JobType job,
            Guid userID,
            [CanBeNull] string address = null)
        {
            var fullName = $"{firstName} {lastName}";
            Check.NotNullOrWhiteSpace(fullName, nameof(fullName));

            await ValidateUserInfoCreation(fullName, userID);

            return new UserInfo(
                GuidGenerator.Create(),
                firstName,
                lastName,
                avatarPath,
                birthDate,
                job,
                address,
                userID
            );
        }

        private async Task ValidateUserInfoCreation(string fullName, Guid userID)
        {
            await CheckIfAuthorExists(fullName);
            await CheckIfUserInfoExists(userID);
            await CheckIfUserExists(userID);
        }

        private async Task CheckIfAuthorExists(string fullName)
        {
            var existingAuthor = await _userInfoRepository.FindByNameAsync(fullName);
            if (existingAuthor != null)
            {
                throw new UserInfoAlreadyExistsException(fullName);
            }
        }

        private async Task CheckIfUserInfoExists(Guid userID)
        {
            var existedUserInfo = await _userInfoRepository.FindUserHasInfoAlready(userID);
            var existedUser = await _userRepository.FindByIdAsync(userID);
            if (existedUserInfo != null)
            {
                throw new UserHasInfoAlreadyException($"{existedUser.Name} - {existedUser.Email}");
            }
        }

        private async Task CheckIfUserExists(Guid userID)
        {
            var existingUser = await _userRepository.FindByIdAsync(userID);
            if (existingUser == null)
            {
                throw new UserNotFoundException(userID);
            }
        }

        public async Task<IdentityUser> GetUserDetail(Guid userID)
        {
            return await _userRepository.FindByIdAsync(userID);
        }

        public async Task ChangeNameAsync(
          [NotNull] UserInfo userInfo,
          [NotNull] string firstName,
          [NotNull] string lastName)
        {
            Check.NotNull(userInfo, nameof(userInfo));
            Check.NotNullOrWhiteSpace(firstName, nameof(firstName));
            Check.NotNullOrWhiteSpace(lastName, nameof(lastName));

            var fullName = firstName + lastName;
            var existingUser = await _userInfoRepository.FindByNameAsync(fullName);
            if (existingUser != null && existingUser.Id != userInfo.Id)
            {
                throw new UserInfoAlreadyExistsException(fullName);
            }

            userInfo.ChangeName(firstName, lastName);
        }

    }
}
