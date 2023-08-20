import type { JobType } from './job-type.enum';
import type { AuditedEntityDto, EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUserInfoDto {
  lastName: string;
  firstName: string;
  avatarPath?: string;
  dob?: string;
  job: JobType;
  address?: string;
  userId?: string;
  fullName?: string;
}

export interface GetUserInfoListDto extends PagedAndSortedResultRequestDto {
  filter?: string;
  jobType?: number;
}

export interface UpdateUserInfoDto {
  lastName: string;
  firstName: string;
  avatarPath?: string;
  dob?: string;
  job: JobType;
  address?: string;
}

export interface UserInfoDto extends AuditedEntityDto<string> {
  lastName?: string;
  firstName?: string;
  avatarPath?: string;
  dob?: string;
  job: JobType;
  address?: string;
  userId?: string;
  userName?: string;
  emailAddress?: string;
}

export interface UserLookupDto extends EntityDto<string> {
  name?: string;
  email?: string;
}
