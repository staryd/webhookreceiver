public class Settings 
{
	public string AuthCode { get; set; } = string.Empty;
	public string TempFolder { get; set; } = "webhookreceiver";
	public int DeleteFilesOlderThanMinutes { get; set; } = 60;

	public string GetUserTempPath()
	{
		return Path.Combine(Path.GetTempPath(), TempFolder);
	}
}