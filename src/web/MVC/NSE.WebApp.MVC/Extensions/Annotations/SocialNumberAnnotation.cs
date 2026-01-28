using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace NSE.WebApp.MVC.Extensions.Annotations;

public class SocialNumberAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        return !string.IsNullOrEmpty(value.ToString())
            ? ValidationResult.Success
            : new ValidationResult("Invalid social number");
    }
}

public class SocialNumberAttributeAdapter : AttributeAdapterBase<SocialNumberAttribute>
{
    public SocialNumberAttributeAdapter(
        SocialNumberAttribute attribute, 
        IStringLocalizer stringLocalizer
    ) : base(attribute, stringLocalizer) { }

    public override void AddValidation(ClientModelValidationContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-cpf", GetErrorMessage(context));
    }

    public override string GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return "Invalid Social Number";
    }
}

public class SocialNumberValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
{
    private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

    public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
    {
        if (attribute is SocialNumberAttribute CpfAttribute)
            return new SocialNumberAttributeAdapter(CpfAttribute, stringLocalizer);

        return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
    }
}