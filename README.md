A console app that reads the contents of MIDI files using my [MIDILib library](https://github.com/lskq/MIDILib). Made mostly to show that the library works - it's not terribly user friendly.\
\
Requires Dotnet 8.x.\
\
Usage:\
Pass the location of a MIDI file as the first argument to read the file. Use the arrow keys, page up/down, and home/end to move through the text line by line, page by page, and beginning to end. Q or escape exits. Passing "stdout" as the second argument prints the entirety of the text to stdout.
