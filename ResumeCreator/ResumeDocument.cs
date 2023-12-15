using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Svg.Skia;

namespace ResumeCreator
{
    public static class SvgExtensions
    {
        public static void Svg(this IContainer container, SKSvg svg)
        {
            container
                .AlignCenter()
                .AlignMiddle()
                .ScaleToFit()
                .Width(svg.Picture.CullRect.Width)
                .Height(svg.Picture.CullRect.Height)
                .Canvas((canvas, space) => canvas.DrawPicture(svg.Picture));
        }
    }

    public class Layout
    {
        public static Layout Instance { get; set; } = new Layout();
        public TextDirection TextDirection { get; set; } = TextDirection.LeftToRight;
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>
        {
            ["Personal Details"] = "Personal Info",
            ["Languages"] = "Languages",
            ["Education"] = "Education",
            ["Courses"] = "Courses",
            ["About Me"] = "Summary",
            ["Working Experiences"] = "Working Experience",
            ["Military Service"] = "Military Service"
        };
    }

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
            languages = new ContentBlock(model.FontSizeBlockMajorTitle, Layout.Instance.Headers["Languages"], model.Languages);

            education = new EducationBlock(model.FontSizeBlockMajorTitle, model.Education);

            aboutMe = new FreeTextBlock(model.FontSizeBlockMajorTitle, Layout.Instance.Headers["About Me"], model.AboutMe);

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

                page.DefaultTextStyle(x =>
                    x.FontSize(Model.FontSizeDefault)
                    .FontFamily(Model.FontFamily)
                    .DirectionAuto()
                );

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

                if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
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

                if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
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

