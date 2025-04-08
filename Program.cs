using MIDILib;
using MIDIReader.Model;

namespace MIDIReader;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            var midi = MIDIParser.ParseFile(args[0]);
            if (args.Length > 1 && args[1] == "stdout")
            {
                Reader.Print(midi);
            }
            else
            {
                Reader.Read(midi);
            }
        }
    }
}
