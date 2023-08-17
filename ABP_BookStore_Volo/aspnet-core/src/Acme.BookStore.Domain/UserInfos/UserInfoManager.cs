using Acme.BookStore.Authors;
using Acme.BookStore.UserInfos;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Acme.BookStore.UserInfos
{
    public class UserInfoManager : DomainService
    {
        private readonly IUserInfoRepository _userInfoRepository;
        public UserInfoManager(IUserInfoRepository userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
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
            var fullName = firstName + lastName;
            Check.NotNullOrWhiteSpace(fullName, nameof(fullName));

            var existingAuthor = await _userInfoRepository.FindByNameAsync(fullName);
            if (existingAuthor != null)
            {
                throw new UserAlreadyExistsException(fullName);
            }

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
                throw new UserAlreadyExistsException(fullName);
            }

            userInfo.ChangeName(firstName, lastName);
        }

    }
}
