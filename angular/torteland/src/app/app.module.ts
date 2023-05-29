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
import { MatCheckboxModule } from '@angular/material/checkbox';

import { AppComponent } from './components/app.component';
import { BASE_URL_TOKEN, NotebooksAcrudClient, NotebooksClient } from './services/generated';
import { NotebooksAcrudComponent } from './components/notebooks-acrud/notebooks-acrud.component';
import { TextDialogComponent } from './components/dialogs/text-dialog/text-dialog.component';
import { ConfirmDialogComponent } from './components/dialogs/confirm-dialog/confirm-dialog.component';
import { NotebookComponent } from './components/notebook/notebook.component';
import { AppRoutingModule } from './app-routing.module';
import { ContinueAddNoteDialogComponent } from './components/continue-add-note-dialog/continue-add-note-dialog.component';
import { StartAddNoteDialogComponent } from './components/start-add-note-dialog/start-add-note-dialog.component';

import configUrl from '../assets/config.json';
import { ScrollingModule } from '@angular/cdk/scrolling';

@NgModule({
  declarations: [
    AppComponent,
    NotebooksAcrudComponent,
    TextDialogComponent,
    ConfirmDialogComponent,
    NotebookComponent,
    ContinueAddNoteDialogComponent,
    StartAddNoteDialogComponent
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
    AppRoutingModule,
    ScrollingModule,
    MatCheckboxModule
  ],
  providers: [
    {
      provide: BASE_URL_TOKEN,
      useValue: configUrl.apiServer.url
    },
    NotebooksAcrudClient,
    NotebooksClient
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
