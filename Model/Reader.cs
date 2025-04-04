using MIDILib;
using MIDILib.Chunks;
using MIDILib.Events;

namespace MIDIReader.Model;

public static class Reader
{
    public static void PrintSummary(MIDIFile midi)
    {
        int format = 0;
        int ntrks = 0;
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

                format = header.Format;
                ntrks = header.Ntrks;
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
            Format: {Definitions.GetFormat(format)}
            Ntrks: {ntrks}
            Division: {Definitions.GetDivision(division)}
            
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
