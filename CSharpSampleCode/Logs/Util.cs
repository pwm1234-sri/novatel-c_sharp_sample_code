using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CSharpSampleCode.Logs;

public class Util
{
    public enum BinaryLogType
    {
        VERB_LOG_TYPE = 37,
        BESTPOSB_LOG_TYPE = 42,
    }

    public static string DecodeString(byte[] message, int offset, int count)
    {
        string s = new(Encoding.ASCII.GetChars(message, offset, count));
        if (s.Contains('\0'))
            s = s.Substring(0, s.IndexOf('\0'));
        return s;
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
        var binaryLogType = (BinaryLogType)logType;
        switch (binaryLogType)
        {
            case BinaryLogType.BESTPOSB_LOG_TYPE:
                {
                    var msg = Bestpos.Decode(message, logType, h);
                    Console.WriteLine(msg.ToPrettyString());
                    // Console.WriteLine($"Bestpos:\n{msg}\n\n");
                    break;
                }

            case BinaryLogType.VERB_LOG_TYPE:
                {
                    var msg = VersionComponent.DecodeVersionComponents(message, logType, h);
                    string prettyStr = string.Join("\n----\n", msg);
                    // Console.WriteLine(prettyStr);
                    Console.WriteLine($"Version:\n{msg.ToPrettyString()}\n\n");
                    break;
                }
            default:
                Console.WriteLine($"*** Unhandled binaryLogType={binaryLogType}");
                break;
        }
    }


    public static (List<string>, List<string>) TokenizeAsciiLog(string line)
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
                            NumberStyles.HexNumber);
                        var crcAct = CalculateBlockCRC32(line,
                            nameTokens[0].Length + 1,
                            line.Length - crcTokens[1].Length - 1);
                        if (crcExp == crcAct)
                        {
                            return MakeHeaderCommandTokens(tokens, name);
                        }
                    }
                    catch (FormatException)
                    {
                        // TODO: this should be a log.error
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
}