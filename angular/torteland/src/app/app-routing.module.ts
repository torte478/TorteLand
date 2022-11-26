import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotebooksAcrudComponent } from './components/notebooks-acrud/notebooks-acrud.component';
import { NotebookComponent } from './components/notebook/notebook.component';

const routes: Routes = [
  { path: '', redirectTo: '/notebooks', pathMatch: 'full' },
  { path: 'notebooks', component: NotebooksAcrudComponent },
  { path: 'notebooks/:id', component: NotebookComponent }
];

@NgModule({
  declarations: [],
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
