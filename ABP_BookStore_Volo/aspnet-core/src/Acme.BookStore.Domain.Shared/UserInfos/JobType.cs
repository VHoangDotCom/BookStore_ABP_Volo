using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Acme.BookStore.UserInfos
{
    public enum JobType
    {
        [EnumMember(Value = "None")]
        None = 0,

        [EnumMember(Value = "Teacher")]
        Teacher = 1,

        [EnumMember(Value = "Student")]
        Student = 2,

        [EnumMember(Value = "Developer")]
        Developer = 3,
    }
}
