using CloudinaryTest.Dto;
using CloudinaryTest.Entities;
using AutoMapper;

namespace CloudinaryTest.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CloudFolder, CloudFolderDto>();
            CreateMap<CloudFolderDto, CloudFolder>();
            CreateMap<CreateCloudFolderDto, CloudFolder>();
            CreateMap<CloudFolder, CreateCloudFolderDto>();
            CreateMap<CloudFolder, UpdateCloudFolderDto>();
            CreateMap<UpdateCloudFolderDto, CloudFolder>();

            CreateMap<CloudFile, GetCloudFileDto>();
            CreateMap<GetCloudFileDto, CloudFile>();
            CreateMap<CreateCloudFileDto, CloudFile>();
            CreateMap<CloudFile, CreateCloudFileDto > ();
            CreateMap<CloudFile, UpdateCloudFileDto>();
            CreateMap<UpdateCloudFileDto, CloudFile>();
        }
    }
}
