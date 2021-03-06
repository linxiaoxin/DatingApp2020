import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminGuard } from 'src/_guard/admin.guard';
import { AuthGuard } from 'src/_guard/auth.guard';
import { PreventUnsavedChangesGuard } from 'src/_guard/prevent-unsaved-changes.guard';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MembersDetailsComponent } from './members/members-details/members-details.component';
import { MembersListComponent } from './members/members-list/members-list.component';
import { MessagesComponent } from './messages/messages.component';
import { TestErrorsComponent } from './test-errors/test-errors.component';
import { MembersDetailsResolver } from './_resolvers/members-details.resolver';

const routes: Routes = [
  {path: '', component: HomeComponent },
  {path: '', 
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [ 
    {path: 'members', component: MembersListComponent},
    {path: 'members/:username', component: MembersDetailsComponent, resolve:{member: MembersDetailsResolver}},
    {path: 'member/edit', component: MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard]},
    {path: 'lists', component:ListsComponent},
    {path: 'messages', component: MessagesComponent},
    {path: 'admin', component: AdminPanelComponent, canActivate:[AdminGuard]}]
  },
  {path: 'server-error', component:ServerErrorComponent},
  {path:'not-found', component: NotFoundComponent},
  {path: 'test-errors', component: TestErrorsComponent},
  {path: '**', component: NotFoundComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
