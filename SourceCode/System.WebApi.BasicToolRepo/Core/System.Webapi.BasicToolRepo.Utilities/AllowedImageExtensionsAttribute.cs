using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

public class AllowedImageExtensionsAttribute : ValidationAttribute  
{
    private static readonly Regex ImageFileRegex =
        new(@"^.*\.(jpg|jpeg|png|gif|bmp|webp)$", RegexOptions.IgnoreCase);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var files = value as IFormFileCollection;
        if (files == null || files.Count == 0)
        {
            return new ValidationResult("At least one file is required.");
        }

        foreach (var file in files)
        {
            if (!ImageFileRegex.IsMatch(file.FileName))
            {
                return new ValidationResult($"File '{file.FileName}' is not a valid image format.");
            }
        }

        return ValidationResult.Success;
    }
}
