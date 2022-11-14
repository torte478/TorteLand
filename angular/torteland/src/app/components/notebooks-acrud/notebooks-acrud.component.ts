import { Component, OnInit } from '@angular/core';
import { NotebooksAcrudClient, StringUniquePage } from 'src/app/services/generated';

@Component({
  selector: 'app-notebooks-acrud',
  templateUrl: './notebooks-acrud.component.html',
  styleUrls: ['./notebooks-acrud.component.css']
})
export class NotebooksAcrudComponent implements OnInit {

  notebooks: string[] | undefined;

  constructor(private client: NotebooksAcrudClient) {}

  ngOnInit(): void {
    this.client.all(undefined, undefined)
      .subscribe(
        (data) => this.notebooks = data.items?.map(x => x.value!));
  }
}
