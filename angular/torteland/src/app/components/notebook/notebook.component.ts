import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { filter, map, mergeMap } from 'rxjs/operators';
import { Int32StringKeyValuePair, NotebooksAcrudClient, NotebooksClient } from 'src/app/services/generated';

@Component({
  selector: 'app-notebook',
  templateUrl: './notebook.component.html',
  styleUrls: ['./notebook.component.css']
})
export class NotebookComponent implements OnInit {

  name?: string;
  notes?: Int32StringKeyValuePair[];
  selection: Int32StringKeyValuePair[] = [];

  constructor(
    private acrudClient: NotebooksAcrudClient,
    private client: NotebooksClient,
    private route: ActivatedRoute,
    private location: Location
  ) { }

  ngOnInit(): void {
    this.reload();
  }

  onBackClick() {
    this.location.back();
  }

  private reload() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.acrudClient.all(undefined, undefined)
      .pipe(
        map(page => {
          const item = page.items?.filter(x => x.id === id);
          if (!!item)
            this.name = item[0].value;
        }),
        filter(_ => !!this.name),
        mergeMap(_ => this.client.all(id, undefined, undefined))
      )
      .subscribe(page => this.notes = page.items);
  }
}
