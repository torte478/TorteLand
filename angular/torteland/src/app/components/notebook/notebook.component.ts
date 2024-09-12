import { ComponentType } from '@angular/cdk/portal';
import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { EMPTY, Observable, of } from 'rxjs';
import {
  expand,
  filter,
  map,
  mergeMap,
  tap,
  withLatestFrom,
} from 'rxjs/operators';
import { AddNoteDialogResult } from 'src/app/enums/add-note-dialog-result';
import {
  Direction,
  Int32IReadOnlyCollectionQuestionEither,
  Note,
  NotebooksAcrudClient,
  NotebooksClient,
} from 'src/app/services/generated';
import { ContinueAddNoteDialogComponent } from '../continue-add-note-dialog/continue-add-note-dialog.component';
import { ConfirmDialogComponent } from '../dialogs/confirm-dialog/confirm-dialog.component';
import { TextDialogComponent } from '../dialogs/text-dialog/text-dialog.component';
import { StartAddNoteDialogComponent } from '../start-add-note-dialog/start-add-note-dialog.component';

@Component({
  selector: 'app-notebook',
  templateUrl: './notebook.component.html',
  styleUrls: ['./notebook.component.css'],
})
export class NotebookComponent implements OnInit {
  notebookId?: number;
  name?: string;
  notes?: Note[];
  selection: Note[] = [];
  isBusy: boolean = true;
  exactChecked: boolean = false;

  constructor(
    private acrudClient: NotebooksAcrudClient,
    private client: NotebooksClient,
    private route: ActivatedRoute,
    private location: Location,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    const notebookId = Number(this.route.snapshot.paramMap.get('id'));

    this.acrudClient
      .all(undefined, undefined)
      .pipe(
        map((page) => {
          const item = page.items?.filter((x) => x.id === notebookId);
          if (!!item) {
            this.notebookId = notebookId;
            this.name = item[0].value;
          }
        })
      )
      .subscribe((_) => this.reload());
  }

  onBackClick() {
    this.location.back();
  }

  onRenameClick() {
    const selected = this.getSelected();
    if (!selected) return;

    this.RunWithDialog(TextDialogComponent, `Rename ${selected.text}`, (_) =>
      _.pipe(
        mergeMap((name) =>
          this.client.update(this.notebookId, selected.id, name as string)
        )
      )
    );
  }

  onPlusClick() {
    const selected = this.getSelected();
    if (!selected) return;

    this.isBusy = true;
    this.client
      .increment(this.notebookId, selected.id)
      .subscribe((_) => this.reload());
  }

  onMinusClick() {
    const selected = this.getSelected();
    if (!selected) return;

    this.isBusy = true;
    this.client
      .decrement(this.notebookId, selected.id)
      .subscribe((_) => this.reload());
  }

  onDeleteClick() {
    const selected = this.getSelected();
    if (!selected) return;

    this.RunWithDialog(
      ConfirmDialogComponent,
      `Delete '${selected.text}' ?`,
      (_) =>
        _.pipe(
          mergeMap((_) => this.client.delete(this.notebookId, selected.id)),
          tap((_) => (this.selection = []))
        )
    );
  }

  onCreateClick() {
    this.startAdd('Add ', undefined, undefined);
  }

  onCreateAfterClick() {
    const selected = this.getSelected();
    if (!selected) return;

    const caption = this.getAddText(selected, false);
    this.startAdd(caption, selected.id, false);
  }

  onCreateBeforeClick() {
    const selected = this.getSelected();
    if (!selected) return;

    const caption = this.getAddText(selected, true);
    this.startAdd(caption, selected.id, true);
  }

  toPlusText(pluses: number): string {
    let result = '';
    for (let i = 0; i < pluses; ++i) result += '+';

    return result;
  }

  onActualizeClick() {
    const selected = this.getSelected();
    const names = [selected?.text ?? ''];
    of(selected?.id)
      .pipe(
        tap((_) => (this.isBusy = true)),
        mergeMap((noteId) =>
          this.client.startActualize(this.notebookId, noteId)
        ),
        expand((addResult) => {
          if (!addResult.right) return EMPTY;

          return this.continueAdd(names, addResult).pipe();
        })
      )
      .subscribe({ complete: () => this.reload() });
  }

  private getAddText(selected: Note, before: boolean) {
    const exactText = this.exactChecked ? '_Exact_ ' : '';
    const beforeText = before ? 'before' : 'after';

    return `Add ${exactText} ${beforeText} ${selected.text}`;
  }

  private startAdd(
    caption: string,
    origin: number | undefined,
    before: boolean | undefined
  ) {
    const getAdded = this.dialog
      .open(StartAddNoteDialogComponent, { data: { caption: caption } })
      .afterClosed();

    // TODO: refactor
    getAdded
      .pipe(
        filter((names) => !!names),
        map((names: string[]) => names.filter((x) => !!x)),
        filter((names) => !!names.length),
        tap((_) => (this.isBusy = true)),
        mergeMap((names) =>
          this.client.startAdd(
            this.notebookId,
            origin,
            !!before ? Direction._0 : Direction._1,
            this.exactChecked,
            names
          )
        ),
        withLatestFrom(getAdded),
        expand(([addResult, added]) => {
          if (!addResult.right) return EMPTY;

          return this.continueAdd(added, addResult).pipe(
            map((_) => [_, added])
          );
        })
      )
      .subscribe({ complete: () => this.reload() });
  }

  private continueAdd(
    names: string[],
    either: Int32IReadOnlyCollectionQuestionEither
  ) {
    if (!either.right) return of(either);

    return this.dialog
      .open(ContinueAddNoteDialogComponent, {
        data: {
          added: names[0],
          note: either.right.text,
        },
      })
      .afterClosed()
      .pipe(
        mergeMap((res) => {
          if (!res) return EMPTY;

          const isRight =
            res === AddNoteDialogResult.Yes ||
            (res === AddNoteDialogResult.Random && Math.random() >= 0.5);

          return this.client.continueAdd(
            this.notebookId,
            either.right?.id,
            isRight
          );
        })
      );
  }

  private RunWithDialog<TDialog, TDialogResult, TOut>(
    dialogType: ComponentType<TDialog>,
    title: string,
    fn: (x: Observable<TDialogResult>) => Observable<TOut>
  ) {
    const dialogResult = this.dialog
      .open(dialogType, {
        data: { title: title },
      })
      .afterClosed()
      .pipe(
        filter((res) => !!res),
        tap((_) => (this.isBusy = true))
      );

    fn(dialogResult).subscribe((_) => this.reload());
  }

  private getSelected() {
    return this.selection.length === 1 ? this.selection[0] : null;
  }

  private reload() {
    if (this.notebookId === null) return;

    this.selection = [];

    this.client.all(this.notebookId, undefined, undefined).subscribe((page) => {
      this.notes = page.items;
      this.isBusy = false;
    });
  }
}
