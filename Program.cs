using MIDILib;
using MIDILib.Chunks;
using MIDILib.Events;

namespace MIDIReader;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            var midi = MIDIParser.ParseFile(args[0]);
            PrintSummary(midi);
        }
    }

    static void PrintSummary(MIDIFile midi)
    {
        int ntrks = 0;
        int format = 0;
        int division = 0;

        int nChunks = 0;

        int nHeaders = 0;
        int nTracks = 0;
        int nAliens = 0;

        int nEvents = 0;
        int nMIDI = 0;
        int nSysex = 0;
        int nMeta = 0;

        foreach (var chunk in midi.Chunks)
        {
            nChunks++;

            var type = chunk.GetType();

            if (type == typeof(HeaderChunk))
            {
                nHeaders++;

                var header = (HeaderChunk)chunk;

                ntrks = header.Ntrks;
                format = header.Format;
                division = header.Division;
            }
            else if (type == typeof(TrackChunk))
            {
                nTracks++;

                var track = (TrackChunk)chunk;

                foreach (var ev in track.Events)
                {
                    nEvents++;

                    type = ev.GetType();

                    if (type == typeof(MIDIEvent)) nMIDI++;
                    else if (type == typeof(SysexEvent)) nSysex++;
                    else if (type == typeof(MetaEvent)) nMeta++;
                }
            }
            else if (type == typeof(AlienChunk))
            {
                nAliens++;
            }
        }

        Console.WriteLine($"""
            Ntrks: {ntrks}
            Format: {format}
            Division: {division}
            
            Chunks: {nChunks}
            └ Headers: {nHeaders}
            └ Tracks: {nTracks}
            └ Aliens: {nAliens}
            
            Events: {nEvents}
            └ MIDI: {nMIDI}
            └ Sysex: {nSysex}
            └ Meta: {nMeta}
        """);
    }
}
