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
import { BASE_URL_TOKEN, NotebooksAcrudClient } from './services/generated';
import { NotebooksAcrudComponent } from './components/notebooks-acrud/notebooks-acrud.component';
import { TextDialogComponent } from './components/dialogs/text-dialog/text-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    NotebooksAcrudComponent,
    TextDialogComponent
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
    MatListModule
  ],
  providers: [
    {
      provide: BASE_URL_TOKEN,
      useValue: 'api'
    },
    NotebooksAcrudClient
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
