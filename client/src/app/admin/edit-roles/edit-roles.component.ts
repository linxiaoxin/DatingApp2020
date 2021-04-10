import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-edit-roles',
  templateUrl: './edit-roles.component.html',
  styleUrls: ['./edit-roles.component.css']
})
export class EditRolesComponent implements OnInit {
  @Input() updateSelectedRoles = new EventEmitter();
  user: User;
  roles: any[] =[];
  availableRole= ["Admin", "Moderator","Member"];
  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  initRoles(){
    this.availableRole.forEach(role => {
      if(this.user.roles.includes(role)){
        this.roles.push( {name: role, value: role, checked: true })
      }else
      {
        this.roles.push( {name: role, value: role, checked: false })
      }
    });
  }

  updateRoles(){
    const roles = this.roles.filter(r => r.checked === true).map(r => r.value);
    this.updateSelectedRoles.emit(roles);
    this.bsModalRef.hide();
  }
}
