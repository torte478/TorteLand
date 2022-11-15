import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { filter, map, mergeMap } from 'rxjs';
import { NotebooksAcrudClient } from 'src/app/services/generated';
import { TextDialogComponent } from '../dialogs/text-dialog/text-dialog.component';

@Component({
  selector: 'app-notebooks-acrud',
  templateUrl: './notebooks-acrud.component.html',
  styleUrls: ['./notebooks-acrud.component.css']
})
export class NotebooksAcrudComponent implements OnInit {

  notebooks: string[] | undefined;

  constructor(
    private client: NotebooksAcrudClient,
    public dialog: MatDialog) {}

  ngOnInit(): void {
    this.reload();
  }

  onCreateClick() : void {
    const createDialog = this.dialog.open(TextDialogComponent, {
      data: { title: 'New notebook name' }
    });

    createDialog
      .afterClosed()
      .pipe(
        filter((name) => !!name),
        mergeMap((name) => this.client.create(name))
      )
      .subscribe(_ => this.reload());
  }

  private reload(): void {
    this.client.all(undefined, undefined)
      .subscribe(
        (data) => this.notebooks = data.items?.map(x => x.value!));    
  }
}
