import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { filter, map, Observable, of, range } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';

// const source$: Observable<number> = range(0, 10);

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  //TODO: ha beinjektaljuk a memberservice-t kiurul a memberlist component megnezni miert.

  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) {}

  ngOnInit(): void {
    
  }

  login(){
    this.accountService.login(this.model).subscribe({
        next: () => {
          this.router.navigateByUrl('/members')
          this.model = {}; //igy kilogolaskor ures lesz a username and password textbox
        }

    });
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
// source$.pipe(
//   map(x=>x * 2),
//   filter(x=>x %3 === 0)
//       ).subscribe(x=> console.log(x));

  }
}
