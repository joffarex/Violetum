﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Violetum.API.Contracts.V1.Responses;

namespace Violetum.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                KeyValuePair<string, IEnumerable<string>>[] errorsInModelState = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(x => x.ErrorMessage)
                    )
                    .ToArray();

                var errorResponse = new ErrorResponse();

                foreach ((string fieldName, IEnumerable<string> errorMessages) in errorsInModelState)
                {
                    foreach (string errorMessage in errorMessages)
                    {
                        var errorModel = new ErrorModel
                        {
                            FieldName = fieldName,
                            Message = errorMessage,
                        };

                        errorResponse.Errors.Add(errorModel);
                    }
                }

                context.Result = new BadRequestObjectResult(errorResponse);
                return;
            }

            await next();
        }
    }
}