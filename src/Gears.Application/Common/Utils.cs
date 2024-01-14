namespace Gears.Application.Common;

public static class Utils
{
    public static string GenerateRandomPassword(PasswordOptions passwordOptions)
    {
        var randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };
        var random = new Random();
        var chars = new List<char>();

        if (passwordOptions.RequireUppercase)
        {
            chars.Insert(random.Next(0, chars.Count), randomChars[0][random.Next(0, randomChars[0].Length)]);
        }

        if (passwordOptions.RequireLowercase)
        {
            chars.Insert(random.Next(0, chars.Count), randomChars[1][random.Next(0, randomChars[1].Length)]);
        }

        if (passwordOptions.RequireDigit)
        {
            chars.Insert(random.Next(0, chars.Count), randomChars[2][random.Next(0, randomChars[2].Length)]);
        }

        if (passwordOptions.RequireNonAlphanumeric)
        {
            chars.Insert(random.Next(0, chars.Count), randomChars[3][random.Next(0, randomChars[3].Length)]);
        }

        for (var i = chars.Count; i < passwordOptions.RequiredLength || chars.Distinct().Count() < passwordOptions.RequiredUniqueChars; i++)
        {
            var rcs = randomChars[random.Next(0, randomChars.Length)];
            chars.Insert(random.Next(0, chars.Count), rcs[random.Next(0, rcs.Length)]);
        }

        return new string(chars.ToArray());
    }
}
