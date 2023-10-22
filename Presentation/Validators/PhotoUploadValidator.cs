namespace Presentation.Validators;

using FluentValidation;
using Presentation.Dtos.Gallery;

public class PhotoUploadValidator : AbstractValidator<PhotoUploadRequest>
{
    public PhotoUploadValidator()
    {
        RuleFor(rqs => rqs.Title)
            .NotEmpty().NotNull().MaximumLength(50);
        RuleFor(rqs => rqs.Description)
            .NotNull().NotEmpty().MaximumLength(1000);
        RuleFor(rqs => rqs.File)
            .NotEmpty().NotNull()
            .Must(file => file.ContentType.Contains("image"))
                .WithMessage("The uploaded file is not an image")
            .Must(file => file.Length != 0)
                .WithMessage("File is empty or not attached.");
    }
}