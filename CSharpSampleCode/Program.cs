using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Net.Sockets;

namespace CSharpSampleCode
{
    class Program
    {
        static void Main(string[] args)
        {

#if DEBUG //For Debug only
            // args = new string[]{ "/l"};
            // args = new string[] { "/c8" };
            args = Array.Empty<string>();
#endif
            TcpClient? client = null;
            StreamReader? streamReader = null;
            NetworkStream? streamWriter = null;
            if (args.Length == 0)
            {
                (client, streamReader, streamWriter) = TestEthernet();
            }

            using var myclient = client;
            SerialPort? serialPort = null;
            if (myclient is null)
            {
                serialPort = CreateSerialPort(args);
                if (serialPort is null)
                {
                    string argStr = string.Join(", ", args);
                    Console.WriteLine(
                        $"Could not open serial port for args={argStr}");
                    return;
                }
            }

            #region BESTPOSB - Request and Decoding
            // Send a BestPosB log request
            // WriteCommand(serialPort, streamWriter, "\r\nLOG BESTPOSB ONCE\r\n");
            WriteCommand(serialPort, streamWriter, "\r\nLOG BESTPOSB ONTIME 1\r\n");
            Thread.Sleep(1000);

            NovatelReader rd = new NovatelReader(serialPort, streamReader, 5000);

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    // Call the method that will swipe the serial port in search of a binary novatel message
                    byte[] message =
                        getNovatelMessage(rd);

                    // If a message was found, pass it to the decode method
                    if (message != null)
                    {
                        Console.WriteLine($"*** Decoded pos record {i} ***");
                        DecodeBinaryMessage(message);
                    }

                    else
                        Console.WriteLine("Unable to retrieve message.");
                }
                catch (Exception ex)
                {
                    if (ex is TimeoutException)
                        Console.WriteLine("Timeout to retrieve the message.");

                    else
                    {
                        Console.WriteLine("Unfortunately an " +
                                          ex.GetType().Name + " happened.");
                    }
                }
            }

            #endregion

            #region VERSIONB - Request and Decoding
            // Send a BestPosB log request
            WriteCommand(serialPort, streamWriter, "\r\nLOG VERSIONB ONCE\r\n");
            Thread.Sleep(1000);
            try
            {
                // Call the method that will swipe the serial port in search of a binary novatel message
                byte[] message = getNovatelMessage(rd);

                // If a message was found, pass it to the decode method
                if (message != null)
                    DecodeBinaryMessage(message);

                else
                    Console.WriteLine("Unable to retrieve message.");
            }
            catch (Exception ex)
            {
                if (ex is TimeoutException)
                    Console.WriteLine("Timeout to retrieve the message.");

                else
                {
                    Console.WriteLine("Unfortunately an " + ex.GetType().Name + " happened.");
                }
            }
            #endregion

