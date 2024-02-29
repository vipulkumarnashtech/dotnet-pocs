import { Injectable } from '@angular/core';
import { Note } from '../models/Note';
import { v4 as uuidv4 } from 'uuid';

@Injectable({
  providedIn: 'root',
})
export class NoteService {
  noteStore: Note[] = [
    {
      id: uuidv4(),
      title: 'test',
      content: `1Lorem ipsum dolor sit, amet consectetur adipisicing elit. Ab odio itaque
      nesciunt ratione est eligendi natus adipisci consectetur? Molestias
      amet aut, harum inventore dolores debitis odio ex dolore nulla explicabo `,
      backgroundColor: 'white',
    },
    {
      id: uuidv4(),
      title: 'test',
      content: '2fkjdskfjiajefijd djk jfkjdf kdjfkj fjksjffewf ef ',
      backgroundColor: 'white',
    },
    {
      id: uuidv4(),
      title: 'test',
      content: '3dfjkajdfjaifj dsfkdkfjjf dkfkdfjj fd jfdkjfjdi jfidj kjdj ',
      backgroundColor: 'white',
    },
    {
      id: uuidv4(),
      title: 'test',
      content: '4dfjkajdfjaifj',
      backgroundColor: 'white',
    },
    {
      id: uuidv4(),
      title: 'test',
      content: `5Eligendi similique ad  optio impedit aperiam libero praesentium 
      earum id nobis consequatur`,
      backgroundColor: 'white',
    },
    {
      id: uuidv4(),
      title: 'test',
      content: '6dfjk fdasfasf asd df dsajdfjaifj',
      backgroundColor: 'lightcoral',
    },
    {
      id: uuidv4(),
      title: 'test',
      content: '7dfjfdfa fadsf df df dfdsfasdf df dfdf dasf dfkajdfjaifj',
      backgroundColor: 'lightcoral',
    },
    {
      id: uuidv4(),
      title: 'test',
      content: '8dfjkajdfdf dsfs fdsfdf df df df df djaifj',
      backgroundColor: 'lightcoral',
    },
    {
      id: uuidv4(),
      title: 'test',
      content: '9fdfasdf fdafd fd fdf dfda fasf df dsf dfdfdfds fd fd ',
      backgroundColor: 'lightcoral',
    },
    {
      id: uuidv4(),
      title: 'test',
      content: '10dfjkajdfd fdsf dsaf df df df dafaf djaifj',
      backgroundColor: 'lightcoral',
    },
  ];

  constructor() {}

  createNote(note: Note) {
    this.noteStore.push(note);
  }

  getAllNotes() {
    return this.noteStore;
  }

  deleteNote(id: string) {
    const indexOfDeletedNote = this.noteStore.findIndex(
      (note) => note.id === id
    );
    if (indexOfDeletedNote > -1) {
      this.noteStore.splice(indexOfDeletedNote, 1);
    }
  }

  updateColor(id: string, color: string) {
    const indexOfNote = this.noteStore.findIndex((note) => note.id === id);
    if (indexOfNote > -1) {
      this.noteStore[indexOfNote].backgroundColor = color;
    }
  }
}
