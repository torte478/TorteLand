import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './components/app.component';
import { BASE_URL_TOKEN, NotebooksAcrudClient } from './services/generated';
import { NotebooksAcrudComponent } from './components/notebooks-acrud/notebooks-acrud.component';

@NgModule({
  declarations: [
    AppComponent,
    NotebooksAcrudComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule
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
