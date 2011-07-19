using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LZ2
{
    public class LZ2
    {
        public static byte[] Decompress(Stream data)
        {
            BinaryReader Data = new BinaryReader(data);
            List<byte> outputbuffer = new List<byte>();
            byte command;
            byte commandlength;
            short longcommandlength;
            ushort tempaddress;
            byte tempbyte;
            byte tempbyte2;
            Boolean completed = false;

            while (!completed)
            {
                tempbyte = Data.ReadByte();
                command = (byte)((tempbyte >> 5) & 0x7);
                commandlength = (byte)(tempbyte & 0x1F);

                switch (command)
                {
                    case 0: //Direct Copy
                        for (int i = 0; i <= commandlength; i++)
                            outputbuffer.Add(Data.ReadByte());
                        break;
                    case 1: //Byte Fill
                        tempbyte = Data.ReadByte();
                        for (int i = 0; i <= commandlength; i++)
                            outputbuffer.Add(tempbyte);
                        break;
                    case 2: //Word Fill
                        tempbyte = Data.ReadByte();
                        tempbyte2 = Data.ReadByte();
                        for (int i = 0; i <= commandlength; i++)
                        {
                            if (i % 2 == 1)
                                outputbuffer.Add(tempbyte2);
                            else
                                outputbuffer.Add(tempbyte);
                        }
                        break;
                    case 3: //Increasing Fill
                        tempbyte = Data.ReadByte();
                        for (int i = 0; i <= commandlength; i++)
                            outputbuffer.Add(tempbyte++);
                        break;
                    case 4: //Repeat
                        tempaddress = BEShorttoLE(Data.ReadUInt16());
                        if (tempaddress > outputbuffer.Count)
                            throw new Exception("Bad Address!");
                        for (int i = 0; i <= commandlength; i++)
                            outputbuffer.Add(outputbuffer[tempaddress++]);
                        break;
                    case 5: //Unused
                        throw new Exception("Unused command byte detected (5)");
                    case 6: //Unused
                        throw new Exception("Unused command byte detected (6)");
                    case 7: //Long Command
                        command = (byte)((commandlength >> 2) & 0x7);
                        longcommandlength = (short)(((commandlength & 3) << 8) + Data.ReadByte());

                        switch (command)
                        {
                            case 0: //Direct Copy
                                for (int i = 0; i <= longcommandlength; i++)
                                    outputbuffer.Add(Data.ReadByte());
                                break;
                            case 1: //Byte Fill
                                tempbyte = Data.ReadByte();
                                for (int i = 0; i <= longcommandlength; i++)
                                    outputbuffer.Add(tempbyte);
                                break;
                            case 2: //Word Fill
                                tempbyte = Data.ReadByte();
                                tempbyte2 = Data.ReadByte();
                                for (int i = 0; i <= longcommandlength; i++)
                                {
                                    if (i % 2 == 1)
                                        outputbuffer.Add(tempbyte2);
                                    outputbuffer.Add(tempbyte);
                                }
                                break;
                            case 3: //Increasing Fill
                                tempbyte = Data.ReadByte();
                                for (int i = 0; i <= longcommandlength; i++)
                                    outputbuffer.Add(tempbyte++);
                                break;
                            case 4: //Repeat
                                tempaddress = BEShorttoLE(Data.ReadUInt16());
                                if (tempaddress > outputbuffer.Count)
                                    throw new Exception("Bad Address!");
                                for (int i = 0; i <= longcommandlength; i++)
                                    outputbuffer.Add(outputbuffer[tempaddress++]);
                                break;
                            case 5: //Unused
                                throw new Exception("Unused command byte detected (5)");
                            case 6: //Unused
                                throw new Exception("Unused command byte detected (6)");
                            case 7: //End
                                completed = true;
                                break;

                        }
                        break;

                }
            }

            return outputbuffer.ToArray();
        }
        private static ushort BEShorttoLE(ushort input)
        {
            return (ushort)((input>>8) + (input<<8));
        }
    }
}
