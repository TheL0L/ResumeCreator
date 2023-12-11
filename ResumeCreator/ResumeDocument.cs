using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ResumeCreator
{
    public class ResumeDocument : IDocument
    {
        // constants
        private const float _bottomHeaderBuffer = 0;  // extra height buffer under header, inside body blocks.
        private const float _blocksSpacing = 16;  // spacing between individual content blocks.
        private const float _pageSpacing = 0;  // gap between BodyDetails & BodyMain, affects header as well.
        private const float _blocksPadding_BodyDetails = 15;  // horizontal margins for BodyDetails elements.
        private const float _blocksPadding_BodyMain = 15;  // horizontal margins for BodyMain elements.


        // content blocks
        private PersonalBlock personalInfo;
        private ContentBlock languages;
        private EducationBlock education;

        private FreeTextBlock aboutMe;
        private List<ExperienceBlock> jobs = new List<ExperienceBlock>();
        private List<ExperienceBlock> military = new List<ExperienceBlock>();


        public ResumeModel Model { get; }

        public ResumeDocument(ResumeModel model)
        {
            Model = model;

            personalInfo = new PersonalBlock(model.FontSizeBlockMajorTitle, model.PersonalInfo);
            languages = new ContentBlock(model.FontSizeBlockMajorTitle, "Languages", model.Languages);

            education = new EducationBlock(model.FontSizeBlockMajorTitle, model.Education);

            aboutMe = new FreeTextBlock(model.FontSizeBlockMajorTitle, "Summary", model.AboutMe);

            foreach (var job in model.WorkExperience)
            {
                jobs.Add(new JobBlock(model.FontSizeBlockTitle, model.FontSizeBlockSubTitle, job));
            }

            foreach (var section in model.MilitaryService)
            {
                military.Add(new MilitaryBlock(model.FontSizeBlockTitle, model.FontSizeBlockSubTitle, section));
            }
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // page setup
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(Model.FontSizeDefault).FontFamily(Model.FontFamily));

                // page contents
                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

            });
        }

        // helpers
        private void ComposeHeader(IContainer container)
        {
            container.Background(Model.BackgroundColors.Header)
            .Height(2.5f, Unit.Centimetre)
            .Row(row =>
            {
                // define spacing to match the body layout
                row.Spacing(_pageSpacing);

                // define 2 segments that corespond to the body proportions:

                if (Model.TextDirection == TextDirection.LeftToRight)
                {
                    // above BodyDetails
                    row.RelativeItem(Model.Proportions.BodyDetails).AlignCenter().AlignMiddle()
                    .Text(Model.FullName).FontSize(26).FontColor(Colors.White);

                    // above BodyMain
                    row.RelativeItem(Model.Proportions.BodyMain);
                }
                else
                {
                    // above BodyMain
                    row.RelativeItem(Model.Proportions.BodyMain);

                    // above BodyDetails
                    row.RelativeItem(Model.Proportions.BodyDetails).AlignCenter().AlignMiddle()
                    .Text(Model.FullName).FontSize(26).FontColor(Colors.White);
                }
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Row(x =>
            {
                // define spacing to match the header layout
                x.Spacing(_pageSpacing);

                // define 2 segments that corespond to details & main text:

                if (Model.TextDirection == TextDirection.LeftToRight)
                {
                    x.RelativeItem(Model.Proportions.BodyDetails).Element(ComposeBodyDetails);
                    x.RelativeItem(Model.Proportions.BodyMain).Element(ComposeBodyMain);
                }
                else
                {
                    x.RelativeItem(Model.Proportions.BodyMain).Element(ComposeBodyMain);
                    x.RelativeItem(Model.Proportions.BodyDetails).Element(ComposeBodyDetails);
                }

            });
        }

        private void ComposeBodyDetails(IContainer container)
        {
            container.Background(Model.BackgroundColors.BodyDetails)
            .Column(column =>
            {
                // setup content blocks spacing
                column.Spacing(_blocksSpacing);

                // add extra height buffer under the header
                column.Item().Height(_bottomHeaderBuffer);

                // add content blocks:

                column.Item()
                .PaddingHorizontal(_blocksPadding_BodyDetails)
                .Component(personalInfo);

                column.Item()
                .PaddingHorizontal(_blocksPadding_BodyDetails)
                .Component(languages);

                column.Item()
                .PaddingHorizontal(_blocksPadding_BodyDetails)
                .Component(education);

                column.Item()
                .PaddingHorizontal(_blocksPadding_BodyDetails)
                .Element(ComposeExtraCourses);

            });
        }

        private void ComposeExtraCourses(IContainer container)
        {
            if (Model.ExtraCourses.Count == 0) return;

            var extraCourses = new ContentBlock(Model.FontSizeBlockMajorTitle, "Courses", Model.ExtraCourses);
            container.Component(extraCourses);
        }

        private void ComposeBodyMain(IContainer container)
        {
            container.Column(column =>
            {
                // setup content blocks spacing
                column.Spacing(_blocksSpacing);

                // add extra height buffer under the header
                column.Item().Height(_bottomHeaderBuffer);

                // add content blocks:

                column.Item()
                .PaddingHorizontal(_blocksPadding_BodyMain)
                .Component(aboutMe);

                column.Item()
                .PaddingHorizontal(_blocksPadding_BodyMain)
                .Element(ComposeWorkSection);

                column.Item()
                .PaddingHorizontal(_blocksPadding_BodyMain)
                .Element(ComposeMilitarySection);

            });
        }

        private void ComposeWorkSection(IContainer container)
        {
            var block = new ExperienceCollectionBlock(
                "Working Experience",
                Model.FontSizeBlockMajorTitle,
                jobs
            );

            container.Component(block);
        }

        private void ComposeMilitarySection(IContainer container)
        {
            var block = new ExperienceCollectionBlock(
                "Military Service",
                Model.FontSizeBlockMajorTitle,
                military
            );

            container.Component(block);
        }
    }

    public class FeatureItemComponent : IComponent
    {
        private FeatureItem Feature { get; }

        public FeatureItemComponent(FeatureItem item)
        {
            Feature = item;
        }

        public void Compose(IContainer container)
        {
            container.Row(row =>
            {
                if (Feature.Icon != null)
                {
                    row.ConstantItem(10).AlignMiddle().Image(Feature.Icon);
                }
                else
                {
                    row.ConstantItem(10);
                }

                if (Feature.HyperLink != null)
                {
                    row.RelativeItem().PaddingLeft(5)
                    .AlignMiddle()
                    .Hyperlink(Feature.HyperLink).Text(Feature.Text);
                }
                else
                {
                    row.RelativeItem().PaddingLeft(5)
                    .AlignMiddle()
                    .Text(Feature.Text);
                }
            });
        }
    }

    public class PersonalBlock : IComponent
    {
        private int TitleFontSize;
        private List<FeatureItem> Items { get; }

        public PersonalBlock(int titleFontSize, List<FeatureItem> items)
        {
            Items = items;
            TitleFontSize = titleFontSize;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(1).Text("Personal Info").FontSize(TitleFontSize);
                column.Item().Height(5);

                foreach (var item in Items)
                {
                    column.Item().Component(new FeatureItemComponent(item));
                }
            });
        }
    }

    public class EducationBlock : IComponent
    {
        private int TitleFontSize;
        private List<EducationDetails> Items { get; }

        public EducationBlock(int titleFontSize, List<EducationDetails> items)
        {
            Items = items;
            TitleFontSize = titleFontSize;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(1).Text("Education").FontSize(TitleFontSize);
                column.Item().Height(5);

                foreach (var item in Items)
                {
                    column.Item().PaddingLeft(3).Text(item.Major).SemiBold();

                    if (!string.IsNullOrEmpty(item.Facility))
                    {
                        column.Item().PaddingLeft(3).Text(item.Facility);
                    }

                    if (!string.IsNullOrEmpty(item.GetDate()))
                    {
                        column.Item().PaddingLeft(3).Text(item.GetDate());
                    }

                    column.Item().PaddingBottom(10);
                }
            });
        }
    }

    public class ContentBlock : IComponent
    {
        private int TitleFontSize;
        protected string Title { get; set; }
        private List<string> Items { get; }

        public ContentBlock(int titleFontSize, string title, List<string> items)
        {
            TitleFontSize = titleFontSize;
            Title = title;
            Items = items;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(1).Text(Title).FontSize(TitleFontSize);
                column.Item().Height(5);

                foreach (var item in Items)
                {
                    column.Item().PaddingLeft(3).Text(item);
                }
            });
        }
    }

    public class ExperienceBlock : IComponent
    {
        protected float TitleFontSize { get; }
        protected string? Title { get; set; }
        protected float SubTitleFontSize { get; }
        protected string? SubTitle { get; set; }
        protected List<string> Details { get; set; }

        public ExperienceBlock(float titleFontSize, float subTitleFontSize)
        {
            TitleFontSize = titleFontSize;
            SubTitleFontSize = subTitleFontSize;
            Details = new List<string>();
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                if (!string.IsNullOrEmpty(SubTitle))
                {
                    column.Item().Text(Title).FontSize(TitleFontSize);
                    column.Item().PaddingBottom(3).Text(SubTitle).FontSize(SubTitleFontSize).FontColor(Colors.Grey.Darken3);
                }
                else
                {
                    column.Item().PaddingBottom(3).Text(Title).FontSize(TitleFontSize);
                }

                foreach (var detail in Details)
                {
                    column.Item().Text($"• {detail}");
                }
            });
        }
    }

    public class JobBlock : ExperienceBlock
    {
        public JobBlock(float titleFontSize, float subTitleFontSize, WorkExperienceDetails data)
            : base(titleFontSize, subTitleFontSize)
        {
            Title = data.GetHeader();
            SubTitle = data.GetDate();
            Details = data.DescriptionItems;
        }
    }

    public class MilitaryBlock : ExperienceBlock
    {
        public MilitaryBlock(int titleFontSize, int subTitleFontSize, MilitaryServiceDetails data)
            : base(titleFontSize, subTitleFontSize)
        {
            Title = data.GetHeader();
            SubTitle = data.GetDate();
            Details = data.Description;
        }
    }

    public class ExperienceCollectionBlock : IComponent
    {
        private string Title { get; }
        private float TitleFontSize { get; }
        private List<ExperienceBlock> Items { get; }

        public ExperienceCollectionBlock(string title, float titleFontSize, List<ExperienceBlock> items)
        {
            Title = title;
            TitleFontSize = titleFontSize;
            Items = items;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(1).Text(Title).FontSize(TitleFontSize);

                foreach (var item in Items)
                {
                    column.Item().Padding(5).Component(item);
                }
            });
        }
    }

    public class FreeTextBlock : IComponent
    {
        private int TitleFontSize { get; }
        private string? Title { get; }
        private string Text { get; }

        public FreeTextBlock(int titleFontSize, string? title, string text)
        {
            TitleFontSize = titleFontSize;
            Title = title;
            Text = text;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                if (!string.IsNullOrEmpty(Text))
                {
                    column.Item().BorderBottom(1).Text(Title).FontSize(TitleFontSize);
                    column.Item().Height(5);
                }

                column.Item().PaddingLeft(3).Text(Text);
            });
        }
    }
}
