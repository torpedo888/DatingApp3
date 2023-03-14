import { Component, OnInit } from '@angular/core';
import { filter, map, Observable, of, range } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

// const source$: Observable<number> = range(0, 10);

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public accountService: AccountService) {}

  ngOnInit(): void {
    
  }

  login(){
    this.accountService.login(this.model).subscribe({
        next: response => {
          console.log(response);
        },
        error: error => console.log(error)
    });
  }

  logout(){
    this.accountService.logout();
// source$.pipe(
//   map(x=>x * 2),
//   filter(x=>x %3 === 0)
//       ).subscribe(x=> console.log(x));

  }
}
