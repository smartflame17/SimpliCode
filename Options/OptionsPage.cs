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
        [Category("Detection Settings")]
        [DisplayName("Detection Sensibility")]
        [Description("Threshold for Detection. A low value will result in misdetection.")]
        [DefaultValue(0.7)]
        public double SimilarityThreshold { get; set; } = 0.7;

        [Category("Detection Settings")]
        [DisplayName("Minimum Detection Block Size")]
        [Description("Minimum block size for detection. A low value will result in longer execution.")]
        [DefaultValue(10)]
        public int CodeBlockSize { get; set; } = 10;

        [Category("Semantic Analysis")]
        [DisplayName("Enable Semantic Analysis")]
        [Description("Enables semantic analysis using OpenAI's model. An API key is required to function.")]
        [DefaultValue(false)]
        public bool SemanticAnalysis { get; set; } = false;

        [Category("Semantic Analysis")]
        [DisplayName("API key for semantic analysis")]
        [Description("Insert a valid OpenAi API key. This key will be used for semantic analysis.")]
        [DefaultValue("")]
        public string OpenAI_API_Key { get; set; } = string.Empty;
    }
}
