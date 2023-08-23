import type { CreateUserInfoDto, GetUserInfoListDto, UpdateUserInfoDto, UserInfoDto, UserLookupDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class UserInfoService {
  apiName = 'Default';
  

  create = (input: CreateUserInfoDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CreateUserInfoDto>({
      method: 'POST',
      url: '/api/app/user-info',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/user-info/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserInfoDto>({
      method: 'GET',
      url: `/api/app/user-info/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetUserInfoListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<UserInfoDto>>({
      method: 'GET',
      url: '/api/app/user-info',
      params: { filter: input.filter, jobType: input.jobType, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getUserLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<UserLookupDto>>({
      method: 'GET',
      url: '/api/app/user-info/user-lookup',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateUserInfoDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: `/api/app/user-info/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
