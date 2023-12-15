﻿using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
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
                Console.WriteLine("1. Generate an example json & pdf");
                Console.WriteLine("2. Generate a pdf from existing json");
                var input = Console.ReadKey().KeyChar;
                Console.Clear();

                switch (input)
                {
                    case '1':
                        {
                            var exampleModel = GenerateExampleJson();
                            SaveModelJson("example.json", exampleModel);

                            GeneratePdf(exampleModel);

                            flag = false;
                            break;
                        }
                    case '2':
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

        static ResumeModel GenerateExampleJson()
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
                },
                Education = new List<EducationDetails>
                {
                    new EducationDetails
                    {
                        Major = "Degree of Some Major",
                        Facility = "Some Facility",
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
                        Position = "Developer",
                        Company = "Some Place",
                        JobType = "Freelance",
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
                        Position = "Technician",
                        Branch = "Tech Corps",
                        DateSpan = "Jan 2000 - Dec 2003",
                        DescriptionItems = new List<string>
                        {
                            "Some details about the service period.",
                        }
                    }
                },
            };
        }

        static ResumeModel ReadModelJson(string path)
        {
            // Setup the serializer options
            var SerializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
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

        static void SaveModelJson(string path, ResumeModel model)
        {
            // Setup the serializer options
            var SerializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            // Create an anonymous object with the specified root name
            var rootObject = new JsonContainer { LayoutData = Layout.Instance, ResumeModelData = model };

            // Serialize the object to a JSON string
            string jsonString = JsonSerializer.Serialize(rootObject, SerializerOptions);

            // Write the JSON string to a file
            File.WriteAllText(path, jsonString);
        }

        static void GeneratePdf(ResumeModel model)
        {
            var now = DateTime.Now.ToString("dd_MMM_yyyy__hh_mm");
            var filePath = $"resume_{now}.pdf";

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