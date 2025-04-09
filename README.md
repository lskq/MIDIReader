A console app that reads the contents of MIDI files using my [MIDILib library](https://github.com/lskq/MIDILib). Made mostly to show that the library works - it's not terribly user friendly. Supports all track events defined in the [standard midi file](https://midi.org/standard-midi-files-specification) and [midi 1.0 core](https://midi.org/midi-1-0-core-specifications) specifications. Other events will still show up; they just won't have bespoke formatting.\
\
Requirements:\
-Dotnet 8\
\
Usage:\
Pass the location of a MIDI file as the first argument to read the file using the built-in paginator. Pass "stdout" as the second argument to print the entirety of the text to stdout.\
\
Controls:\
-Up/Down: Move by line\
-PgUp/PgDn: Move by page\
-Home/End: Move to start/end\
-Esc/Q: Quit\
