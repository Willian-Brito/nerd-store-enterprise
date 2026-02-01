using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSE.Core.Communication;

namespace NSE.WebAPI.Core.Controllers;

[ApiController]
public abstract class MainController : Controller
{
    protected ICollection<string> Errors = new List<string>();
    
    protected ActionResult CustomResponse(object result = null)
    {
        if (ValidOperation()) return Ok(result);

        return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Messages", Errors.ToArray() }
        }));
    }
    
    protected ActionResult CustomResponse(ModelStateDictionary modelState)
    {
        var errors = modelState.Values.SelectMany(e => e.Errors).ToList();
        errors.ForEach(error => AddErrorToStack(error.ErrorMessage));

        return CustomResponse();
    }
    
    protected ActionResult CustomResponse(ValidationResult validationResult)
    {
        validationResult.Errors.ForEach(error => AddErrorToStack(error.ErrorMessage));
        return CustomResponse();
    }
    
    protected ActionResult CustomResponse(ResponseResult responseResult)
    {
        ResponseHasErrors(responseResult);
        return CustomResponse();
    }
    
    protected bool ResponseHasErrors(ResponseResult responseResult)
    {
        if (responseResult == null || !responseResult.Errors.Messages.Any()) return false;

        responseResult.Errors.Messages.ForEach(errorMessage => AddErrorToStack(errorMessage));
        return true;
    }
    
    protected bool ValidOperation()
    {
        return !Errors.Any();
    }
    
    protected void AddErrorToStack(string error)
    {
        Errors.Add(error);
    }
    
    protected void CleanErrors()
    {
        Errors.Clear();
    }
}