            var extraCourses = new ContentBlock(Model.FontSizeBlockMajorTitle, Layout.Instance.Headers["Courses"], Model.ExtraCourses);
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
                Layout.Instance.Headers["Working Experiences"],
                Model.FontSizeBlockMajorTitle,
                jobs
            );

            container.Component(block);
        }

        private void ComposeMilitarySection(IContainer container)
        {
            if (military.Count == 0) return;

            var block = new ExperienceCollectionBlock(
                Layout.Instance.Headers["Military Service"],
                Model.FontSizeBlockMajorTitle,
                military
            );

            container.Component(block);
        }
    }

    public class BulletPoint : IComponent
    {
        private string Text;

        public BulletPoint(string text)
        {
            Text = text;
        }

        public void Compose(IContainer container)
        {
            if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
            {
                container.Row(row =>
                {
                    row.AutoItem().Text("•");
                    row.RelativeItem().PaddingLeft(3).Text(Text);
                });
            }
            else
            {
                container.Row(row =>
                {
                    row.RelativeItem().AlignRight().PaddingRight(3).Text(Text);
                    row.AutoItem().AlignRight().Text("•").DirectionFromRightToLeft();
                });
            }
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
                if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
                {
                    row.ConstantItem(15).AlignMiddle().Element(composeIcon);
                    row.RelativeItem().PaddingLeft(5).AlignMiddle().Element(composeText);
                }
                else
                {
                    row.RelativeItem().PaddingRight(5).AlignMiddle().AlignRight().Element(composeText);
                    row.ConstantItem(15).AlignMiddle().Element(composeIcon);
                }
            });
        }
    
        private void composeIcon(IContainer container)
        {
            if (!File.Exists(Feature.Icon))
            {
                return;
            }
            else if (Feature.Icon.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            {
                var svg = new SKSvg();
                svg.Load(Feature.Icon);

                container.Svg(svg);
            }
            else
            {
                container.Image(Feature.Icon);
            }
        }

        private void composeText(IContainer container)
        {
            if (Feature.HyperLink != null)
            {
                container.Hyperlink(Feature.HyperLink).Text(Feature.Text);
            }
            else
            {
                container.Text(Feature.Text);
            }
        }
    }

    public class PersonalBlock : IComponent
    {
        private int TitleFontSize;
        private List<FeatureItem> Items { get; }

        public PersonalBlock(int titleFontSize, List<FeatureItem> items)
        {
            TitleFontSize = titleFontSize;
            Items = items;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
                {
                    column.Item().BorderBottom(1)
                    .Text(Layout.Instance.Headers["Personal Details"])
                    .FontSize(TitleFontSize).Bold();
                }
                else
                {
                    column.Item().BorderBottom(1).AlignRight()
                    .Text(Layout.Instance.Headers["Personal Details"])
                    .FontSize(TitleFontSize).Bold();
                }
                column.Item().Height(5);

                foreach (var item in Items)
                {
                    column.Item().PaddingBottom(3).Component(new FeatureItemComponent(item));
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
                if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
                {
                    column.Item().BorderBottom(1)
                    .Text(Layout.Instance.Headers["Education"])
                    .FontSize(TitleFontSize).Bold();
                }
                else
                {
                    column.Item().BorderBottom(1).AlignRight()
                    .Text(Layout.Instance.Headers["Education"])
                    .FontSize(TitleFontSize).Bold();
                }
                column.Item().Height(5);

                foreach (var item in Items)
                {
                    if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
                    {
                        column.Item().PaddingLeft(3).Text(item.Major).SemiBold();
                        if (!string.IsNullOrEmpty(item.Facility))
                        {
                            column.Item().PaddingLeft(3).Text(item.Facility);
                        }
                        if (!string.IsNullOrEmpty(item.DateSpan))
                        {
                            column.Item().PaddingLeft(3).Text(item.DateSpan);
                        }
                        column.Item().PaddingBottom(10);
                    }
                    else
                    {
                        column.Item().PaddingRight(3).AlignRight().Text(item.Major).SemiBold();
                        if (!string.IsNullOrEmpty(item.Facility))
                        {
                            column.Item().PaddingRight(3).AlignRight().Text(item.Facility);
                        }
                        if (!string.IsNullOrEmpty(item.DateSpan))
                        {
                            column.Item().PaddingRight(3).AlignRight().Text(item.DateSpan);
                        }
                        column.Item().PaddingBottom(10);
                    }
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
                if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
                {
                    column.Item().BorderBottom(1)
                    .Text(Title).FontSize(TitleFontSize).Bold();
                }
                else
                {
                    column.Item().BorderBottom(1).AlignRight()
                    .Text(Title).FontSize(TitleFontSize).Bold();
                }
                column.Item().Height(5);

                if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
                {
                    foreach (var item in Items)
                    {
                        column.Item().PaddingLeft(3).Text(item);
                    }
                }
                else
                {
                    foreach (var item in Items)
                    {
                        column.Item().AlignRight().PaddingRight(3).Text(item);
                    }
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
                    if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
                    {
                        column.Item().Text(Title).FontSize(TitleFontSize).SemiBold();
                        column.Item().PaddingBottom(3).Text(SubTitle)
                        .FontSize(SubTitleFontSize).FontColor(Colors.Grey.Darken3);
                    }
                    else
                    {
                        column.Item().AlignRight().Text(Title).FontSize(TitleFontSize).SemiBold();
                        column.Item().AlignRight().PaddingBottom(3).Text(SubTitle)
                        .FontSize(SubTitleFontSize).FontColor(Colors.Grey.Darken3);
                    }
                }
                else
                {
                    if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
                    {
                        column.Item().PaddingBottom(3).Text(Title).FontSize(TitleFontSize);
                    }
                    else
                    {
                        column.Item().AlignRight().PaddingBottom(3).Text(Title).FontSize(TitleFontSize);
                    }
                }

                foreach (var detail in Details)
                {
                    column.Item().Component(new BulletPoint(detail));
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
            SubTitle = data.DateSpan;
            Details = data.DescriptionItems;
        }
    }

    public class MilitaryBlock : ExperienceBlock
    {
        public MilitaryBlock(int titleFontSize, int subTitleFontSize, MilitaryServiceDetails data)
            : base(titleFontSize, subTitleFontSize)
        {
            Title = data.GetHeader();
            SubTitle = data.DateSpan;
            Details = data.DescriptionItems;
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
                if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
                {
                    column.Item().BorderBottom(1).Text(Title).FontSize(TitleFontSize).Bold();

                    foreach (var item in Items)
                    {
                        column.Item().Padding(5).Component(item);
                    }
                }
                else
                {
                    column.Item().BorderBottom(1).AlignRight().Text(Title).FontSize(TitleFontSize).Bold();

                    foreach (var item in Items)
                    {
                        column.Item().AlignRight().Padding(5).Component(item);
                    }
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
                if (Layout.Instance.TextDirection == TextDirection.LeftToRight)
                {
                    if (!string.IsNullOrEmpty(Text))
                    {
                        column.Item().BorderBottom(1)
                        .Text(Title).FontSize(TitleFontSize).Bold();
                        column.Item().Height(5);
                    }

                    column.Item().PaddingLeft(3).Text(Text);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Text))
                    {
                        column.Item().BorderBottom(1).AlignRight()
                        .Text(Title).FontSize(TitleFontSize).Bold();
                        column.Item().Height(5);
                    }

                    column.Item().AlignRight().PaddingRight(3).Text(Text);
                }
                
            });
        }
    }
}
