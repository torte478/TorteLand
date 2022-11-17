import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatButtonModule} from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';

import { AppComponent } from './components/app.component';
import { BASE_URL_TOKEN, NotebooksAcrudClient, NotebooksClient } from './services/generated';
import { NotebooksAcrudComponent } from './components/notebooks-acrud/notebooks-acrud.component';
import { TextDialogComponent } from './components/dialogs/text-dialog/text-dialog.component';
import { ConfirmDialogComponent } from './components/dialogs/confirm-dialog/confirm-dialog.component';
import { NotebookComponent } from './components/notebook/notebook.component';
import { AppRoutingModule } from './app-routing.module';
import { ContinueAddNoteDialogComponent } from './components/add-note-dialog/continue-add-note-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    NotebooksAcrudComponent,
    TextDialogComponent,
    ConfirmDialogComponent,
    NotebookComponent,
    ContinueAddNoteDialogComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatListModule,
    AppRoutingModule
  ],
  providers: [
    {
      provide: BASE_URL_TOKEN,
      useValue: 'api'
    },
    NotebooksAcrudClient,
    NotebooksClient
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
