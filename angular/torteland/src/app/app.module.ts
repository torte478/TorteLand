import { InjectionToken, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { BASE_URL_TOKEN, NotebooksAcrudClient } from './services/generated';

@NgModule({
  declarations: [
    AppComponent
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
