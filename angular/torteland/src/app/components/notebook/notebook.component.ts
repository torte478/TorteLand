import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { provideProtractorTestingSupport } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { EMPTY, iif, Observable, of } from 'rxjs';
import { expand, filter, map, mergeMap, switchMap, takeUntil, takeWhile, withLatestFrom } from 'rxjs/operators';
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

  notebookId?: number;
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
    const notebookId = Number(this.route.snapshot.paramMap.get('id'));

    this.acrudClient.all(undefined, undefined)
      .pipe(
        map(page => {
          const item = page.items?.filter(x => x.id === notebookId);
          if (!!item)
          {
            this.notebookId = notebookId;
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
        mergeMap(name => this.client.rename(this.notebookId, selected.key, name))
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
        mergeMap(_ => this.client.delete(this.notebookId, selected.key))
      )
      .subscribe(_ => this.reload());
  }

  onCreateClick() {
    const getAdded = this.dialog
      .open(TextDialogComponent, {
        data: { title: 'Add new note' }
      })
      .afterClosed();

    getAdded
      .pipe(
        mergeMap(name => this.client.startAdd(this.notebookId, [ name ])),
        withLatestFrom(getAdded),
        expand(([addResult, added]) => {
            if (!addResult.right)
              return EMPTY;

            return this.continueAdd(added, addResult)
              .pipe(
                map(_ => [_, added]));
        })
      )
      .subscribe({ complete: () => this.reload() });
  }

  private continueAdd(name: string, either: Int32IReadOnlyCollectionQuestionEither) {
    if (!either.right)
      return of(either);

    return this.dialog
      .open(AddNoteDialogComponent, {
        data: { added: name, note: either.right.text }
      })
      .afterClosed()
      .pipe(
        mergeMap(res => {
          const isRight = res === AddNoteDialogResult.Yes 
                          || (res === AddNoteDialogResult.Random && Math.random() >= 0.5);

          return this.client.continueAdd(this.notebookId, either.right?.id, isRight)
        })
      )
  }

  private getSelected()  {
    return this.selection.length === 1
    ? this.selection[0]
    : null;
  }

  private reload() {
    if (this.notebookId === null)
      return;

    this.client.all(this.notebookId, undefined, undefined)
      .subscribe(page => this.notes = page.items)
  }
}

interface FooBar {
  transactionId?: string,
  isRight: boolean
}
