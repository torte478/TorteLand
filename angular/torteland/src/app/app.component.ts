import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { NotebooksAcrudClient, StringUniquePage } from './services/generated';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'TorteLand';

  posts: StringUniquePage | undefined;

  constructor(private client: NotebooksAcrudClient, private http : HttpClient) 
  {
  }

  getPosts() {
    this.client.all(1, 1)
      .subscribe((data) => this.posts = data);
  }
}
