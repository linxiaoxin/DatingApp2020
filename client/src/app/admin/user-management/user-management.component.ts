import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { EditRolesComponent } from '../edit-roles/edit-roles.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: Partial<User>;
  bsModalRef: BsModalRef;

  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.getUserWithRoles();
  }

  getUserWithRoles(){
    this.adminService.getUsersWithRoles().subscribe(users =>{
      this.users = users;
    });
  }

  openEditRolesModal(user: User) {
    const config = {
      class:"modal-dialog-centered",
      initialState: {
        user
      }
    };
    this.bsModalRef = this.modalService.show(EditRolesComponent, config);
    this.bsModalRef.content.initRoles();
    this.bsModalRef.content.updateSelectedRoles.subscribe(roles => {
      this.adminService.editUserRole(user, [...roles]).subscribe(() => {
        user.roles =[...roles];
      })
    });
  }

}
