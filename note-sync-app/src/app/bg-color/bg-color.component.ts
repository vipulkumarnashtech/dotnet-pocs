import { Component } from '@angular/core';
import { BgColors } from '../models/BgColors';
import { NoteService } from '../services/note.service';

@Component({
  selector: 'app-bg-color',
  templateUrl: './bg-color.component.html',
  styleUrls: ['./bg-color.component.scss'],
})
export class BgColorComponent {
  colors = BgColors;

  constructor(private noteService: NoteService) {}

  changeBgColor(color: string) {}
}
