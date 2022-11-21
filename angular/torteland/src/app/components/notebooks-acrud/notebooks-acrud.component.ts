import { ComponentType } from '@angular/cdk/portal';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { filter, mergeMap, Observable, Operator, tap } from 'rxjs';
import { NotebooksAcrudClient, StringUnique } from 'src/app/services/generated';
import { ConfirmDialogComponent } from '../dialogs/confirm-dialog/confirm-dialog.component';
import { TextDialogComponent } from '../dialogs/text-dialog/text-dialog.component';

@Component({
  selector: 'app-notebooks-acrud',
  templateUrl: './notebooks-acrud.component.html',
  styleUrls: ['./notebooks-acrud.component.css']
})
export class NotebooksAcrudComponent implements OnInit {

  notebooks: StringUnique[] = [];
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
    this.RunWithDialog(
      TextDialogComponent,
      'New notebook name',
      _ => _.pipe(
        mergeMap(name => this.client.create(name as string))
    ));
  }

  onRenameClick() {
    const selected = this.getSelected();
    if (!selected)
      return;

    this.RunWithDialog(
      TextDialogComponent,
      `Rename '${selected.value}'`,
      _ => _.pipe(
        mergeMap(name => this.client.update(selected.id, name as string))
    ));
  }

  onDeleteClick() {
    const selected = this.getSelected();
    if (!selected)
      return;

    this.RunWithDialog(
      ConfirmDialogComponent, 
      `Delete '${selected.value}' ?`, 
      _ => _.pipe(
        mergeMap(_ => this.client.delete(selected.id)),
        tap(_ => this.selection = [])
    ));
  }

  onOpenClick() {
    const selected = this.getSelected();
    if (!selected)
      return;

    this.route.navigateByUrl(`notebooks/${selected.id}`);
  }

  private RunWithDialog<TDialog, TDialogResult, TOut>(
    dialogType: ComponentType<TDialog>, 
    title: string, 
    fn: (x: Observable<TDialogResult>) => Observable<TOut>) {

    const dialogResult = this.dialog
      .open(dialogType, {
        data: { title: title }
      })
      .afterClosed()
      .pipe(
        filter(res => !!res),
        tap(_ => this.isBusy = true)
      );

    fn(dialogResult)
      .subscribe(_ => this.reload());
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
          if (!!data.items)
            this.notebooks = data.items;

          this.isBusy = false;
        });    
  }
}
