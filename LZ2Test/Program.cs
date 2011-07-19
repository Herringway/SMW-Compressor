using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LZ2;
using System.IO;

namespace LZ2Test
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: {0} Input Output Offset", System.AppDomain.CurrentDomain.FriendlyName);
                return;
            }
            Stream file;
            try
            {
                file = File.Open(args[0], FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            file.Seek(long.Parse(args[2]), SeekOrigin.Begin);
            byte[] output = LZ2.LZ2.Decompress(file);
            file.Close();
            if (File.Exists(args[1]))
                File.Delete(args[1]);
            try
            {
                BinaryWriter outputFile = new BinaryWriter(File.Open(args[1], FileMode.OpenOrCreate));
                outputFile.Write(output);
                outputFile.BaseStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
    }
}
