using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.BookStore.UserInfos
{
    public class UserInfo : FullAuditedAggregateRoot<Guid>
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string AvatarPath { get; set; }
        public DateTime DOB { get; set; }
        public JobType Job { get; set; }
        public string Address { get; set; }
        public Guid UserId { get; set; }

        public UserInfo()
        {
            
        }

        internal UserInfo(
            Guid id,
            [NotNull] string firstName,
            [NotNull] string lastName,
            string avatarPath,
            DateTime dob,
            JobType job,
            string address,
            Guid userId)
            : base(id)
        {
            SetFirstName(firstName);
            SetLastName(lastName);
            AvatarPath = avatarPath;
            DOB  = dob;
            Job = job;
            Address = address;
            UserId = userId;
        }

        internal UserInfo ChangeName([NotNull] string firstName, [NotNull] string lastName)
        {
            SetFirstName(firstName); SetLastName(lastName); return this;
        }

        private void SetFirstName([NotNull] string name)
        {
            FirstName = Check.NotNullOrWhiteSpace(
                name,
                nameof(name),
                maxLength: UserInfoConsts.MaxNameLength
            );
        }

        private void SetLastName([NotNull] string name)
        {
            LastName = Check.NotNullOrWhiteSpace(
                name,
                nameof(name),
                maxLength: UserInfoConsts.MaxNameLength
            );
        }
    }
}
