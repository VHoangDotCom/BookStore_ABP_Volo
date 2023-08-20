import { mapEnumToOptions } from '@abp/ng.core';

export enum JobType {
  None = 0,
  Teacher = 1,
  Student = 2,
  Developer = 3,
}

export const jobTypeOptions = mapEnumToOptions(JobType);
