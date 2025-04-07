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
            Reader.Read(midi);
        }
    }
}
