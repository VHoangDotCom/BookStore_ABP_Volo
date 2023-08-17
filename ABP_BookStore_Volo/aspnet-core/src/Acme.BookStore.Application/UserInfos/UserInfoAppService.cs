using Acme.BookStore.Authors;
using Acme.BookStore.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.UserInfos
{
    [Authorize(BookStorePermissions.UserInfos.Default)]
    public class UserInfoAppService : BookStoreAppService, IUserInfoAppService
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly UserInfoManager _userInfoManager;

        public UserInfoAppService(
            IUserInfoRepository userInfoRepository,
            UserInfoManager userInfoManager)
        {
            _userInfoRepository = userInfoRepository;
            _userInfoManager = userInfoManager;
        }

        public async Task<UserInfoDto> GetAsync(Guid id)
        {
            var userInfo = await _userInfoRepository.GetAsync(id);
            return ObjectMapper.Map<UserInfo, UserInfoDto>(userInfo);
        }

        public async Task<PagedResultDto<UserInfoDto>> GetListAsync(GetUserInfoListDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(UserInfo.FirstName);
            }

            var userInfos = await _userInfoRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter
                );

            var totalCount = input.Filter == null
              ? await _userInfoRepository.CountAsync()
              : await _userInfoRepository.CountAsync(
                  user => user.FirstName.Contains(input.Filter) ||
                          user.LastName.Contains(input.Filter) ||
                          user.Job.ToString().Contains(input.Filter) ||
                          user.Address.Contains(input.Filter));

            return new PagedResultDto<UserInfoDto>(
                totalCount,
                ObjectMapper.Map<List<UserInfo>, List<UserInfoDto>>(userInfos) 
                );
        }


        [Authorize(BookStorePermissions.UserInfos.Create)]
        public async Task<UserInfoDto> CreateAsync(CreateOrUpdateUserInfoDto input)
        {
            var userInfo = await _userInfoManager.CreateAsync(
                 input.FirstName,
                 input.LastName,
                 input.AvatarPath,
                 input.DOB,
                 input.Job,
                 input.UserId,
                 input.Address
                );

            await _userInfoRepository.InsertAsync(userInfo);

            return ObjectMapper.Map<UserInfo, UserInfoDto>(userInfo);
        }

        [Authorize(BookStorePermissions.UserInfos.Edit)]
        public async Task UpdateAsync(Guid id, CreateOrUpdateUserInfoDto input)
        {
            var userInfo = await _userInfoRepository.GetAsync( id );

            if (userInfo.FirstName != input.FirstName || userInfo.LastName != input.LastName)
            {
                await _userInfoManager.ChangeNameAsync(userInfo, input.FirstName, input.LastName);  
            }

            userInfo.AvatarPath = input.AvatarPath;
            userInfo.Job = input.Job;
            userInfo.DOB = input.DOB;
            userInfo.Address = input.Address;

            await _userInfoRepository.UpdateAsync(userInfo);
        }

        [Authorize(BookStorePermissions.UserInfos.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            await _userInfoRepository.DeleteAsync( id );
        }
      
    }
}
