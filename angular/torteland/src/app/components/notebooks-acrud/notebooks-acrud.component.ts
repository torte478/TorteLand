import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { filter, mergeMap } from 'rxjs';
import { NotebooksAcrudClient, StringUnique } from 'src/app/services/generated';
import { ConfirmDialogComponent } from '../dialogs/confirm-dialog/confirm-dialog.component';
import { TextDialogComponent } from '../dialogs/text-dialog/text-dialog.component';

@Component({
  selector: 'app-notebooks-acrud',
  templateUrl: './notebooks-acrud.component.html',
  styleUrls: ['./notebooks-acrud.component.css']
})
export class NotebooksAcrudComponent implements OnInit {

  notebooks: StringUnique[] | undefined;
  selection: StringUnique[] = [];

  constructor(
    private client: NotebooksAcrudClient,
    public dialog: MatDialog) {}

  ngOnInit(): void {
    this.reload();
  }

  onCreateClick() : void {
    this.dialog.open(TextDialogComponent, {
      data: { title: 'New notebook name' }
      })
      .afterClosed()
      .pipe(
        filter(name => !!name),
        mergeMap(name => this.client.create(name))
      )
      .subscribe(_ => this.reload());
  }

  onRenameClick() {
    if (this.selection.length !== 1)
      return;

    const selected = this.selection[0];

    this.dialog
      .open(TextDialogComponent, {
        data: { title: `Rename '${selected.value}'` }
      })
      .afterClosed()
      .pipe(
        filter(name => !!name),
        mergeMap(name => this.client.rename(selected.id, name))
      )
      .subscribe(_ => this.reload());
  }

  onDeleteClick() {
    if (this.selection.length !== 1)
      return;

    const selected = this.selection[0];

    this.dialog
      .open(ConfirmDialogComponent, {
        data: { title: `Delete '${selected.value}'?` }
      })
      .afterClosed()
      .pipe(
        filter(res => !!res),
        mergeMap(_ => this.client.delete(selected.id))
      )
      .subscribe(_ => this.reload());
  }

  private reload(): void {
    this.client.all(undefined, undefined)
      .subscribe(
        (data) => this.notebooks = data.items);    
  }
}
