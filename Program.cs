using MIDILib;
using MIDIReader.Model;

namespace MIDIReader;

class Program
{
    static void Main(string[] args)
    {
        int mode = 0;

        if (args.Length > 1)
        {
            string arg = args[1];

            if (arg.Equals("summary"))
                mode = 1;
            else if (arg.Equals("events"))
                mode = 2;
        }
        if (args.Length > 0)
        {
            var midi = MIDIParser.ParseFile(args[0]);
            if (mode < 2)
                Reader.PrintSummary(midi);
            if (mode == 0)
                Console.WriteLine();
            if (mode != 1)
                Reader.PrintMIDI(midi);
        }
    }
}
