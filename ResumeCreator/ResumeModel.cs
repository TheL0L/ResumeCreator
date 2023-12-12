
namespace ResumeCreator
{
    public enum TextDirection
    {
        LeftToRight, RightToLeft
    }

    public class DocumentColors
    {
        public string Header = "35373d";
        public string BodyDetails = "e9e2dd";
        public string BodyMain = "ffffff";
    }

    public class DocumentProportions
    {
        public float BodyDetails = 1;
        public float BodyMain = 2;
    }


    public class ResumeModel
    {
        public TextDirection TextDirection = TextDirection.LeftToRight;
        public string FontFamily { get; set; } = "Arial";  // "Rubik" for hebrew
        public int FontSizeDefault { get; set; } = 10;
        public int FontSizeBlockSubTitle { get; set; } = 8;
        public int FontSizeBlockTitle { get; set; } = 12;
        public int FontSizeBlockMajorTitle { get; set; } = 14;

        public DocumentColors BackgroundColors { get; set; } = new DocumentColors();
        public DocumentProportions Proportions { get; set; } = new DocumentProportions();

        public string FullName { get; set; } = string.Empty;

        public List<FeatureItem> PersonalInfo { get; set; } = new List<FeatureItem>();
        public List<string> Languages { get; set; } = new List<string>();
        public List<EducationDetails> Education { get; set; } = new List<EducationDetails>();
        public List<string> ExtraCourses { get; set; } = new List<string>();

        public string AboutMe { get; set; } = string.Empty;
        public List<WorkExperienceDetails> WorkExperience { get; set; } = new List<WorkExperienceDetails>();
        public List<MilitaryServiceDetails> MilitaryService { get; set; } = new List<MilitaryServiceDetails>();

    }


    public class MonthYear
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public MonthYear(int month, int year)
        {
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
            }

            if (year < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(year), "Year must be a positive value.");
            }

            Month = month;
            Year = year;
        }

        public static MonthYear FromDateTime(DateTime dateTime)
        {
            return new MonthYear(dateTime.Month, dateTime.Year);
        }

        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, 1);
        }

        public override string ToString()
        {
            return ToDateTime().ToString("MMM yyyy");
        }

        public string ToString(string format)
        {
            return ToDateTime().ToString(format);
        }
    }

    public class FeatureItem
    {
        public string Text { get; set; } = string.Empty;
        public string? HyperLink { get; set; }
        public string? Icon { get; set; }
    }

    public class WorkExperienceDetails
    {
        public string Position { get; set; } = string.Empty;
        public string? Company { get; set; }
        public string? JobType { get; set; }
        public MonthYear? StartDate { get; set; }
        public MonthYear? EndDate { get; set; }
        public List<string> DescriptionItems { get; set; } = new List<string>();

        public string GetHeader()
        {
            string result = Position;

            if (!string.IsNullOrEmpty(Company)) result += $", {Company}";
            if (!string.IsNullOrEmpty(JobType)) result += $" ({JobType})";

            return result;
        }

        public string GetDate()
        {
            string result = string.Empty;

            if (StartDate != null) result += $"{StartDate}";
            if (EndDate != null) result += $" - {EndDate}";

            return result;
        }
    }

    public class MilitaryServiceDetails
    {
        public string Position { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public MonthYear? StartDate { get; set; }
        public MonthYear? EndDate { get; set; }
        public List<string> Description { get; set; } = new List<string>();

        public string GetHeader()
        {
            return $"{Position} at {Branch}";
        }

        public string GetDate()
        {
            string result = string.Empty;

            if (StartDate != null) result += StartDate;
            if (EndDate != null) result += $" - {EndDate}";

            return result;
        }
    }

    public class EducationDetails
    {
        public string Major { get; set; } = string.Empty;
        public string? Facility { get; set; }
        public MonthYear? StartDate { get; set; }
        public MonthYear? EndDate { get; set; }

        public string GetDate()
        {
            string result = string.Empty;

            if (StartDate != null) result += StartDate;
            if (EndDate != null) result += $" - {EndDate}";

            return result;
        }
    }
}
