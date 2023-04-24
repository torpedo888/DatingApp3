import { Component, OnInit } from '@angular/core';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { MemberEditComponent } from '../member-edit/member-edit.component';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams;
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}]

  constructor(private memberservice: MembersService) {
    this.userParams = memberservice.getUserParams();
  }

  ngOnInit(): void {
    //this.members$ = this.memberservice.getMembers();
    this.loadMembers();
  }

  testLoadMembers(){
    this.memberservice.getMembers(this.userParams).subscribe({
      next: response => {
        //response.
      }
    })
  }

  loadMembers() {
    if(this.userParams){
      
      //beallitjuk az aktualisan kivalasztott user parameterkre
      this.memberservice.setUserParams(this.userParams);

      this.memberservice.getMembers(this.userParams).subscribe({
        next: response => {
          if(response.result && response.pagination){
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      })
    }
    
  }

  resetFilters() {
    this.userParams = this.memberservice.resetUserParams();
    this.loadMembers();
  }

  pagedChanged(event: any) {
    if(this.userParams.pageNumber!== event.page){
      this.userParams.pageNumber = event.page;
      this.loadMembers();
    }
  }

  // loadMembers(){
  //   this.memberservice.getMembers().subscribe({
  //     next: members => this.members = members
  //   })
  // }
}
