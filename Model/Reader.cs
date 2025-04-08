using MIDILib;
using MIDILib.Chunks;
using MIDILib.Events;
using System.Text;

namespace MIDIReader.Model;

public static class Reader
{
    public static void Read(MIDIFile midi)
    {
        string[] lines = MIDIToStringArray(midi);

        Console.CursorVisible = false;

        int linePos = 0;
        int length = lines.Length;
        bool running = true;

        do
        {
            Console.Clear();

            var key = ConsoleKey.None;

            int height = Console.WindowHeight - 1;

            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < height; i++)
            {
                if (linePos + i < length)
                    Console.WriteLine(lines[linePos + i]);
            }

            while (true)
            {
                if (Console.KeyAvailable == true)
                {
                    key = Console.ReadKey(true).Key;
                }

                if (key == ConsoleKey.UpArrow)
                {
                    if (linePos > 0) linePos--;
                    break;
                }
                if (key == ConsoleKey.DownArrow)
                {
                    if (linePos + height < length) linePos++;
                    break;
                }
                if (key == ConsoleKey.PageUp)
                {
                    int pageUp = linePos - height;

                    linePos = pageUp < 0 ? 0 : pageUp;
                    break;
                }
                if (key == ConsoleKey.PageDown)
                {
                    int pageDown = linePos + height;
                    int endOfFile = length - height;
                    linePos = pageDown > endOfFile ? endOfFile : pageDown;
                    break;
                }
                if (key == ConsoleKey.Home)
                {
                    linePos = 0;
                    break;
                }
                if (key == ConsoleKey.End)
                {
                    linePos = length - height;
                    break;
                }
                if (key == ConsoleKey.Escape || key == ConsoleKey.Q)
                {
                    running = false;
                    break;
                }
            }
        } while (running);

        Console.Clear();
        Console.CursorVisible = true;
    }

    public static void Print(MIDIFile midi)
    {
        foreach (var line in MIDIToStringArray(midi))
        {
            Console.WriteLine(line);
        }
    }

    public static string[] MIDIToStringArray(MIDIFile midi)
    {
        string[] array = ["START OF TRANSMISSION", "", .. SummaryToStringArray(midi), ""];

        for (int i = 0; i < midi.Chunks.Length; i++)
        {
            var chunk = midi.Chunks[i];

            array = [.. array, $"{i} {Definitions.ChunkTypeToString(chunk)}", ""];


            if (chunk.GetType() == typeof(HeaderChunk))
            {
                var headerChunk = (HeaderChunk)chunk;

                array = [.. array, .. HeaderChunkToStringArray(headerChunk), ""];
            }
            else if (chunk.GetType() == typeof(TrackChunk))
            {
                var trackChunk = (TrackChunk)chunk;

                for (int j = 0; j < trackChunk.Events.Length; j++)
                {
                    var ev = trackChunk.Events[j];

                    string eventCount = $"{i}-{j} ";

                    if (ev.GetType() == typeof(MIDIEvent))
                    {
                        array = [.. array, eventCount + "MIDI", .. MIDIEventToStringArray((MIDIEvent)ev)];
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

        array = [.. array, "END OF TRANSMISSION"];

        return array;
    }

    public static string[] HeaderChunkToStringArray(HeaderChunk header)
    {
        return
        [
            $"Format: {Definitions.HeaderFormatToString(header.Format)}",
            $"Ntrks: {header.Ntrks}",
            $"Division: {Definitions.HeaderDivisionToString(header.Division)}"
        ];
    }

    public static string[] MIDIEventToStringArray(MIDIEvent midi)
    {
        int statusHalfbyte = midi.StatusByte >> 4;
        bool runningStatus = midi.RunningStatus;

        string statusString = Definitions.MIDIEventStatusToString(statusHalfbyte, runningStatus);

        string channel = $"{midi.StatusByte & 0xF}";

        string message = "";
        if (statusHalfbyte == 0x8 || statusHalfbyte == 0x9)
        {
            message += Definitions.MIDINoteToString(midi.DataBytes[0]) + " " + midi.DataBytes[1];
        }
        else
        {
            message = Definitions.ByteArrayToHexString(midi.DataBytes);
        }

        return
        [
            $"Status: {statusString}",
            $"Channel: {channel}",
            $"Message: {message}",
        ];
    }

    public static string[] MetaEventToStringArray(MetaEvent meta)
    {
        int typeByte = meta.TypeByte;
        byte[] dataBytes = meta.DataBytes;

        string typeString = Definitions.MetaEventTypeToString(typeByte);
        int length = meta.Length;

        string message = typeByte switch
        {
            0x00 => $"{Definitions.ByteArrayToInt(dataBytes)}",
            0x20 => $"{Definitions.ByteArrayToInt(dataBytes)}",
            0x51 => $"{Definitions.ByteArrayToInt(dataBytes)} microseconds per quarter note",
            0x54 => Definitions.ByteArrayToHexString(dataBytes),
            0x58 => Definitions.MetaTimeSignatureToString(dataBytes),
            0x59 => Definitions.MetaKeySignatureToString(dataBytes),
            0x7F => Definitions.ByteArrayToHexString(dataBytes),
            _ => Encoding.ASCII.GetString(dataBytes),
        };

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
        string message = Definitions.ByteArrayToHexString(sysex.DataBytes);

        return
        [
            $"Length: {length}",
            $"Message: {message}"
        ];
    }

    public static string[] SummaryToStringArray(MIDIFile midi)
    {
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
            "Summary",
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
