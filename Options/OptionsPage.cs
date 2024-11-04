using System.ComponentModel;
using System.Runtime.InteropServices;

namespace simplicode
{
    internal partial class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.OptionsPageOptions), "simplicode", "OptionsPage", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class OptionsPageOptions : BaseOptionPage<OptionsPage> { }
    }

    public class OptionsPage : BaseOptionModel<OptionsPage>
    {
        [Category("My category")]
        [DisplayName("My Option")]
        [Description("An informative description.")]
        [DefaultValue(true)]
        public bool MyOption { get; set; } = true;

        [Category("My category")]
        [DisplayName("Detection Sensibility")]
        [Description("Threshold for Detection. A low value will result in misdetection.")]
        [DefaultValue(0.7)]
        public double SimilarityThreshold { get; set; } = 0.7;

        [Category("My category")]
        [DisplayName("Minimum Detection Block Size")]
        [Description("Minimum block size for detection. A low value will result in longer execution.")]
        [DefaultValue(10)]
        public int CodeBlockSize { get; set; } = 10;
    }
}
