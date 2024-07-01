using System;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CommunityToolkit.Diagnostics;

namespace CSharpSampleCode.Logs;

public class NovatelReaderWriter
{
    private readonly SerialPort? _serialPort;
    private readonly StreamReader? _streamReader;
    private readonly NetworkStream? _streamWriter;

    public NovatelReaderWriter(SerialPort? sp, StreamReader? sr, NetworkStream? ns, int timeOut)
    {
        _serialPort = sp;
        _streamReader = sr;
        _streamWriter = ns;
        Timeout = timeOut;
        if (sp != null)
        {
            sp.ReadTimeout = timeOut;
        }
    }
    public static (TcpClient, NovatelReaderWriter) CreateEthernet(string hostname,
        int port)
    {
        // See [Static IP Address Configuration]
        // (https://docs.novatel.com/OEM7/Content/Ethernet_Configuration/Static_IP_Address_Config.htm#StaticIP_Receiver)
        // for details on how to configure the ethernet settings for a CPT-7
        // receiver. If the ethernet address is already set, then you can use
        // the following command to create the port. 
        // 
        // ICOMCONFIG ICOM1 TCP :2000
        //
        // There is some bootstrapping involved here; you will need a USB/COM
        // connection to configure the ethernet settings. And when connecting
        // to the webserver running on the receiver, the default password, if
        // needed, is the product serial number (PSN).
        var tcpClient = new TcpClient(hostname, port);
        NetworkStream ns = tcpClient.GetStream();
        ns.ReadTimeout = 1000;
        ns.WriteTimeout = 1000;
        var bufferedStream = new BufferedStream(ns);
        var streamReader = new StreamReader(bufferedStream, Encoding.UTF8);
        streamReader.DiscardBufferedData();
        return (tcpClient, new(null, streamReader, ns, 5000));
    }

    public static SerialPort? CreateSerialPort(string[] args)
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
                SendBreakSignal(serialPort);
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
            try
            {
                var portNames = SerialPort.GetPortNames();
                if (portNames.Length > 0)
                {
                    Console.WriteLine("* This computer has the following COM Ports: ");
                    foreach (string s in portNames)
                    {
                        Console.WriteLine("* " + s);
                    }
                }
                else
                {
                    Console.WriteLine("*** This computer has no COM Ports ***");
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
    public int Timeout { get; }

    public StreamReader? StreamReader => _streamReader;

    /// <summary>
    /// The current COM port configuration can be reset to its default state at any time by sending it two hardware
    /// break signals of 250 milliseconds each, spaced by fifteen hundred milliseconds(1.5 seconds) with a pause of
    /// at least 250 milliseconds following the second break.
    /// </summary>
    /// <param name="sp">Serial Port to send the break to</param>
    private static void SendBreakSignal(SerialPort sp)
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

    // <summary> Method that search for a Novatel message in the serial
    // port and return a byte[] or return a null object if the timeOut
    // value is reached. Note that this method ignore any bytes that don't
    // match with a Novatel log. It will search straight for the binary
    // message's sync bytes (0xAA, 0x44 and 0x12)
    // </summary>
    // <param name="sp">Serial Port to read the message from</param>
    // <param name="timeOut">Timeout in milliseconds. 10000 is the default</
    // param>
    // <returns></returns>
    public byte[] GetNovatelMessage()
    {
        long timeOutLimit = DateTime.Now.Ticks + TimeSpan.TicksPerMillisecond * Timeout;
#if DEBUG
        _ = timeOutLimit;
        timeOutLimit = DateTime.MaxValue.Ticks;
#endif

        // initially 3 bytes for the sync bytes + 1 for the header length
        byte[] header = new byte[4] { 0x00, 0x00, 0x00, 0x00 };

        bool readFirst = true;
        do
        {
            if (readFirst)
                Read(header, 0, 1); // Read the first byte

            if (header[0] == 0xAA) // Check the first sync byte
            {
                Read(header, 1, 1); // Read the second byte
                if (header[1] == 0x44) // Check the second sync byte
                {
                    Read(header, 2, 1); // Read the third byte
                    if (header[2] == 0x12) // Check the third sync byte
                    {
                        // At this point we have the 3 sync bytes, so all that is needed is to load the rest of the message
                        Read(header, 3, 1); // Read the header length in the byte of index 3

                        int headerLength = header[3];

                        {
                            byte[]? tmpBuffer = new byte[headerLength];
                            header.CopyTo(tmpBuffer, 0); // Copy header to a temporary buffer
                            header = new byte[headerLength];
                            tmpBuffer.CopyTo(header, 0); // Copy the header back into the header array after updating its size
                        }

                        Read(header, 4, headerLength - 4); // Read the rest of the header

                        int messageLength = BitConverter.ToUInt16(header, 8); // Convert the 8th and 9th bytes to a Ushort that is the body message

                        byte[] message = new byte[headerLength + messageLength]; // Create a buffer for the whole message

                        header.CopyTo(message, 0); // Copy the header to the message

                        Read(message, headerLength, messageLength); // Read the whole message into the message buffer, using the headerLenght as offset

                        byte[] crc = new byte[4];
                        Read(crc, 0, 4); // Create and populate the CRC byte array

                        ulong crc32 = BitConverter.ToUInt32(crc, 0);

                        if (crc32 == Util.CalculateBlockCRC32(message)) //If the calculated CRC matches the received CRC, return the message
                            return message;

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

    public void WriteCommand(string cmd)
    {
        if (_serialPort is not null)
        {
            _serialPort.Write(cmd);
        }
        else if (_streamWriter is not null)
        {
            byte[] data = Encoding.ASCII.GetBytes(cmd);
            _streamWriter?.Write(data, 0, data.Length);
        }
        else
        {
            throw new ArgumentException(
                "Either serial port or stream writer must be valid");
        }
        Thread.Sleep(1000);
    }

    public void ResetLogs()
    {
        WriteCommand("\r\nUNLOGALL ALL_PORTS true\r\n");

        Guard.IsNotNull(StreamReader);
        Guard.IsNull(_serialPort);
        // XXX: I do not know how to read a line from a serial port; 

        string? line = StreamReader.ReadLine();

        // TODO: this should be a log.info
        Console.WriteLine($"Read line={line}");
    }
}