using System;

namespace CSharpSampleCode.Logs;

public enum PositionType
{
    POSTYPE_NOTSET = -1,
    NONE = 0,
    FIXEDPOS = 1,
    FIXEDHEIGHT = 2,
    FIXEDVEL = 3,
    DOPPLER_VELOCITY = 8,
    SINGLE = 16,
    PSRDIFF = 17,
    WAAS = 18,
    PROPAGATED = 19,
    OMNISTAR = 20,
    L1_FLOAT = 32,
    IONOFREE_FLOAT = 33,
    NARROW_FLOAT = 34,
    L1_INT = 48,
    WIDE_INT = 49,
    NARROW_INT = 50,
    RTK_DIRECT_INS = 51,
    INS = 52,
    INS_PSRSP = 53,
    INS_PSRDIFF = 54,
    INS_RTKFLOAT = 55,
    INS_RTKFIXED = 56,
    INS_OMNISTAR = 57,
    INS_OMNISTAR_HP = 58,
    INS_OMNISTAR_XP = 59,
    OMNISTAR_HP = 64,
    OMNISTAR_XP = 65,
    CDGPS = 66,
    EXT_CONSTRAINED = 67,
    PPP_CONVERGING = 68,
    PPP = 69,
    OPERATIONAL = 70,
    WARNING = 71,
    OUT_OF_BOUNDS = 72,
    INS_PPP_CONVERGING = 73,
    INS_PPP = 74,
    PPP_BASIC_CONVERGING = 77,
    PPP_BASIC = 78,
    INS_PPP_BASIC_CONVERGING = 79,
    INS_PPP_BASIC = 80,
    MAX_POSTYPE
}

public static class PositionTypeExtensions
{
    public static string GetPosTypeString(this int posType)
    {
        string cpRetValue = "UNKNOWN";

        switch ((PositionType)posType)
        {
            case PositionType.POSTYPE_NOTSET: cpRetValue = "NOTSET"; break;
            case PositionType.NONE: cpRetValue = "NONE"; break;
            case PositionType.FIXEDPOS: cpRetValue = "FIXEDPOS"; break;
            case PositionType.FIXEDHEIGHT: cpRetValue = "FIXEDHEIGHT"; break;
            case PositionType.FIXEDVEL: cpRetValue = "FIXEDVEL"; break;
            case PositionType.DOPPLER_VELOCITY: cpRetValue = "DOPPLER_VELOCITY"; break;
            case PositionType.SINGLE: cpRetValue = "SINGLE"; break;
            case PositionType.PSRDIFF: cpRetValue = "PSRDIFF"; break;
            case PositionType.WAAS: cpRetValue = "WAAS"; break;
            case PositionType.PROPAGATED: cpRetValue = "PROPAGATED"; break;
            case PositionType.OMNISTAR: cpRetValue = "OMNISTAR"; break;
            case PositionType.L1_FLOAT: cpRetValue = "L1_FLOAT"; break;
            case PositionType.IONOFREE_FLOAT: cpRetValue = "IONOFREE_FLOAT"; break;
            case PositionType.NARROW_FLOAT: cpRetValue = "NARROW_FLOAT"; break;
            case PositionType.L1_INT: cpRetValue = "L1_INT"; break;
            case PositionType.WIDE_INT: cpRetValue = "WIDE_INT"; break;
            case PositionType.NARROW_INT: cpRetValue = "NARROW_INT"; break;
            case PositionType.RTK_DIRECT_INS: cpRetValue = "RTK_DIRECT_INS"; break;
            case PositionType.INS: cpRetValue = "INS"; break;
            case PositionType.INS_PSRSP: cpRetValue = "INS_PSRSP"; break;
            case PositionType.INS_PSRDIFF: cpRetValue = "INS_PSRDIFF"; break;
            case PositionType.INS_RTKFLOAT: cpRetValue = "INS_RTKFLOAT"; break;
            case PositionType.INS_OMNISTAR: cpRetValue = "INS_OMNISTAR"; break;
            case PositionType.INS_OMNISTAR_XP: cpRetValue = "INS_OMNISTAR_XP"; break;
            case PositionType.INS_OMNISTAR_HP: cpRetValue = "INS_OMNISTAR_HP"; break;
            case PositionType.INS_RTKFIXED: cpRetValue = "INS_RTKFIXED"; break;
            case PositionType.OMNISTAR_HP: cpRetValue = "OMNISTAR_HP"; break;
            case PositionType.OMNISTAR_XP: cpRetValue = "OMNISTAR_XP"; break;
            case PositionType.CDGPS: cpRetValue = "CDGPS"; break;
            case PositionType.EXT_CONSTRAINED: cpRetValue = "EXT_CONSTRAINED"; break;
            case PositionType.PPP_CONVERGING: cpRetValue = "PPP_CONVERGING"; break;
            case PositionType.PPP: cpRetValue = "PPP"; break;
            case PositionType.OPERATIONAL: cpRetValue = "OPERATIONAL"; break;
            case PositionType.WARNING: cpRetValue = "WARNING"; break;
            case PositionType.OUT_OF_BOUNDS: cpRetValue = "OUT_OF_BOUNDS"; break;
            case PositionType.INS_PPP_CONVERGING: cpRetValue = "INS_PPP_CONVERGING"; break;
            case PositionType.INS_PPP: cpRetValue = "INS_PPP"; break;
            case PositionType.PPP_BASIC: cpRetValue = "PPP_BASIC"; break;
            case PositionType.PPP_BASIC_CONVERGING: cpRetValue = "PPP_BASIC_CONVERGING"; break;
            case PositionType.INS_PPP_BASIC: cpRetValue = "INS_PPP_BASIC"; break;
            case PositionType.INS_PPP_BASIC_CONVERGING: cpRetValue = "INS_PPP_BASIC_CONVERGING"; break;
            default:
                break;   // Absolutely impossible to reach here
        }

        return cpRetValue;
    }
}