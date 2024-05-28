using System.Text.RegularExpressions;

namespace WebhookReceiver;

public static class Utils 
{
	private static readonly Regex SanitizeExpression = new Regex("[^a-zA-Z0-9_.\\/\\-]+", RegexOptions.Compiled);
		///<summary>Ersätter åäöéü mot aaoeu, mellanslag mot _ och tar bort övriga ogiltiga tecken.</summary>
	public static string SanitizeFileName(this string itemName) {
		itemName = itemName
			.Replace('Å', 'A')
			.Replace('Ä', 'A')
			.Replace('Ö', 'O')
			.Replace('É', 'E')
			.Replace('Ü', 'U')
			.Replace('å', 'a')
			.Replace('ä', 'a')
			.Replace('ö', 'o')
			.Replace('é', 'e')
			.Replace('ü', 'u')
			.Replace(' ', '_')
			.Replace('/', '_');

		return SanitizeExpression.Replace(itemName, string.Empty);
	}
}