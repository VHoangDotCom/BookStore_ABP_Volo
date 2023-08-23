using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.UserInfos
{
    public class UserLookupDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
