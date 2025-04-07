using MIDILib;
using MIDILib.Chunks;
using MIDILib.Events;
using System.Text;

namespace MIDIReader.Model;

public static class Reader
{
    public static void Print(MIDIFile midi)
    {
        foreach (var line in MIDIToStringArray(midi))
        {
            Console.WriteLine(line);
        }
    }

    public static String[] MIDIToStringArray(MIDIFile midi)
    {
        string[] array = [];

        for (int i = 0; i < midi.Chunks.Length; i++)
        {
            var chunk = midi.Chunks[i];

            array = [.. array, $"{i} {Definitions.ChunkTypeToString(chunk)}", ""];

            if (chunk.GetType() == typeof(TrackChunk))
            {
                var trackChunk = (TrackChunk)chunk;

                for (int j = 0; j < trackChunk.Events.Length; j++)
                {
                    var ev = trackChunk.Events[j];

                    string eventCount = $"\tEvent #{j} - ";

                    if (ev.GetType() == typeof(MIDIEvent))
                    {
                        array = [.. array, eventCount + "MIDI Channel", .. MIDIEventToStringArray((MIDIEvent)ev)];
                    }
                    else if (ev.GetType() == typeof(MetaEvent))
                    {

                        array = [.. array, eventCount + "Meta", .. MetaEventToStringArray((MetaEvent)ev)];
                    }
                    else if (ev.GetType() == typeof(SysexEvent))
                    {
                        array = [.. array, eventCount + "Sysex", .. SysexEventToStringArray((SysexEvent)ev)];
                    }
                    else
                    {
                        array = [.. array, eventCount + "Unrecognized"];
                    }

                    array = [.. array, ""];
                }
            }
        }

        return array;
    }

    public static string[] MIDIEventToStringArray(MIDIEvent midi)
    {
        string statusString = Definitions.MIDIEventStatusToString(midi.StatusByte);

        string message = "";
        foreach (var b in midi.DataBytes)
        {
            message += $"0b{b:B8} ";
        }

        return
        [
            $"Status: {statusString}",
            $"Channel: {midi.StatusByte & 0xF:X1}",
            $"Message: {message}",
        ];
    }

    public static string[] MetaEventToStringArray(MetaEvent meta)
    {
        string typeString = Definitions.MetaEventTypeToString(meta.TypeByte);
        int length = meta.Length;

        string message = Encoding.ASCII.GetString(meta.DataBytes);

        return
        [
            $"Type: {typeString}",
            $"Length: {length}",
            $"Message: {message}",
        ];
    }

    public static string[] SysexEventToStringArray(SysexEvent sysex)
    {
        int length = sysex.Length;
        string message = Encoding.ASCII.GetString(sysex.DataBytes);

        return
        [
            $"Length: {length}",
            $"Message: {message}"
        ];
    }

    public static String[] SummaryToStringArray(MIDIFile midi)
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

        return
        [
            $"Format: {Definitions.HeaderFormatToString(format)}",
            $"Ntrks: {ntrks}",
            $"Division: {Definitions.HeaderDivisionToString(division)}",
            "",
            $"Chunks: {nChunks}",
            $"└ Headers: {nHeaders}",
            $"└ Tracks: {nTracks}",
            $"└ Aliens: {nAliens}",
            "",
            $"Events: {nEvents}",
            $"└ MIDI: {nMIDI}",
            $"└ Sysex: {nSysex}",
            $"└ Meta: {nMeta}",
        ];
    }
}
