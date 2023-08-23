using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Acme.BookStore.UserInfos
{
    public class CreateUserInfoDto

    {
        [Required]
        [StringLength(UserInfoConsts.MaxNameLength)]
        public string LastName { get; set; }

        [Required]
        [StringLength(UserInfoConsts.MaxNameLength)]
        public string FirstName { get; set; }

        public string AvatarPath { get; set; }

        public DateTime DOB { get; set; }

        public JobType Job { get; set; }

        public string Address { get; set; }

        public Guid UserId { get; set; }
        public string FullName { get { return $"{FirstName} {LastName}"; } }

    }
}
