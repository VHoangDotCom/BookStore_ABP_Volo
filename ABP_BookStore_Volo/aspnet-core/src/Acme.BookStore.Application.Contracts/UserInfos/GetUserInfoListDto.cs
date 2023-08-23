using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.UserInfos
{
    public class GetUserInfoListDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public int? JobType { get; set; }
    }
}
