namespace IPFees.Web.Data
{
    /// <summary>
    /// Whether the web application returns its store to the reference corpus. The public sandbox
    /// resets at start-up and every few hours, so that what one visitor edits does not reach the
    /// next. A firm's own deployment turns the reset off: an empty store is still seeded once, and
    /// from then on the firm's schedules, jurisdictions and service fee levels survive restarts.
    /// </summary>
    public class DatabaseResetSettings
    {
        public const string SectionName = "DatabaseReset";

        /// <summary>
        /// Reset at start-up and every <see cref="IntervalHours"/> hours. When false, the corpus and
        /// the service fee levels are loaded only into an empty store.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>Hours between resets; zero or less resets at start-up only.</summary>
        public double IntervalHours { get; set; } = 6;

        /// <summary>Folder, relative to the working directory, that holds servicefees.json.</summary>
        public string DataFolder { get; set; } = "wwwroot/data";
    }
}