            #region Owil Messages
            // Shakuntala said the InertialExplorer app needs the following logs
            // for its correction post-processing
            //
            // LOG RANGECMPB ONTIME 1
            // LOG GPSEPHEMB ONNEW
            // LOG GLOEPHEMERISB ONNEW
            // LOG GALINAVEPHEMERISB ONNEW
            // LOG GALFNAVEPHEMERISB ONNEW
            // LOG BDSEPHEMERISB ONNEW
            // LOG QZSSEPHEMERISB ONNEW
            // LOG RAWIMUSXB ONNEW
            // LOG VERSIONB ONCE
            // LOG RXCONFIGB ONCE
            // LOG RXSTATUSB ONCE
            // LOG THISANTENNATYPEB ONCE
            // LOG INSPVAXB ONTIME 1
            // LOG BESTPOSB ONTIME 1
            // LOG BESTGNSSPOSB ONTIME 1
            // LOG TIMEB ONTIME 1
            // LOG HEADING2B ONNEW
            // LOG INSCONFIGB ONCHANGED
            // LOG INSUPDATESTATUSB ONCHANGED
            #endregion
        }

        private static void WriteCommand(SerialPort? serialPort,
            NetworkStream? streamWriter, string cmd)
        {
            if (serialPort != null)
            {
                serialPort?.Write(cmd);
            }
            else
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(cmd);
                streamWriter?.Write(data, 0, data.Length);
            }
        }

        private static SerialPort? CreateSerialPort(string[] args)
        {
            SerialPort? serialPort = null;

            if (args.Length == 1 && args[0].IndexOf("/c") == 0)
            {
                string portName = "N/A";
                try
                {
                    int portNum = int.Parse(args[0].Replace("/c", ""));
                    portName = "COM" + portNum;
                    serialPort = new SerialPort();
                    serialPort.PortName = portName;
                    serialPort.BaudRate = 9600;
                    serialPort.Open();

                    #region Break Signal
                    Console.WriteLine("Sending break signal to " + serialPort.PortName);
                    // Call the break signal function, passing the serial port as a parameter
                    sendBreakSignal(serialPort);
                    Console.WriteLine("Break sent to " + serialPort.PortName);
                    Console.WriteLine(serialPort.PortName + " must be at its default configuration now.");
                    #endregion

                    // Discard any data in the in and out buffers
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    Thread.Sleep(1000);
                }
                catch
                {
                    Console.WriteLine("Error when opening ComPort " + portName);
                    serialPort = null;
                }
            }

            else if (args.Length == 1 && args[0] == "/l")
            {
                Console.WriteLine("* This computer have the following COM Ports: ");

                try
                {
                    foreach (string s in SerialPort.GetPortNames())
                    {
                        Console.WriteLine("* " + s);
                    }
                }
                catch   // In case there is not even one serial port
                {
                }
            }

            else
            {
                Console.WriteLine("*");
                if (args.Length != 1 || args[0] != "/h")
                {
                    Console.WriteLine("* ERROR: unrecognized or incomplete command line.");
                    Console.WriteLine("*");
                }
                Console.WriteLine("* NovAtel C# Sample Code");
                Console.WriteLine("* Usage: csharpsample [option]");
                Console.WriteLine("*      /c<port>       : COM Port #");
                Console.WriteLine("*      /l             : List COM Ports");
                Console.WriteLine("*      /h             : This help list");
            }

            return serialPort;
        }

        private static (TcpClient, StreamReader, NetworkStream) TestEthernet()
        {
            TcpClient client = new TcpClient("192.168.1.16", 2000);
            var (streamReader, networkStream) = CreateStream(client);
            streamReader.DiscardBufferedData();
            string msg = "\r\nUNLOGALL ALL_PORTS true\r\n";
            byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);
            networkStream.Write(data, 0, data.Length);
            Thread.Sleep(1000);
            string? line = streamReader.ReadLine();
            Console.WriteLine($"Read line={line}");

            ReadBestPosA(networkStream, streamReader);

            return (client, streamReader, networkStream);
        }

        private static void ReadBestPosA(NetworkStream networkStream,
            StreamReader streamReader)
        {
            string msg;
            byte[] data;
            msg = "\r\nLOG BESTPOSA ONCE\r\n";
            data = System.Text.Encoding.ASCII.GetBytes(msg);
            networkStream.Write(data, 0, data.Length);
            Thread.Sleep(1000);

            byte[] buf = new byte[1024];
            int nread = buf.Length;
            do
            {
                try
                {
                    string? line = streamReader.ReadLine();
                    Console.WriteLine($"Read line={line}");
                    if (line != null)
                    {
                        var (headerTokens, commandTokens) =
                            TokenizeAsciiLog(line);
                        var hstr = string.Join(", ", headerTokens);
                        var cstr = string.Join(", ", commandTokens);
                        Console.WriteLine($"Header:  {hstr}");
                        Console.WriteLine($"Command: {cstr}");
                    }
                    else
                    {
                        nread = 0;
                    }
                }
                catch (IOException)
                {
                    nread = 0;
                }
            } while (nread > 0);
        }

        private static (StreamReader, NetworkStream) CreateStream(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            ns.ReadTimeout = 1000;
            ns.WriteTimeout = 1000;
            var bufferedStream = new BufferedStream(ns);
            return (new StreamReader(bufferedStream, Encoding.UTF8), ns);
        }

        private static (List<string>, List<string>) TokenizeAsciiLog(string line)
        {
            string[] tokens = line.Split(',');
            if (tokens.Length > 1)
            {
                string[] nameTokens = tokens[0].Split('#');
                if (nameTokens.Length > 1)
                {
                    string name = nameTokens[1];
                    string[] crcTokens = tokens[^1].Split('*');
                    if (crcTokens.Length > 1)
                    {
                        try
                        {
                            var crcExp = ulong.Parse(crcTokens[1],
                                System.Globalization.NumberStyles.HexNumber);
                            var crcAct = CalculateBlockCRC32(line,
                                nameTokens[0].Length + 1,
                                line.Length - crcTokens[1].Length - 1);
                            if (crcExp == crcAct)
                            {
                                return MakeHeaderCommandTokens(tokens, name);
                            }
                        }
                        catch (System.FormatException)
                        {
                            Console.WriteLine(
                                $"Cannot parse crc from {crcTokens[1]}");
                        }
                    }
                }
            }

            return (new List<string>(Array.Empty<string>()), new List<string>(
                Array.Empty<string>()));
        }

        // split ascii CSV tokens into its header and command sets; see
        // https://docs.novatel.com/OEM7/Content/Messages/ASCII.htm
        private static (List<string>, List<string>) MakeHeaderCommandTokens(
            string[] tokens, string name)
        {
            var headerTokens = new List<string>(tokens);
            var commandTokens = new List<string>(tokens.Length);
            int commandIdx = commandTokens.Count;
            // initialize header tokens
            headerTokens[0] = name;
            for (int i = 1; i < tokens.Length; i++)
            {
                var curToken = tokens[i];
                if (curToken.Contains(';'))
                {
                    // the token with ; is the end of the header
                    string[] eohTokens = curToken.Split(';');
                    headerTokens[i] = eohTokens[0];
                    commandTokens.Add(eohTokens[1]);
                    var count = headerTokens.Count - i - 1;
                    headerTokens.RemoveRange(i + 1, count);
                    commandIdx = i + 1;
                    break;
                }
            }
            headerTokens.Insert(0, "#");

            for (; commandIdx < tokens.Length; commandIdx += 1)
            {
                commandTokens.Add(tokens[commandIdx]);
            }

            return (headerTokens, commandTokens);
        }

        #region Methods

        /// <summary>
        /// The current COM port configuration can be reset to its default state at any time by sending it two hardware
        /// break signals of 250 milliseconds each, spaced by fifteen hundred milliseconds(1.5 seconds) with a pause of
        /// at least 250 milliseconds following the second break.
        /// </summary>
        /// <param name="sp">Serial Port to send the break to</param>
        public static void sendBreakSignal(SerialPort sp)
        {
            sp.BaudRate = 9600;
            int bytesPerSecond = sp.BaudRate / 8; // number or bytes sent in 1000 ms
            byte[] breakSignal = new byte[bytesPerSecond / 4]; // Create a byte array that will take 1/4 of a second (250 ms) to be sent

            // Populate the array
            for (int i = 0; i < breakSignal.Length; i++)
                breakSignal[i] = 0xFF;

            sp.Write(breakSignal, 0, breakSignal.Length); // Send the first hardware break signal
            Thread.Sleep(1500); // Wait 1500 milisseconds to send the next break signal
            sp.Write(breakSignal, 0, breakSignal.Length); // Send the second hardware break signal
            Thread.Sleep(250); // Wait 250 milisseconds after the second hardware break signal
        }

        // <summary> Method that search for a NovAtel message in the serial
        // port and return a byte[] or return a null object if the timeOut
        // value is reached. Note that this method ignore any bytes that don't
        // match with a NovAtel log. It will search straight for the binary
        // message's sync bytes (0xAA, 0x44 and 0x12)
        // </summary>
        // <param name="sp">Serial Port to read the message from</param>
        // <param name="timeOut">Timeout in milisseconds. 10000 is the default</
        // param>
        // <returns></returns>
        public static byte[] getNovatelMessage(NovatelReader rd) 
        {
            long timeOutLimit = DateTime.Now.Ticks + (TimeSpan.TicksPerMillisecond * rd.Timeout);
#if DEBUG
            timeOutLimit = DateTime.MaxValue.Ticks;
#endif

            byte[] header = new byte[4] { 0x00, 0x00, 0x00, 0x00 }; // initially 3 bytes for the sync bytes + 1 for the header lenght

            bool readFirst = true;
            do
            {
                if (readFirst)
                    rd.Read(header, 0, 1); // Read the first byte

                if (header[0] == 0xAA) // Check the first sync byte
                {
                    rd.Read(header, 1, 1); // Read the second byte
                    if (header[1] == 0x44) // Check the second sync byte
                    {
                        rd.Read(header, 2, 1); // Read the third byte
                        if (header[2] == 0x12) // Check the third sync byte
                        {
                            // At this point we have the 3 sync bytes, so all that is needed is to load the rest of the message
                            rd.Read(header, 3, 1); // Read the header lenght in the byte of index 3

                            int headerLenght = header[3];

                            byte[]? tmpBuffer = new byte[headerLenght];
                            header.CopyTo(tmpBuffer, 0); // Copy header to a temporary buffer
                            header = new byte[headerLenght];
                            tmpBuffer.CopyTo(header, 0); // Copy the header back into the header array after updating its size
                            tmpBuffer = null;

                            rd.Read(header, 4, headerLenght - 4); // Read the rest of the header

                            int messageLenght = BitConverter.ToUInt16(header, 8); // Convert the 8th and 9th bytes to a Ushort that is the body message

                            byte[] message = new byte[headerLenght + messageLenght]; // Create a buffer for the whole message

                            header.CopyTo(message, 0); // Copy the header to the message

                            rd.Read(message, headerLenght, messageLenght); // Read the whole message into the message buffer, using the headerLenght as offset

                            byte[] crc = new byte[4];
                            rd.Read(crc, 0, 4); // Create and populate the CRC byte array

                            ulong crc32 = BitConverter.ToUInt32(crc, 0);

                            if (crc32 == CalculateBlockCRC32(message)) //If the calculated CRC matches the received CRC, return the message
                                return message;
                            else
                                readFirst = true;
                        }
                        else if (header[2] == 0xAA) // If the byte found is 0xAA it will be used in the next loop
                            readFirst = false;
                        else
                            readFirst = true;
                    }
                    else if (header[1] == 0xAA) // If the byte found is 0xAA it will be used in the next loop
                        readFirst = false;
                    else
                        readFirst = true;
                }
                else
                    readFirst = true;
            } while (timeOutLimit > DateTime.Now.Ticks);
            return Array.Empty<byte>();
        }

        #region Crc Calculations
        /// <summary>
        /// Calculate a CRC value to be used by CRC calculation functions.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static ulong CRC32Value(int i)
        {
            const ulong CRC32_POLYNOMIAL = 0xEDB88320L;
            int j;
            ulong ulCRC;
            ulCRC = (ulong)i;
            for (j = 8; j > 0; j--)
            {
                if ((ulCRC & 1) == 1)
                    ulCRC = (ulCRC >> 1) ^ CRC32_POLYNOMIAL;
                else
                    ulCRC >>= 1;
            }
            return ulCRC;
        }

        /// <summary>
        /// Calculates the CRC-32 of a block of data all at once
        /// </summary>
        /// <param name="buffer">byte[] to calculate the CRC-32</param>
        /// <returns></returns>
        public static ulong CalculateBlockCRC32(byte[] buffer)
        {
            ulong ulTemp1;
            ulong ulTemp2;
            ulong ulCRC = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                ulTemp1 = (ulCRC >> 8) & 0x00FFFFFFL;
                ulTemp2 = CRC32Value(((int)ulCRC ^ buffer[i]) & 0xff);
                ulCRC = ulTemp1 ^ ulTemp2;
            }
            return (ulCRC);
        }
        public static ulong CalculateBlockCRC32(string s, int idxBgn, int idxEnd)
        {
            ulong ulTemp1;
            ulong ulTemp2;
            ulong ulCRC = 0;

            for (int i = idxBgn; i < idxEnd; i++)
            {
                ulTemp1 = (ulCRC >> 8) & 0x00FFFFFFL;
                ulTemp2 = CRC32Value(((int)ulCRC ^ s[i]) & 0xff);
                ulCRC = ulTemp1 ^ ulTemp2;
            }
            return (ulCRC);
        }
        #endregion

        /// <summary>
        /// Decode a Binary message and output some data based on the Message ID
        /// </summary>
        /// <param name="message"></param>
        public static void DecodeBinaryMessage(byte[] message)
        {
            int h = message[3]; //header size, for offset
            int logType = BitConverter.ToUInt16(message, 4);

            // Decode the log based on its type
            var binaryLogType = (BINARY_LOG_TYPE)logType;
            switch (binaryLogType)
            {
                case BINARY_LOG_TYPE.BESTPOSB_LOG_TYPE:
                    var msg = Logs.Bestpos.Decode(message, logType, h);
                    Console.WriteLine(msg.ToPrettyString());
                    Console.WriteLine($"Bestpos:\n{msg}\n\n");
                    break;

                case BINARY_LOG_TYPE.VERB_LOG_TYPE:
                    DecodeVersion(message, logType, h);
                    break;
                default:
                    Console.WriteLine($"*** Unhandled binaryLogType={binaryLogType}");
                    break;
            }
        }


        private static void DecodeVersion(byte[] message, int logType, int h)
        {
            Console.WriteLine("**** VERSION LOG DECODED:");
            Console.WriteLine("* Message ID     : " + logType);

            int numberComp = BitConverter.ToInt32(message, h);
            Console.WriteLine("* # of Components: " + numberComp);

            for (int i = 0; i < numberComp; i++)
            {
                DecodeVersionComponent(message, h, i);
            }
        }

        private static void DecodeVersionComponent(byte[] message, int h, int i)
        {
            int offset = h + (i * 108);

            int compType = BitConverter.ToInt32(message, offset + 4);
            Console.WriteLine($"* {i:d02}: Component Type : " + GetCompTypeString(compType));


            string model = new string(Encoding.ASCII.GetChars(message, offset + 8, 16));
            if (model.Contains('\0'))
                model = model.Substring(0, model.IndexOf('\0'));
            Console.WriteLine("   * Comp Model  : " + model);

            string psn = new string(Encoding.ASCII.GetChars(message, offset + 24, 16));
            if (psn.Contains('\0'))
                psn = psn.Substring(0, psn.IndexOf('\0'));
            Console.WriteLine("   * Serial #    : " + psn);

            string hwVersion = new string(Encoding.ASCII.GetChars(message, offset + 40, 16));
            if (hwVersion.Contains('\0'))
                hwVersion = hwVersion.Substring(0, hwVersion.IndexOf('\0'));
            Console.WriteLine("   * Hw Version  : " + hwVersion);

            string swVersion = new string(Encoding.ASCII.GetChars(message, offset + 56, 16));
            if (swVersion.Contains('\0'))
                swVersion = swVersion.Substring(0, swVersion.IndexOf('\0'));
            Console.WriteLine("   * Sw Version  : " + swVersion);

            string bootVersion = new string(Encoding.ASCII.GetChars(message, offset + 72, 16));
            if (bootVersion.Contains('\0'))
                bootVersion = bootVersion.Substring(0, bootVersion.IndexOf('\0'));
            Console.WriteLine("   * Boot Version: " + bootVersion);

            string compDate = new string(Encoding.ASCII.GetChars(message, offset + 88, 12));
            if (compDate.Contains('\0'))
                compDate = compDate.Substring(0, compDate.IndexOf('\0'));
            Console.WriteLine("   * Fw Comp Date: " + compDate);

            string compTime = new string(Encoding.ASCII.GetChars(message, offset + 100, 12));
            if (compTime.Contains('\0'))
                compTime = compTime.Substring(0, compTime.IndexOf('\0'));
            Console.WriteLine("   * Fw Comp Time: " + compTime);
        }

        public enum BINARY_LOG_TYPE
        {
            VERB_LOG_TYPE = 37,
            BESTPOSB_LOG_TYPE = 42,
        }

        #region Component Types
        enum COMPONENT_TYPE
        {
            UNKNOWN = 0,                 // Unknown component
            GPSCARD = 1,                 // GPSCard
            CONTROLLER = 2,                 // Reserved
            ENCLOSURE = 3,                 // OEM card enclosure
            USERINFO = 8,                 // App specific Information
            DB_HEIGHTMODEL = (0x3A7A0000 | 0),  //Height/track model data
            DB_USERAPP = (0x3A7A0000 | 1),  // User application firmware
            DB_USERAPPAUTO = (0x3A7A0000 | 5),  // Auto-starting user application firmware
            OEM6FPGA = 12,                // OEM638 FPGA version
            GPSCARD2 = 13,                // Second card in a ProPak6
            BLUETOOTH = 14,                // Bluetooth component in a ProPak6
            WIFI = 15,                // Wifi component in a ProPak6
            CELLULAR = 16,                // Cellular component in a ProPak6
        };

        public static string GetCompTypeString(int compType)
        {
            switch ((COMPONENT_TYPE)compType)
            {
                case COMPONENT_TYPE.GPSCARD: return "OEM family component";
                case COMPONENT_TYPE.CONTROLLER: return "Reserved";
                case COMPONENT_TYPE.ENCLOSURE: return "OEM card enclosure";
                case COMPONENT_TYPE.USERINFO: return "Application specific information";
                case COMPONENT_TYPE.DB_HEIGHTMODEL: return "Height/track model data";
                case COMPONENT_TYPE.DB_USERAPP: return "User application firmware";
                case COMPONENT_TYPE.DB_USERAPPAUTO: return "Auto-starting user application firmware";
                case COMPONENT_TYPE.OEM6FPGA: return "OEM638 FPGA version";
                case COMPONENT_TYPE.GPSCARD2: return "Second card in a ProPak6";
                case COMPONENT_TYPE.BLUETOOTH: return "Bluetooth component in a ProPak6";
                case COMPONENT_TYPE.WIFI: return "Wi-Fi component in a ProPak6";
                case COMPONENT_TYPE.CELLULAR: return "Cellular component in a ProPak6";
                default:
                    return "Unknown component";
            }
        }
        #endregion
#endregion

    }

    internal class NovatelReader
    {
        private SerialPort? _serialPort;
        private StreamReader? _streamReader;
        public int Timeout { get; }

        public NovatelReader(SerialPort? sp, StreamReader? sr, int timeOut)
        {
            _serialPort = sp;
            _streamReader = sr;
            Timeout = timeOut;
            if (sp != null)
            {
                sp.ReadTimeout = timeOut;
            }
        }

        public void Read(byte[] header, int offset, int count)
        {
            if (_serialPort is not null)
            {
                _serialPort.Read(header, offset, count);
            }
            else if (_streamReader is not null)
            {
                _streamReader.BaseStream.Read(header, offset, count);
            }
            else
            {
                throw new ArgumentException(
                    "Both _serialPort and _streamReader are null");
            }
        }
    }
}
