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
        Task<CreateUserInfoDto> CreateAsync(CreateUserInfoDto input);

        Task UpdateAsync(Guid id, UpdateUserInfoDto input);

        Task DeleteAsync(Guid id);

        Task<ListResultDto<UserLookupDto>> GetUserLookupAsync();
    }
}
