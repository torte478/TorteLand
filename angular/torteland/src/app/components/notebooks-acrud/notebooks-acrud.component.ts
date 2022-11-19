import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { filter, mergeMap, tap } from 'rxjs';
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
  isBusy: Boolean = true;

  constructor(
    private client: NotebooksAcrudClient,
    private route: Router,
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
        tap(_ => this.isBusy = true),
        mergeMap(name => this.client.create(name))
      )
      .subscribe(_ => this.reload());
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
        tap(_ => this.isBusy = true),
        mergeMap(name => this.client.rename(selected.id, name))
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
        tap(_ => this.isBusy = true),
        mergeMap(_ => this.client.delete(selected.id))
      )
      .subscribe(_ => this.reload());
  }

  onOpenClick() {
    const selected = this.getSelected();
    if (!selected)
      return;

    this.route.navigateByUrl(`notebooks/${selected.id}`);
  }

  private getSelected()  {
    return this.selection.length === 1
    ? this.selection[0]
    : null;
  }

  private reload(): void {
    this.client.all(undefined, undefined)
      .subscribe(
        (data) => {
          this.notebooks = data.items;
          this.isBusy = false;
        });    
  }
}
