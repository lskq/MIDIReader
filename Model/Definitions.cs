using MIDILib.Chunks;

namespace MIDIReader.Model;

public static class Definitions
{
    public static string ChunkTypeToString(IChunk chunk)
    {
        var type = chunk.GetType();

        if (type == typeof(HeaderChunk))
            return "Header Chunk";
        if (type == typeof(TrackChunk))
            return "Track Chunk";
        if (type == typeof(AlienChunk))
            return "Alien Chunk";

        return "Unimplemented Chunk";
    }

    public static string HeaderFormatToString(int format)
    {
        string str = $"{format} ";

        if (format == 0)
            str += "(Single track)";
        if (format == 1)
            str += "(One or more sequential tracks)";
        if (format == 2)
            str += "(One or more independent tracks)";
        else
            str += "(Undefined)";

        return str;
    }

    public static string HeaderDivisionToString(int division)
    {
        if (division < 0x80)
        {
            return $"{division} (Ticks per quarter note)";
        }
        else
        {
            int smpte = (division - 0x80) >> 8;
            int tpf = division - 0x80 - (smpte << 8);

            return $"{smpte} / {tpf} (Negative SMPTE / Ticks per frame)";
        }
    }

    public static string MIDIEventStatusToString(int code)
    {
        int statusByte = code >> 4;

        string midiEventString = statusByte switch
        {
            0x8 => "Note Off",
            0x9 => "Note On",
            0xA => "Polyphonic Key Pressure",
            0xB => "Control Change",
            0xC => "Program Change",
            0xD => "Channel Pressure",
            0xE => "Pitch Bend Change",
            -1 => "Running Status",
            _ => throw new ArgumentException($"0x{code:X} is not a MIDI Channel status byte."),
        };

        return midiEventString + $" (0x{statusByte:X1}n)";
    }

    public static string MetaEventTypeToString(int code)
    {
        string metaEventString = code switch
        {
            0x00 => "Sequence Number",
            0x01 => "Text Event",
            0x02 => "Copyright Notice",
            0x03 => "Sequence/Track Name",
            0x04 => "Instrument Name",
            0x05 => "Lyric",
            0x06 => "Marker",
            0x07 => "Cue Point",
            0x20 => "MIDI Channel Prefix",
            0x2F => "End of Track",
            0x51 => "Set Tempo",
            0x54 => "SMPTE Offset",
            0x58 => "Time Signature",
            0x59 => "Key Signature",
            0x7F => "Sequencer-Specific Meta-Event",
            _ => "Undefined",
        };

        return metaEventString + $" (0x{code:X2})";
    }
}
