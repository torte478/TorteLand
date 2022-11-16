import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { filter, map, mergeMap } from 'rxjs/operators';
import { Int32StringKeyValuePair, NotebooksAcrudClient, NotebooksClient } from 'src/app/services/generated';
import { ConfirmDialogComponent } from '../dialogs/confirm-dialog/confirm-dialog.component';
import { TextDialogComponent } from '../dialogs/text-dialog/text-dialog.component';

@Component({
  selector: 'app-notebook',
  templateUrl: './notebook.component.html',
  styleUrls: ['./notebook.component.css']
})
export class NotebookComponent implements OnInit {

  id?: number;
  name?: string;
  notes?: Int32StringKeyValuePair[];
  selection: Int32StringKeyValuePair[] = [];

  constructor(
    private acrudClient: NotebooksAcrudClient,
    private client: NotebooksClient,
    private route: ActivatedRoute,
    private location: Location,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.acrudClient.all(undefined, undefined)
      .pipe(
        map(page => {
          const item = page.items?.filter(x => x.id === id);
          if (!!item)
          {
            this.id = id;
            this.name = item[0].value;
          }
        }))
      .subscribe(_ => this.reload());
  }

  onBackClick() {
    this.location.back();
  }

  onRenameClick() {
    const selected = this.getSelected();
    if (!selected)
      return;

    this.dialog
      .open(TextDialogComponent, {
        data: { title: `Rename '${selected.value}'` }
      })
      .afterClosed()
      .pipe(
        filter(name => !!name),
        mergeMap(name => this.client.rename(this.id, selected.key, name))
      )
      .subscribe(_ => this.reload());
  }

  onDeleteClick() {
    const selected = this.getSelected();
    if (!selected)
      return;

    this.dialog
      .open(ConfirmDialogComponent, {
        data: { title: `Delete '${selected.value}' ?` }
      })
      .afterClosed()
      .pipe(
        filter(res => !!res),
        mergeMap(_ => this.client.delete(this.id, selected.key))
      )
      .subscribe(_ => this.reload());
  }

  private getSelected()  {
    return this.selection.length === 1
    ? this.selection[0]
    : null;
  }

  private reload() {
    if (this.id === null)
      return;

    this.client.all(this.id, undefined, undefined)
      .subscribe(page => this.notes = page.items)
  }
}
