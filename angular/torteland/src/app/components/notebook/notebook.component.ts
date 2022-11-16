import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { filter, map, mergeMap, takeUntil, takeWhile } from 'rxjs/operators';
import { AddNoteDialogResult } from 'src/app/enums/add-note-dialog-result';
import { AddNoteDialogData } from 'src/app/interfaces/add-note-dialog-data';
import { Int32IReadOnlyCollectionQuestionEither, Int32StringKeyValuePair, NotebooksAcrudClient, NotebooksClient } from 'src/app/services/generated';
import { AddNoteDialogComponent } from '../add-note-dialog/add-note-dialog.component';
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

  onCreateClick() {
    this.dialog
      .open(TextDialogComponent, {
        data: { title: 'Add new note' }
      })
      .afterClosed()
      .pipe(
        mergeMap(name => this.Foo(name)))
      .subscribe(_ => this.reload());
  }

  private Foo(name: string) {
    return this.client.startAdd(this.id, [name])
      .pipe(
        filter(_ => !!_.right),
        mergeMap(_ => this.Bar({ added: name, note: _.right?.text}, _.right?.id)),
        mergeMap(_ => this.client.continueAdd(this.id, _.transactionId, _.isRight)),
      );
  }

  private Bar(data: AddNoteDialogData, transactionId?: string): Observable<FooBar> {
    return this.dialog
      .open(AddNoteDialogComponent, {data: data})
      .afterClosed()
      .pipe(
        map(res => { return {
          transactionId: transactionId,
          isRight: res === AddNoteDialogResult.Yes 
                   || (res === AddNoteDialogResult.Random && Math.random() >= 0.5)
        }})
      );
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

interface FooBar {
  transactionId?: string,
  isRight: boolean
}
