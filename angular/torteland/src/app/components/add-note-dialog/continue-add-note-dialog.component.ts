import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AddNoteDialogResult } from 'src/app/enums/add-note-dialog-result';
import { AddNoteDialogData } from '../../interfaces/add-note-dialog-data';

@Component({
  selector: 'continue-add-note-dialog.component',
  templateUrl: './continue-add-note-dialog.component.html',
  styleUrls: ['./continue-add-note-dialog.component.css']
})
export class ContinueAddNoteDialogComponent {

  constructor(
    public dialogRef: MatDialogRef<ContinueAddNoteDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public input: AddNoteDialogData
  ) {}

  onYesClick() {
     this.dialogRef.close(AddNoteDialogResult.Yes);
  }

  onNoClick() {
    this.dialogRef.close(AddNoteDialogResult.No);
 }

  onRandomClick() {
    this.dialogRef.close(AddNoteDialogResult.Random);
  }
}
