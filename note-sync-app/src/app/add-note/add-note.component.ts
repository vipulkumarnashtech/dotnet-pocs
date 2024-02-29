import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { NoteService } from '../services/note.service';
import { Note } from '../models/Note';
import { v4 as uuidv4 } from 'uuid';

@Component({
  selector: 'app-add-note',
  templateUrl: './add-note.component.html',
  styleUrls: ['./add-note.component.scss'],
})
export class AddNoteComponent implements OnInit {
  showAddNote: boolean = false;
  noteForm: FormGroup = this.formBuilder.group({
    title: new FormControl(''),
    content: new FormControl(''),
  });

  constructor(
    private formBuilder: FormBuilder,
    private noteService: NoteService
  ) {}

  public get titleControl(): FormControl {
    return this.noteForm.get('title') as FormControl;
  }

  public get contentControl(): FormControl {
    return this.noteForm.get('content') as FormControl;
  }

  ngOnInit(): void {}

  onClickedInside() {
    this.showAddNote = true;
  }

  onClickedOutside(isSubmit: boolean = true) {
    this.showAddNote = false;
    if (
      (this.contentControl.value.trim() !== '' ||
        this.titleControl.value.trim() !== '') &&
      isSubmit === true
    ) {
      const note: Note = {
        id: uuidv4(),
        title: this.titleControl.value.trim(),
        content: this.contentControl.value.trim(),
        backgroundColor: 'white',
      };
      this.noteService.createNote(note);
    }
    this.contentControl.setValue('');
    this.titleControl.setValue('');
  }
}
