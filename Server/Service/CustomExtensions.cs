namespace Mientreno.Server.Service;

public static class CustomExtensions
{
	public static string NextString(this Random random, int size)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		return new string(Enumerable.Repeat(chars, size)
			.Select(s => s[random.Next(s.Length)]).ToArray());
	}

	public static string NextString(this Random random)
	{
		return random.NextString(8);
	}
	
	public static bool IsNullOrWhiteSpace(this string? str)
	{
		return string.IsNullOrWhiteSpace(str);
	}
}