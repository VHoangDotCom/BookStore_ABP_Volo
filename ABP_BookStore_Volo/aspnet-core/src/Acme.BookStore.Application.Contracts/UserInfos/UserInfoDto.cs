using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.UserInfos
{
    public class UserInfoDto : EntityDto<Guid>
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string AvatarPath { get; set; }
        public DateTime DOB { get; set; }
        public JobType Job { get; set; }
        public string Address { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
