
namespace ResumeCreator
{
    public enum TextDirection
    {
        LeftToRight, RightToLeft
    }

    public class DocumentColors
    {
        public string Header { get; set; } = "35373d";
        public string BodyDetails { get; set; } = "e9e2dd";
        public string BodyMain { get; set; } = "ffffff";
    }

    public class DocumentProportions
    {
        public float BodyDetails { get; set; } = 1;
        public float BodyMain { get; set; } = 2;
    }


    public class ResumeModel
    {
        public string FontFamily { get; set; } = "Arial";
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
        public string? DateSpan { get; set; }
        public List<string> DescriptionItems { get; set; } = new List<string>();

        public string GetHeader()
        {
            string result = Position;

            if (!string.IsNullOrEmpty(Company)) result += $", {Company}";
            if (!string.IsNullOrEmpty(JobType)) result += $" ({JobType})";

            return result;
        }
    }

    public class MilitaryServiceDetails
    {
        public string Position { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string? DateSpan { get; set; }
        public List<string> DescriptionItems { get; set; } = new List<string>();

        public string GetHeader()
        {
            return $"{Position} at {Branch}";
        }
    }

    public class EducationDetails
    {
        public string Major { get; set; } = string.Empty;
        public string? Facility { get; set; }
        public string? DateSpan { get; set; }
    }
}
