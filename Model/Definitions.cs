namespace MIDIReader.Model;

public static class Definitions
{
    public static string GetFormat(int f)
    {
        if (f == 0)
            return $"{f} (Single track)";
        if (f == 1)
            return $"{f} (One or more sequential tracks)";
        if (f == 2)
            return $"{f} (One or more independent tracks)";

        return "Undefined";
    }

    public static string GetDivision(int d)
    {
        if (d < 0x80)
        {
            return $"{d} (Ticks per quarter note)";
        }
        else
        {
            int smpte = (d - 0x80) >> 8;
            int tpf = d - 0x80 - (smpte << 8);

            return $"{smpte} / {tpf} (Negative SMPTE / Ticks per frame)";
        }
    }
}
