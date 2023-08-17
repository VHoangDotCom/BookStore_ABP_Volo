using Acme.BookStore.Authors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.UserInfos
{
    public interface IUserInfoAppService : IApplicationService
    {
        Task<UserInfoDto> GetAsync(Guid id);
        Task<PagedResultDto<UserInfoDto>> GetListAsync(GetUserInfoListDto input);
        Task<UserInfoDto> CreateAsync(CreateOrUpdateUserInfoDto input);

        Task UpdateAsync(Guid id, CreateOrUpdateUserInfoDto input);

        Task DeleteAsync(Guid id);
    }
}
