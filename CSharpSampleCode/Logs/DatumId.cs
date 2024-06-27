namespace CSharpSampleCode.Logs;

public enum DatumId
{
    UNKNOWN_DATUM = -1,
    ADIND = 1,
    ARC50,
    ARC60,
    AGD66,
    AGD84,
    BUKIT,
    ASTRO,
    CHATM,
    CARTH,
    CAPE,
    DJAKA,
    EGYPT,
    ED50,
    ED79,
    GUNSG,
    GEO49,
    GRB36,
    GUAM,
    HAWAII,
    KAUAI,
    MAUI,
    OAHU,
    HERAT,
    HJORS,
    HONGK,
    HUTZU,
    INDIA,
    IRE65,
    KERTA,
    KANDA,
    LIBER,
    LUZON,
    MINDA,
    MERCH,
    NAHR,
    NAD83,
    CANADA,
    ALASKA,
    NAD27,
    CARIBB,
    MEXICO,
    CAMER,
    MINNA,
    OMAN,
    PUERTO,
    QORNO,
    ROME,
    CHUA,
    SAM56,
    SAM69,
    CAMPO,
    SACOR,
    YACAR,
    TANAN,
    TIMBA,
    TOKYO,
    TRIST,
    VITI,
    WAK60,
    WGS72,
    WGS84,
    ZANDE,
    USER,
    CSRS,
    ADIM,
    ARSM,
    ENW,
    HTN,
    INDB,
    INDI,
    IRL,
    LUZA,
    LUZB,
    NAHC,
    NASP,
    OGMB,
    OHAA,
    OHAB,
    OHAC,
    OHAD,
    OHIA,
    OHIB,
    OHIC,
    OHID,
    TIL,
    TOYM
}

public static class DatumIdExtensions
{
    public static string GetDatumString(this int datum)
    {
        switch ((DatumId)datum)
        {
            case DatumId.UNKNOWN_DATUM:
                return "UNKNOWN_DATUM";
            case DatumId.ADIND:
                return "ADIND";
            case DatumId.ARC50:
                return "ARC50";
            case DatumId.ARC60:
                return "ARC60";
            case DatumId.AGD66:
                return "AGD66";
            case DatumId.AGD84:
                return "AGD84";
            case DatumId.BUKIT:
                return "BUKIT";
            case DatumId.ASTRO:
                return "ASTRO";
            case DatumId.CHATM:
                return "CHATM";
            case DatumId.CARTH:
                return "CARTH";
            case DatumId.CAPE:
                return "CAPE";
            case DatumId.DJAKA:
                return "DJAKA";
            case DatumId.EGYPT:
                return "EGYPT";
            case DatumId.ED50:
                return "ED50";
            case DatumId.ED79:
                return "ED79";
            case DatumId.GUNSG:
                return "GUNSG";
            case DatumId.GEO49:
                return "GEO49";
            case DatumId.GRB36:
                return "GRB36";
            case DatumId.GUAM:
                return "GUAM";
            case DatumId.HAWAII:
                return "HAWAII";
            case DatumId.KAUAI:
                return "KAUAI";
            case DatumId.MAUI:
                return "MAUI";
            case DatumId.OAHU:
                return "OAHU";
            case DatumId.HERAT:
                return "HERAT";
            case DatumId.HJORS:
                return "HJORS";
            case DatumId.HONGK:
                return "HONGK";
            case DatumId.HUTZU:
                return "HUTZU";
            case DatumId.INDIA:
                return "INDIA";
            case DatumId.IRE65:
                return "IRE65";
            case DatumId.KERTA:
                return "KERTA";
            case DatumId.KANDA:
                return "KANDA";
            case DatumId.LIBER:
                return "LIBER";
            case DatumId.LUZON:
                return "LUZON";
            case DatumId.MINDA:
                return "MINDA";
            case DatumId.MERCH:
                return "MERCH";
            case DatumId.NAHR:
                return "NAHR";
            case DatumId.NAD83:
                return "NAD83";
            case DatumId.CANADA:
                return "CANADA";
            case DatumId.ALASKA:
                return "ALASKA";
            case DatumId.NAD27:
                return "NAD27";
            case DatumId.CARIBB:
                return "CARIBB";
            case DatumId.MEXICO:
                return "MEXICO";
            case DatumId.CAMER:
                return "CAMER";
            case DatumId.MINNA:
                return "MINNA";
            case DatumId.OMAN:
                return "OMAN";
            case DatumId.PUERTO:
                return "PUERTO";
            case DatumId.QORNO:
                return "QORNO";
            case DatumId.ROME:
                return "ROME";
            case DatumId.CHUA:
                return "CHUA";
            case DatumId.SAM56:
                return "SAM56";
            case DatumId.SAM69:
                return "SAM69";
            case DatumId.CAMPO:
                return "CAMPO";
            case DatumId.SACOR:
                return "SACOR";
            case DatumId.YACAR:
                return "YACAR";
            case DatumId.TANAN:
                return "TANAN";
            case DatumId.TIMBA:
                return "TIMBA";
            case DatumId.TOKYO:
                return "TOKYO";
            case DatumId.TRIST:
                return "TRIST";
            case DatumId.VITI:
                return "VITI";
            case DatumId.WAK60:
                return "WAK60";
            case DatumId.WGS72:
                return "WGS72";
            case DatumId.WGS84:
                return "WGS84";
            case DatumId.ZANDE:
                return "ZANDE";
            case DatumId.USER:
                return "USER";
            case DatumId.CSRS:
                return "CSRS";
            case DatumId.ADIM:
                return "ADIM";
            case DatumId.ARSM:
                return "ARSM";
            case DatumId.ENW:
                return "ENW";
            case DatumId.HTN:
                return "HTN";
            case DatumId.INDB:
                return "INDB";
            case DatumId.INDI:
                return "INDI";
            case DatumId.IRL:
                return "IRL";
            case DatumId.LUZA:
                return "LUZA";
            case DatumId.LUZB:
                return "LUZB";
            case DatumId.NAHC:
                return "NAHC";
            case DatumId.NASP:
                return "NASP";
            case DatumId.OGMB:
                return "OGMB";
            case DatumId.OHAA:
                return "OHAA";
            case DatumId.OHAB:
                return "OHAB";
            case DatumId.OHAC:
                return "OHAC";
            case DatumId.OHAD:
                return "OHAD";
            case DatumId.OHIA:
                return "OHIA";
            case DatumId.OHIB:
                return "OHIB";
            case DatumId.OHIC:
                return "OHIC";
            case DatumId.OHID:
                return "OHID";
            case DatumId.TIL:
                return "TIL";
            case DatumId.TOYM:
                return "TOYM";
        }
        return "UNKNOWN DATUM";
    }
}