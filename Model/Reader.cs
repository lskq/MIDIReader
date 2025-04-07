using MIDILib;
using MIDILib.Chunks;
using MIDILib.Events;
using System.Text;

namespace MIDIReader.Model;

public static class Reader
{
    public static void PrintMIDI(MIDIFile midi)
    {
        for (int i = 0; i < midi.Chunks.Length; i++)
        {
            var chunk = midi.Chunks[i];

            Console.WriteLine($"{i} {Definitions.ChunkTypeToString(chunk)}\n");

            if (chunk.GetType() == typeof(TrackChunk))
            {
                var trackChunk = (TrackChunk)chunk;

                for (int j = 0; j < trackChunk.Events.Length; j++)
                {
                    var ev = trackChunk.Events[j];

                    string eventCount = $"\tEvent #{j} - ";

                    if (ev.GetType() == typeof(MIDIEvent))
                    {
                        Console.WriteLine(eventCount + "MIDI Channel");
                        PrintMIDIEvent((MIDIEvent)ev);
                    }
                    else if (ev.GetType() == typeof(MetaEvent))
                    {

                        Console.WriteLine(eventCount + "Meta");
                        PrintMetaEvent((MetaEvent)ev);
                    }
                    else if (ev.GetType() == typeof(SysexEvent))
                    {
                        Console.WriteLine(eventCount + "Sysex");
                        PrintSysexEvent((SysexEvent)ev);
                    }
                    else
                    {
                        Console.WriteLine(eventCount + "Unrecognized");
                    }

                    Console.WriteLine();
                }
            }
        }
    }

    public static void PrintMIDIEvent(MIDIEvent midi)
    {
        string statusString = Definitions.MIDIEventStatusToString(midi.StatusByte);

        string message = "";
        foreach (var b in midi.DataBytes)
        {
            message += $"0b{b:B8} ";
        }

        Console.WriteLine($"""
                Status: {statusString}
                Channel: {midi.StatusByte & 0xF:X1}
                Message: {message}
        """);
    }

    public static void PrintMetaEvent(MetaEvent meta)
    {
        string typeString = Definitions.MetaEventTypeToString(meta.TypeByte);
        int length = meta.Length;

        string message = Encoding.ASCII.GetString(meta.DataBytes);

        Console.WriteLine($"""
                Type: {typeString}
                Length: {length}
                Message: {message}
        """);
    }

    public static void PrintSysexEvent(SysexEvent sysex)
    {
        int length = sysex.Length;
        string message = Encoding.ASCII.GetString(sysex.DataBytes);

        Console.WriteLine($"""
                Length: {length}
                Message: {message}
        """);
    }

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
        Format: {Definitions.HeaderFormatToString(format)}
        Ntrks: {ntrks}
        Division: {Definitions.HeaderDivisionToString(division)}
        
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
