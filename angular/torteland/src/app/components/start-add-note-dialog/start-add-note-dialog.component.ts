import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TextDialogComponent } from '../dialogs/text-dialog/text-dialog.component';

@Component({
  selector: 'app-start-add-note-dialog',
  templateUrl: './start-add-note-dialog.component.html',
  styleUrls: ['./start-add-note-dialog.component.css']
})
export class StartAddNoteDialogComponent {

  public caption: string = '';
  public notes: string[] = [''];

  constructor(
    public dialogRef: MatDialogRef<TextDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { caption: string}){
      this.caption = data.caption;
    }

    onMoreClick() {
      this.notes.push('');
    }

    trackByFn(index: any) {
      return index;
    }
}
