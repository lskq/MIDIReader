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
        else if (format == 1)
            str += "(One or more sequential tracks)";
        else if (format == 2)
            str += "(One or more independent tracks)";
        else
            str += "(Undefined)";

        return str;
    }

    public static string SMPTEToString(int division)
    {
        if (division < 0x8000)
        {
            return $"{division} (Ticks per quarter note)";
        }
        else
        {
            int smpte = ((division >> 8) & 0x3F) * -1;
            int tpf = division & 0xFF;

            return $"{smpte} / {tpf} (Negative SMPTE / Ticks per frame)";
        }
    }

    public static string MIDINoteToString(byte note)
    {
        string noteString = (note % 12) switch
        {
            0 => "C",
            1 => "C#",
            2 => "D",
            3 => "D#",
            4 => "E",
            5 => "F",
            6 => "F#",
            7 => "G",
            8 => "G#",
            9 => "A",
            10 => "A#",
            11 => "B",
            _ => throw new NotImplementedException()
        };

        return $"{noteString}{note / 12 - 1}";
    }

    public static string MIDIEventStatusToString(int statusHalfbyte, bool runningStatus = false)
    {
        string statusString = "";

        if (runningStatus) statusString += "Running ";

        statusString += statusHalfbyte switch
        {
            0x8 => $"Note Off (0x{statusHalfbyte:X1}n)",
            0x9 => $"Note On (0x{statusHalfbyte:X1}n)",
            0xA => $"Polyphonic Key Pressure (0x{statusHalfbyte:X1}n)",
            0xB => $"Control Change (0x{statusHalfbyte:X1}n)",
            0xC => $"Program Change (0x{statusHalfbyte:X1}n)",
            0xD => $"Channel Pressure (0x{statusHalfbyte:X1}n)",
            0xE => $"Pitch Bend Change (0x{statusHalfbyte:X1}n)",
            _ => throw new ArgumentException($"0x{statusHalfbyte:X1}n is not a recognized MIDI Channel status."),
        };

        return statusString;
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

    public static string MetaTimeSignatureToString(byte[] bytes)
    {
        if (bytes.Length != 4) throw new ArgumentException($"{bytes}is not a time signature.");

        return $"{bytes[0]}/{Math.Pow(2, bytes[1])} time, {bytes[2]} clocks per click, {bytes[3]} 32nd notes per quarter note";
    }

    public static string MetaKeySignatureToString(byte[] bytes)
    {
        if (bytes.Length != 2) throw new ArgumentException($"{bytes} is not a key signature.");

        sbyte sf = (sbyte)bytes[0];

        string str = "";

        if (sf == 0)
        {
            str += "Key of C";
        }
        else if (sf < 0)
        {
            str += $"{-sf} flat";
        }
        else if (sf > 0)
        {
            str += $"{sf} sharp";
        }
        if (sf > 1 || sf < -1)
        {
            str += "s";
        }

        str += ", ";

        if (bytes[1] == 0)
        {
            str += "major key";
        }
        else if (bytes[1] == 1)
        {
            str += "minor key";
        }
        else
        {
            str += "undefined key";
        }

        return str;
    }

    public static string ByteArrayToSMPTEOffset(byte[] bytes)
    {
        if (bytes.Length != 5) throw new ArgumentException($"{bytes} is not an SMPTE offset.");

        string hr = SMPTEToString(bytes[0]);
        int mn = bytes[1];
        int se = bytes[2];
        int fr = bytes[3];
        int ff = bytes[4];

        return $"{hr}, {mn}:{se}, {fr}/{ff}";
    }

    public static int ByteArrayToInt(byte[] bytes)
    {
        int result = 0;
        for (int i = bytes.Length - 1; i > 0; i--)
        {
            result += bytes[i] * 16 * i;
        }

        return result;
    }

    public static string ByteArrayToHexString(byte[] bytes)
    {
        string str = "0x ";
        foreach (var b in bytes)
        {
            str += $"{b:X2} ";
        }

        return str.TrimEnd();
    }
}
