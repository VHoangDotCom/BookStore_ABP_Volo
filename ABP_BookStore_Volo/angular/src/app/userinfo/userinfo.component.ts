import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { NgbDateNativeAdapter, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { UserInfoService, UserInfoDto, jobTypeOptions, UserLookupDto } from '@proxy/user-infos';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import * as $ from 'jquery';

@Component({
  selector: 'app-userinfo',
  templateUrl: './userinfo.component.html',
  styleUrls: ['./userinfo.component.scss'],
  providers: [ListService, { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
})
export class UserinfoComponent implements OnInit{
  userInfo = { items: [], totalCount: 0 } as PagedResultDto<UserInfoDto>;

  isModalOpen = false;
  form: FormGroup;
  jobTypes = jobTypeOptions;
  selectedUserInfo = {} as UserInfoDto;
  users$: Observable<UserLookupDto[]>;

  constructor(
    public readonly list: ListService,
    private userInfoService: UserInfoService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
    ){
      this.users$ = userInfoService.getUserLookup().pipe(map((r) => r.items));
    }

  ngOnInit(){
      const userInfoStreamCreator = (query) => this.userInfoService.getList(query);

      this.list.hookToQuery(userInfoStreamCreator).subscribe((response) => {
        this.userInfo = response;
      })
  }

  createUserInfo() {
    this.selectedUserInfo = {} as UserInfoDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  editUserInfo(id: string) {
    this.userInfoService.get(id).subscribe((userInfo) => {
      this.selectedUserInfo = userInfo;
      this.buildForm();
      this.isModalOpen = true;
    });
  }

  buildForm() {
    this.form = this.fb.group({
      lastName: [this.selectedUserInfo.lastName || '', Validators.required],
      firstName: [this.selectedUserInfo.firstName ||'', Validators.required],
      avatarPath: [this.selectedUserInfo.avatarPath || ''],
      job: [this.selectedUserInfo.job || null, Validators.required],
      dob: [ this.selectedUserInfo.dob ? new Date(this.selectedUserInfo.dob) : null,
        Validators.required,],
      address: [this.selectedUserInfo.address || null, Validators.required],
      userId: [this.selectedUserInfo.userId || null, Validators.required],
    });
  }

  save() {
    if (this.form.invalid) {
      return;
    }

    if (this.selectedUserInfo.id) {
      this.userInfoService
        .update(this.selectedUserInfo.id, this.form.value)
        .subscribe(() => {
          this.isModalOpen = false;
          this.form.reset();
          this.list.get();
        });
    } else {
      this.userInfoService.create(this.form.value).subscribe(() => {
        this.isModalOpen = false;
        this.form.reset();
        this.list.get();
      });
    }
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure')
        .subscribe((status) => {
          if (status === Confirmation.Status.confirm) {
            this.userInfoService.delete(id).subscribe(() => this.list.get());
          }
	    });
  }

}
