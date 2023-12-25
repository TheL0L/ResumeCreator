using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResumeCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            QuestPDF.Settings.License = LicenseType.Community;


            bool flag = true;
            while (flag)
            {
                Console.WriteLine("Select an option to procced:");
                Console.WriteLine("1. Generate an example json & pdf  [English]");
                Console.WriteLine("2. Generate an example json & pdf  [Hebrew]");
                Console.WriteLine("");
                Console.WriteLine("3. Generate a pdf from existing json");
                var input = Console.ReadKey().KeyChar;
                Console.Clear();

                switch (input)
                {
                    case '1':
                        {
                            var exampleModel = GenerateExampleModelEng();
                            SaveModelJson("example_eng.json", exampleModel, new Layout());

                            GeneratePdf(exampleModel, "example_eng.pdf");

                            flag = false;
                            break;
                        }
                    case '2':
                        {
                            var exampleModel = GenerateExampleModelHe();
                            var exampleLayout = GenerateExampleLayoutHe();
                            SaveModelJson("example_he.json", exampleModel, exampleLayout);

                            Layout.Instance = exampleLayout;
                            GeneratePdf(exampleModel, "example_he.pdf");

                            flag = false;
                            break;
                        }
                    case '3':
                        {
                            Console.Write("Enter the full path to the .json file: ");
                            var path = Console.ReadLine();

                            if (File.Exists(path))
                            {
                                var model = ReadModelJson(path);
                                GeneratePdf(model);
                                flag = false;
                            }
                            else Console.WriteLine("Provided file is inaccessible or doesn't exist.");

                            break;
                        }
                }
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static ResumeModel GenerateExampleModelEng()
        {
            return new ResumeModel
            {
                FullName = "Full Name",
                PersonalInfo = new List<FeatureItem>
                {
                    new FeatureItem
                    {
                        Text = "+000 00 000 0000",
                        Icon = "Icons/phone.svg"
                    },
                    new FeatureItem
                    {
                        Text = "email@domain.com",
                        HyperLink = "mailto:email@domain.com",
                        Icon = "Icons/email.svg"
                    },
                    new FeatureItem
                    {
                        Text = "github",
                        HyperLink = "https://github.com/",
                        Icon = "Icons/github.svg"
                    }
                },
                Languages = new List<string>
                {
                    "English",
                    "Hebrew"
                },
                Education = new List<EducationDetails>
                {
                    new EducationDetails
                    {
                        Major = "Degree or Major",
                        Facility = "Facility",
                        DateSpan = "Jan 2000 - Dec 2004",
                    },
                },
                ExtraCourses = new List<string>
                {
                    "Course 1",
                    "Course 2",
                    "Course 3",
                },

                AboutMe = "Here goes the about me section.",
                WorkExperience = new List<WorkExperienceDetails>
                {
                    new WorkExperienceDetails
                    {
                        Position = "Position",
                        Company = "Company",
                        JobType = "Job Type Freelance/Fulltime/etc.",
                        DescriptionItems = new List<string>
                        {
                            "Some details about the position.",
                        },
                    },
                },
                MilitaryService = new List<MilitaryServiceDetails>
                {
                    new MilitaryServiceDetails
                    {
                        Position = "Role",
                        Branch = "Military Branch",
                        DateSpan = "Recruitment date - Release Date",
                        DescriptionItems = new List<string>
                        {
                            "Some details about the service period.",
                        }
                    }
                },
            };
        }

        static ResumeModel GenerateExampleModelHe()
        {
            return new ResumeModel
            {
                FullName = "שם מלא",
                PersonalInfo = new List<FeatureItem>
                {
                    new FeatureItem
                    {
                        Text = "+000 00 000 0000",
                        Icon = "Icons/phone.svg"
                    },
                    new FeatureItem
                    {
                        Text = "email@domain.com",
                        HyperLink = "mailto:email@domain.com",
                        Icon = "Icons/email.svg"
                    },
                    new FeatureItem
                    {
                        Text = "github",
                        HyperLink = "https://github.com/",
                        Icon = "Icons/github.svg"
                    }
                },
                Languages = new List<string>
                {
                    "אנגלית",
                    "עברית"
                },
                Education = new List<EducationDetails>
                {
                    new EducationDetails
                    {
                        Major = "תואר או התמחות",
                        Facility = "מוסד",
                        DateSpan = "01.2000 - 12.2004",
                    },
                },
                ExtraCourses = new List<string>
                {
                    "קורס 1",
                    "קורס 2",
                    "קורס 3",
                },

                AboutMe = "תקציר אישי נמצא כאן.",
                WorkExperience = new List<WorkExperienceDetails>
                {
                    new WorkExperienceDetails
                    {
                        Position = "משרה",
                        Company = "חברה",
                        JobType = "סוג משרה",
                        DescriptionItems = new List<string>
                        {
                            "פרטים על המשרה.",
                        },
                    },
                },
                MilitaryService = new List<MilitaryServiceDetails>
                {
                    new MilitaryServiceDetails
                    {
                        Position = "תפקיד",
                        Branch = "ענף צבאי",
                        DateSpan = "תאריך גיוס - תאריך שחרור",
                        DescriptionItems = new List<string>
                        {
                            "פרטים על תקופת השירות.",
                        }
                    }
                },
            };
        }

        static Layout GenerateExampleLayoutHe()
        {
            return new Layout
            {
                TextDirection = TextDirection.RightToLeft,
                Headers = {
                    ["Personal Details"] = "מידע אישי",
                    ["Languages"] = "שפות",
                    ["Education"] = "השכלה",
                    ["Courses"] = "קורסים",
                    ["About Me"] = "תקציר",
                    ["Working Experiences"] = "ניסיון תעסוקתי",
                    ["Military Service"] = "שירות צבאי"
                }
            };
        }

        static ResumeModel ReadModelJson(string path)
        {
            // Setup the serializer options
            var SerializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            // Read the JSON string from the file
            string jsonString = File.ReadAllText(path);

            // Deserialize the JSON string back into an object
            JsonContainer? data = JsonSerializer.Deserialize<JsonContainer>(jsonString, SerializerOptions);

            if (data?.LayoutData != null )
            {
                Layout.Instance = data.LayoutData;
            }
            
            return data?.ResumeModelData;
        }

        static void SaveModelJson(string path, ResumeModel model, Layout layout)
        {
            // Setup the serializer options
            var SerializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            // Create an anonymous object with the specified root name
            var rootObject = new JsonContainer { LayoutData = layout, ResumeModelData = model };

            // Serialize the object to a JSON string
            string jsonString = JsonSerializer.Serialize(rootObject, SerializerOptions);

            // Write the JSON string to a file
            File.WriteAllText(path, jsonString);
        }

        static void GeneratePdf(ResumeModel model, string? path = null)
        {
            var now = DateTime.Now.ToString("dd_MMM_yyyy__hh_mm");
            var filePath = string.IsNullOrEmpty(path) ? $"resume_{now}.pdf" : path;

            var document = new ResumeDocument(model);
            document.GeneratePdf(filePath);
        }
    }

    public class JsonContainer
    {
        public Layout LayoutData { get; set; }
        public ResumeModel ResumeModelData { get; set; }
    }
}