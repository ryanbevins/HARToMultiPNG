using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HARToMultiPNG
{
    internal class Program
    {
        private static string _pathToFile;
        private static byte[] _bytes;
        private static readonly List<int> _startingPositions = new List<int>();

        public static void Main(string[] args)
        {
            _pathToFile = args[0];
            var reader = new BinaryReader(new FileStream(_pathToFile, FileMode.Open, FileAccess.ReadWrite));
            reader.BaseStream.Position = 0x00;
            _bytes = reader.ReadBytes((int) reader.BaseStream.Length);
            reader.Close();
            ExtractPNGFiles();

            Console.WriteLine("Complete! Press any key to exit.");
            Console.ReadKey();
        }

        private static void ExtractPNGFiles()
        {
            for (var i = 0; i < _bytes.Length; i++)
                if (_bytes[i] == 0x89 && _bytes[i + 1] == 0x50 && _bytes[i + 2] == 0x4E && _bytes[i + 3] == 0x47)
                    _startingPositions.Add(i);
            for (var i = 0; i < _startingPositions.Count; i++)
            {
                var outPutPath = string.Format(Directory.GetCurrentDirectory() + @"\{0}.png", i);
                var file = new List<byte>();
                var endPos = i != _startingPositions.Count - 1 ? _startingPositions[i + 1] : _bytes.Length - 1;
                for (var j = _startingPositions[i]; j < endPos; j++) file.Add(_bytes[j]);
                File.WriteAllBytes(outPutPath, file.ToArray());
                Console.WriteLine("Extracted successfully to: " + outPutPath);
            }
        }
    }
}