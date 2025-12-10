namespace IntelliGrade.App.Utilities;

/// <summary>
/// Provides grade calculation utilities following BYU-Idaho grading scale.
/// </summary>
public static class GradeCalculator
{
    // Grade thresholds (BYU-Idaho standard)
    private const decimal GradeA = 93m;
    private const decimal GradeAMinus = 90m;
    private const decimal GradeBPlus = 87m;
    private const decimal GradeB = 83m;
    private const decimal GradeBMinus = 80m;
    private const decimal GradeCPlus = 77m;
    private const decimal GradeC = 73m;
    private const decimal GradeCMinus = 70m;
    private const decimal GradeDPlus = 67m;
    private const decimal GradeD = 63m;
    private const decimal GradeDMinus = 60m;

    /// <summary>
    /// Calculates letter grade from percentage (0-100).
    /// </summary>
    /// <param name="percentage">Percentage score (0-100)</param>
    /// <returns>Letter grade (A, A-, B+, B, etc.)</returns>
    public static string CalculateLetterGrade(double percentage)
    {
        return CalculateLetterGrade((decimal)percentage);
    }

    /// <summary>
    /// Calculates letter grade from percentage (0-100).
    /// </summary>
    /// <param name="percentage">Percentage score (0-100)</param>
    /// <returns>Letter grade (A, A-, B+, B, etc.)</returns>
    public static string CalculateLetterGrade(decimal percentage)
    {
        if (percentage < 0)
            return "-";

        return percentage switch
        {
            >= GradeA => "A",
            >= GradeAMinus => "A-",
            >= GradeBPlus => "B+",
            >= GradeB => "B",
            >= GradeBMinus => "B-",
            >= GradeCPlus => "C+",
            >= GradeC => "C",
            >= GradeCMinus => "C-",
            >= GradeDPlus => "D+",
            >= GradeD => "D",
            >= GradeDMinus => "D-",
            _ => "F"
        };
    }

    /// <summary>
    /// Calculates letter grade from nullable percentage.
    /// </summary>
    /// <param name="percentage">Nullable percentage score</param>
    /// <returns>Letter grade or "-" if null/invalid</returns>
    public static string CalculateLetterGrade(decimal? percentage)
    {
        return percentage.HasValue ? CalculateLetterGrade(percentage.Value) : "-";
    }
}
