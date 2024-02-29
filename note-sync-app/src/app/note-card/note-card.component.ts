import { Component, OnInit } from '@angular/core';
import { NoteService } from '../services/note.service';
import { Note } from '../models/Note';
import { NgxMasonryOptions } from 'ngx-masonry';

@Component({
  selector: 'app-note-card',
  templateUrl: './note-card.component.html',
  styleUrls: ['./note-card.component.scss'],
})
export class NoteCardComponent implements OnInit {
  isBgColorCardVisible = false;
  masonryOptions: NgxMasonryOptions = {
    horizontalOrder: true,
  };
  allNotes: Note[] = [];

  constructor(private noteService: NoteService) {}

  ngOnInit(): void {
    this.allNotes = this.noteService.getAllNotes();
  }

  onDelete(noteId: string) {
    this.noteService.deleteNote(noteId);
  }
}
