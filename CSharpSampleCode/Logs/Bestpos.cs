using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CSharpSampleCode.Logs;

public record Bestpos
{
    // [Display(Name = "Foobar")]
    public int LogType { get; set; }
    public SolutionStatus SolutionStatus { get; set; }
    public PositionType PositionType { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
    public double Hgt { get; set; }
    public float Undulation { get; set; }
    public int Datum { get; set; }
    public float LatSigma { get; set; }
    public float LonSigma { get; set; }
    public float HgtSigma { get; set; }
    public string BaseId { get; set; } = string.Empty;
    public float DiffAge { get; set; }
    public float SolutionAge { get; set; }
    public int NumSatsTracked { get; set; }
    public int NumSatsInSolution { get; set; }
    public int NumSatsWithL1E1B1 { get; set; }
    public int NumSatsMultiFreq { get; set; }
    public int Reserved19 { get; set; }
    public byte ExtSolutionStatus { get; set; }
    public byte GalileoSigMask { get; set; }
    public byte GpsSigMask { get; set; }
    public bool ValidCrc { get; set; }

    public string ToPrettyString()
    {
        StringBuilder sb = new();
        sb.AppendLine("**** BESTPOS LOG DECODED:");
        sb.AppendLine("* Message ID     : " + LogType);
        sb.AppendLine("* Solution Status: " + SolutionStatus.GetDisplayName());
        sb.AppendLine("* Position Type  : " + ((int)PositionType).GetPosTypeString());
        sb.AppendLine("* Latitude       : " + Lat.ToString("F10"));
        sb.AppendLine("* Longitude      : " + Lon.ToString("F10"));
        sb.AppendLine("* Height         : " + Hgt.ToString("F10"));
        sb.AppendLine("* Undulation     : " + Undulation.ToString("F4"));
        sb.AppendLine("* Datum          : " + Datum.GetDatumString());
        sb.AppendLine("* Lat Std Dev    : " + LatSigma.ToString("F4"));
        sb.AppendLine("* Lon Std Dev    : " + LonSigma.ToString("F4"));
        sb.AppendLine("* Height Std Dev : " + HgtSigma.ToString("F4"));
        sb.AppendLine("* Base Station ID: " + BaseId);
        sb.AppendLine("* Diff Age       : " + DiffAge.ToString("F2"));
        sb.AppendLine("* Solution Age   : " + SolutionAge.ToString("F2"));
        sb.AppendLine("* Sat Tracked    : " + NumSatsTracked);
        sb.AppendLine("* Sat Used       : " + NumSatsInSolution);
        sb.AppendLine("* Sat L1E1B1 Used: " + NumSatsWithL1E1B1);
        sb.AppendLine("* Sat MultiF Used: " + NumSatsMultiFreq);

        if (PositionType == PositionType.SINGLE) //if the receiver have a PDP solution
            if (IsFlagActive(ExtSolutionStatus, ExtendedSolutionStatus.Glide))
                sb.AppendLine("* Single Solution: GLIDE");
        sb.AppendLine("* Klobuchar Model: " + IsFlagActive(ExtSolutionStatus, ExtendedSolutionStatus.IonoCorrKlobuchar).ToString());
        sb.AppendLine("* SBAS Broadcast : " + IsFlagActive(ExtSolutionStatus, ExtendedSolutionStatus.IonoCorrSBAS).ToString());
        sb.AppendLine("* Multi-Freq Comp: " + IsFlagActive(ExtSolutionStatus, ExtendedSolutionStatus.IonoCorrMultiFreq).ToString());
        sb.AppendLine("* PSRDiff Correct: " + IsFlagActive(ExtSolutionStatus, ExtendedSolutionStatus.IonoCorrPSRDIFF).ToString());
        sb.AppendLine("* NovAtel Iono   : " + IsFlagActive(ExtSolutionStatus, ExtendedSolutionStatus.IonoCorrNovatelIono).ToString());
        sb.AppendLine("* Antenna Warning: " + IsFlagActive(ExtSolutionStatus, ExtendedSolutionStatus.AntennaWarning).ToString());

        sb.AppendLine("* Galileo E1     : " + IsFlagActive(GalileoSigMask, SignalsUsedMask.E1_Used).ToString());
        sb.AppendLine("* BeiDou B1      : " + IsFlagActive(GalileoSigMask, SignalsUsedMask.BEIDOU_B1_Used).ToString());
        sb.AppendLine("* BeiDou B2      : " + IsFlagActive(GalileoSigMask, SignalsUsedMask.BEIDOU_B2_Used).ToString());

        sb.AppendLine("* GPS L1         : " + IsFlagActive(GpsSigMask, SignalsUsedMask.GPS_L1_Used).ToString());
        sb.AppendLine("* GPS L2         : " + IsFlagActive(GpsSigMask, SignalsUsedMask.GPS_L2_Used).ToString());
        sb.AppendLine("* GPS L5         : " + IsFlagActive(GpsSigMask, SignalsUsedMask.GPS_L5_Used).ToString());
        sb.AppendLine("* Glonass L1     : " + IsFlagActive(GpsSigMask, SignalsUsedMask.GLO_L1_Used).ToString());
        sb.AppendLine("* Glonass L2     : " + IsFlagActive(GpsSigMask, SignalsUsedMask.GLO_L2_Used).ToString());

        return sb.ToString();
    }
    public static Bestpos Decode(byte[] message, int logType, int h)
    {
        Bestpos val = new();
        val.LogType = logType;
        int solnStatus = BitConverter.ToInt32(message, h);
        val.SolutionStatus = (SolutionStatus)solnStatus;
        int posType = BitConverter.ToInt32(message, h + 4);
        val.PositionType = (PositionType)posType;
        val.Lat = BitConverter.ToDouble(message, h + 8);
        val.Lon = BitConverter.ToDouble(message, h + 16);
        val.Hgt = BitConverter.ToDouble(message, h + 24);
        val.Undulation = BitConverter.ToSingle(message, h + 32);
        val.Datum = BitConverter.ToInt32(message, h + 36);
        val.LatSigma = BitConverter.ToSingle(message, h + 40);
        val.LonSigma = BitConverter.ToSingle(message, h + 44);
        val.HgtSigma = BitConverter.ToSingle(message, h + 48);
        var baseId = new string(Encoding.ASCII.GetChars(message, h + 52, 4));
        if (baseId.Contains('\0'))
            baseId = baseId.Substring(0, baseId.IndexOf('\0'));
        val.BaseId = baseId;
        val.DiffAge = BitConverter.ToSingle(message, h + 56);
        val.SolutionAge = BitConverter.ToSingle(message, h + 60);
        val.NumSatsTracked = message[h + 64];
        val.NumSatsInSolution = message[h + 65];
        val.NumSatsWithL1E1B1 = message[h + 66];
        val.NumSatsMultiFreq = message[h + 67];
        val.ExtSolutionStatus = message[h + 69];
        val.GalileoSigMask = message[h + 70];
        val.GpsSigMask = message[h + 71];

        return val;
    }
    public static bool IsFlagActive(byte b, object e)
    {
        int flag = (int)e;
        return ((b & flag) == flag);
    }
}

public enum SignalsUsedMask
{
    GPS_L1_Used = 0x01,  // L1
    GPS_L2_Used = 0x02,  // L2
    GPS_L5_Used = 0x04,  // L5
    GLO_L1_Used = 0x10,  // G1
    GLO_L2_Used = 0x20,  // G2

    E1_Used = 0x0001,  // Galileo E1

    BEIDOU_B1_Used = 0x10,  // Beidou B1
    BEIDOU_B2_Used = 0x20,  // Beidou B2
};
