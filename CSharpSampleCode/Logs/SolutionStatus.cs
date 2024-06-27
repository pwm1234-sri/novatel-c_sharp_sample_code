using System.ComponentModel.DataAnnotations;

namespace CSharpSampleCode.Logs;

public enum SolutionStatus
{
    [Display(Name = "Solution status not set")]
    SOLN_STATUS_NOT_SET = -2,
    SOLN_STATUS_MIN = -1,
    [Display(Name = "Solution computed")]
    SOLN_STATUS_SOLUTION_COMPUTED = 0,
    [Display(Name = "Insufficient observations")]
    SOLN_STATUS_INSUFFICIENT_OBS = 1,
    [Display(Name = "No convergence")]
    SOLN_STATUS_NO_CONVERGENCE = 2,
    [Display(Name = "Singular AtPA matrix")]
    SOLN_STATUS_SINGULAR_AtPA_MATRIX = 3,
    [Display(Name = "Covariance trace exceeds maximum (trace > 1000 m)")]
    SOLN_STATUS_BIG_COVARIANCE_TRACE = 4,
    [Display(Name = "Test distance exceeded (maximum of 3 rejections if distance > 10 km)")]
    SOLN_STATUS_BIG_TEST_DISTANCE = 5,
    [Display(Name = "Converging from cold start")]
    SOLN_STATUS_COLD_START = 6,
    [Display(Name = "CoCom limits exceeded")]
    SOLN_STATUS_SPEED_OR_HEIGHT_LIMITS_EXCEEDED = 7,
    [Display(Name = "Variance exceeds limits")]
    SOLN_STATUS_VARIANCE_EXCEEDS_LIMIT = 8,
    [Display(Name = "Residuals are too large")]
    SOLN_STATUS_RESIDUALS_ARE_TOO_LARGE = 9,
    [Display(Name = "Delta position is too large")]
    SOLN_STATUS_DELTA_POSITION_IS_TOO_LARGE = 10,
    [Display(Name = "Negative variance")]
    SOLN_STATUS_NEGATIVE_VARIANCE = 11,
    [Display(Name = "The position is old")]
    SOLN_STATUS_OLD_SOLUTION = 12,
    [Display(Name = "Integrity warning")]
    SOLN_STATUS_INTEGRITY_WARNING = 13,
    [Display(Name = "INS has not started yet")]
    SOLN_STATUS_INS_INACTIVE = 14,
    [Display(Name = "INS doing its coarse alignment")]
    SOLN_STATUS_INS_ALIGNING = 15,
    [Display(Name = "INS position is bad")]
    SOLN_STATUS_INS_BAD = 16,
    [Display(Name = "No IMU detected")]
    SOLN_STATUS_IMU_UNPLUGGED = 17,
    [Display(Name = "Not enough satellites to verify FIX POSITION")]
    SOLN_STATUS_PENDING = 18,
    [Display(Name = "Fixed position is not valid")]
    SOLN_STATUS_INVALID_FIX = 19,
    [Display(Name = "Position type (HP or XP) not authorized")]
    SOLN_STATUS_UNAUTHORIZED = 20,
    [Display(Name = "Selected RTK antenna mode not possible")]
    SOLN_STATUS_ANTENNA_WARNING = 21,
    [Display(Name = "Logging rate not supported for this solution type")]
    SOLN_STATUS_INVALID_RATE = 22,

    SOLN_STATUS_MAX
}

public enum ExtendedSolutionStatus
{
    Glide = 0x01,

    IonoCorrKlobuchar = 0x02, // These bits are considered as part of the IonoCorrMask
    IonoCorrSBAS = 0x04, // These bits are considered as part of the IonoCorrMask
    IonoCorrMultiFreq = 0x06, // These bits are considered as part of the IonoCorrMask
    IonoCorrPSRDIFF = 0x08, // These bits are considered as part of the IonoCorrMask
    IonoCorrNovatelIono = 0x0A, // These bits are considered as part of the IonoCorrMask

    AntennaWarning = 0x20,
}
