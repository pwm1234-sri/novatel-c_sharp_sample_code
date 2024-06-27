using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSampleCode.Logs;

public record VersionComponent
{
    public ComponentType ComponentType { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Psn { get; set; } = string.Empty;
    public string HwVersion { get; set; } = string.Empty;
    public string SwVersion { get; set; } = string.Empty;
    public string BootVersion { get; set; } = string.Empty;
    public string CompileDate { get; set; } = string.Empty;
    public string CompileTime { get; set; } = string.Empty;

    private static VersionComponent DecodeComponent(byte[] message, int h,
        int i)
    {
        VersionComponent val = new();
        int offset = h + (i * 108);

        int compType = BitConverter.ToInt32(message, offset + 4);
        val.ComponentType = (ComponentType)compType;
        val.Model = Util.DecodeString(message, offset + 8, 16);
        val.Psn = Util.DecodeString(message, offset + 24, 16);
        val.HwVersion = Util.DecodeString(message, offset + 40, 16);
        val.SwVersion = Util.DecodeString(message, offset + 56, 16);
        val.BootVersion = Util.DecodeString(message, offset + 72, 16);
        val.CompileDate = Util.DecodeString(message, offset + 88, 12);
        val.CompileTime = Util.DecodeString(message, offset + 100, 12);

        return val;
    }

    public static List<VersionComponent> DecodeVersionComponents(byte[] message,
        int logType, int h)
    {
        Console.WriteLine("**** VERSION LOG DECODED:");
        Console.WriteLine("* Message ID     : " + logType);

        int numberComp = BitConverter.ToInt32(message, h);
        Console.WriteLine("* # of Components: " + numberComp);

        List<VersionComponent> versionComponents = new(numberComp);
        for (int i = 0; i < numberComp; i++)
        {
            versionComponents.Add(
                VersionComponent.DecodeComponent(message, h, i));
        }

        return versionComponents;
    }

    public string ToPrettyString()
    {
        StringBuilder sb = new();
        sb.AppendLine("* Component Type : " +
                      GetCompTypeString((int)ComponentType));
        sb.AppendLine("   * Comp Model  : " + Model);
        sb.AppendLine("   * Serial #    : " + Psn);
        sb.AppendLine("   * Hw Version  : " + HwVersion);
        sb.AppendLine("   * Sw Version  : " + SwVersion);
        sb.AppendLine("   * Boot Version: " + BootVersion);
        sb.AppendLine("   * Fw Comp Date: " + CompileDate);
        sb.AppendLine("   * Fw Comp Time: " + CompileTime);
        return sb.ToString();
    }

    public static string GetCompTypeString(int compType)
    {
        switch ((ComponentType)compType)
        {
            case ComponentType.GPSCARD: return "OEM family component";
            case ComponentType.CONTROLLER: return "Reserved";
            case ComponentType.ENCLOSURE: return "OEM card enclosure";
            case ComponentType.USERINFO:
                return "Application specific information";
            case ComponentType.DB_HEIGHTMODEL: return "Height/track model data";
            case ComponentType.DB_USERAPP: return "User application firmware";
            case ComponentType.DB_USERAPPAUTO:
                return "Auto-starting user application firmware";
            case ComponentType.OEM6FPGA: return "OEM638 FPGA version";
            case ComponentType.GPSCARD2: return "Second card in a ProPak6";
            case ComponentType.BLUETOOTH:
                return "Bluetooth component in a ProPak6";
            case ComponentType.WIFI: return "Wi-Fi component in a ProPak6";
            case ComponentType.CELLULAR:
                return "Cellular component in a ProPak6";
            default:
                return "Unknown component";
        }
    }
}

public enum ComponentType
{
    // ReSharper disable once UnusedMember.Global
    UNKNOWN = 0, // Unknown component
    GPSCARD = 1, // GPSCard
    CONTROLLER = 2, // Reserved
    ENCLOSURE = 3, // OEM card enclosure
    USERINFO = 8, // App specific Information
    DB_HEIGHTMODEL = (0x3A7A0000 | 0), //Height/track model data
    DB_USERAPP = (0x3A7A0000 | 1), // User application firmware

    DB_USERAPPAUTO =
        (0x3A7A0000 | 5), // Auto-starting user application firmware
    OEM6FPGA = 12, // OEM638 FPGA version
    GPSCARD2 = 13, // Second card in a ProPak6
    BLUETOOTH = 14, // Bluetooth component in a ProPak6
    WIFI = 15, // Wifi component in a ProPak6
    CELLULAR = 16, // Cellular component in a ProPak6
};

public static class VersionComponentExtensions
{
    public static string ToPrettyString(
        this List<VersionComponent> versionComponents)
    {
        StringBuilder sb = new();
        for (int i = 0; i < versionComponents.Count; i++)
        {
            sb.AppendLine($"\nComponent {i:d02} -----");
            sb.AppendLine(versionComponents[i].ToPrettyString());
        }

        sb.AppendLine("");
        return sb.ToString();
    }
}