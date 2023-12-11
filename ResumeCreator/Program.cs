﻿using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System.Diagnostics;
using System.Text.Json;

namespace ResumeCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var exampleModel = GenerateExampleJson();
            SaveModelJson("example.json", exampleModel);

            GeneratePdf(exampleModel);
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
                        Text = "+000 00 000 0000"
                    },
                    new FeatureItem
                    {
                        Text = "email@domain.com",
                        HyperLink = "mailto:email@domain.com"
                    },
                    new FeatureItem
                    {
                        Text = "github",
                        HyperLink = "https://github.com/"
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
                        StartDate = new MonthYear(1, 2000),
                        EndDate = new MonthYear(1, 2004),
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
                        StartDate = new MonthYear(1, 2000),
                        EndDate = new MonthYear(1, 2003),
                        Description = new List<string>
                        {
                            "Some details about the service period.",
                        }
                    }
                },
            };
        }

        static ResumeModel ReadModelJson(string path)
        {
            // Read the JSON string from the file
            string jsonString = File.ReadAllText(path);

            // Deserialize the JSON string back into an object
            ResumeModel model = JsonSerializer.Deserialize<ResumeModel>(jsonString);

            return model;
        }

        static void SaveModelJson(string path, ResumeModel model)
        {
            // Serialize the object to a JSON string
            string jsonString = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });

            // Write the JSON string to a file
            File.WriteAllText(path, jsonString);
        }

        static void GeneratePdf(ResumeModel model)
        {
            var now = DateTime.Now.ToString("dd_MMM_yyyy__hh_mm");
            var filePath = $"resume_{now}.pdf";

            var document = new ResumeDocument(model);
            document.GeneratePdf(filePath);

            Process.Start("explorer.exe", filePath);
        }
    }
}