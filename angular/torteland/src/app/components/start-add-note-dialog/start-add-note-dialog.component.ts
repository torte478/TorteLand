import { Component, ElementRef, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { TextDialogComponent } from '../dialogs/text-dialog/text-dialog.component';

@Component({
  selector: 'app-start-add-note-dialog',
  templateUrl: './start-add-note-dialog.component.html',
  styleUrls: ['./start-add-note-dialog.component.css']
})
export class StartAddNoteDialogComponent {

  public text: string = '';
  public notes: string[] = [''];

  constructor(
    public dialogRef: MatDialogRef<TextDialogComponent>)
    {}

    onMoreClick() {
      this.notes.push('');
    }

    trackByFn(index: any, item: any) {
      return index;
    }
}
