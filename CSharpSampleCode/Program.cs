using System;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;
using CommunityToolkit.Diagnostics;

namespace CSharpSampleCode;

using NovatelReader = Logs.NovatelReader;
using static Logs.Util;

class Program
{
    static void Main(string[] args)
    {

#if DEBUG //For Debug only
        // args = new string[]{ "/l"};
        // args = new string[] { "/c8" };
        args = Array.Empty<string>();
#endif
        TcpClient? tcpClient = null;
        NovatelReader? rd = null;
        if (args.Length == 0)
        {
            (tcpClient, rd) = TestEthernet();
        }

        using var myclient = tcpClient;
        SerialPort? serialPort = null;
        if (myclient is null)
        {
            serialPort = NovatelReader.CreateSerialPort(args);
            if (serialPort is null)
            {
                string argStr = string.Join(", ", args);
                Console.WriteLine(
                    $"Could not open serial port for args={argStr}");
                return;
            }
            rd = new(serialPort, null, null, 5000);
        }
        Guard.IsNotNull(rd);

        #region BESTPOSB - Request and Decoding
        // Send a BestPosB log request
        // WriteCommand(serialPort, streamWriter, "\r\nLOG BESTPOSB ONCE\r\n");

        rd.WriteCommand("\r\nLOG BESTPOSB ONTIME 1\r\n");
        // get 5 bestpos messages
        for (int i = 0; i < 5; i++)
        {
            try
            {
                // Call the method that will swipe the serial port in search of a binary novatel message
                byte[] message = rd.GetNovatelMessage();

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
        rd.WriteCommand("\r\nLOG VERSIONB ONCE\r\n");
        try
        {
            // Call the method that will swipe the serial port in search of a binary novatel message
            byte[] message = rd.GetNovatelMessage();

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

    private static (TcpClient, NovatelReader) TestEthernet()
    {
        var (tcpClient, rd) = NovatelReader.CreateEthernet("192.168.1.16", 2000);
        Guard.IsNotNull(rd.StreamReader);
        rd.ResetLogs();

        rd.WriteCommand("\r\nLOG BESTPOSA ONCE\r\n");
        ReadBestposA(rd.StreamReader);

        return (tcpClient, rd);
    }

    private static void ReadBestposA(StreamReader streamReader)
    {
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
}