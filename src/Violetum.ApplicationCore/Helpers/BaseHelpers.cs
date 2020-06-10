﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.StaticFiles;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Helpers
{
    public static class BaseHelpers
    {
        private static readonly FileExtensionContentTypeProvider Provider = new FileExtensionContentTypeProvider();

        public static Func<T, object> GetOrderByExpression<T>(string sortColumn)
        {
            Func<T, object> orderByExpr = null;
            if (!string.IsNullOrEmpty(sortColumn))
            {
                Type sponsorResultType = typeof(T);

                if (sponsorResultType.GetProperties().Any(prop => prop.Name == sortColumn))
                {
                    PropertyInfo pinfo = sponsorResultType.GetProperty(sortColumn);
                    orderByExpr = data => pinfo.GetValue(data, null);
                }
            }

            return orderByExpr;
        }

        public static bool IsPaginatonSearchParamsValid(BaseSearchParams searchParams,
            out QueryStringErrorResponse errorResponse)
        {
            errorResponse = new QueryStringErrorResponse();
            var errors = new List<QueryStringErrorModel>();
            if ((searchParams.Limit > 50) || (searchParams.Limit <= 0))
            {
                errors.Add(new QueryStringErrorModel
                {
                    Message = "Limit must be between 0 and 50",
                    QueryStringName = nameof(searchParams.Limit),
                });
            }

            if (searchParams.CurrentPage <= 0)
            {
                errors.Add(new QueryStringErrorModel
                {
                    Message = "Current Page must not be negative",
                    QueryStringName = nameof(searchParams.CurrentPage),
                });
            }

            foreach (QueryStringErrorModel errorModel in errors.Select(error => new QueryStringErrorModel
            {
                QueryStringName = error.QueryStringName,
                Message = error.Message,
            }))
            {
                errorResponse.Errors.Add(errorModel);
            }

            return !errors.Any();
        }

        public static FileData GetFileData<TEntity>(string image, string fileName)
        {
            // TODO: add validation
            string[] imageParts = image.Split(",");
            string contentType = imageParts[0].Split("/")[1].Split(";")[0];
            string blobName = $"{typeof(TEntity).Name}/{fileName}.{contentType}";
            return new FileData
            {
                Content = imageParts[1],
                FileName = blobName,
            };
        }

        public static string GetContentType(this string fileName)
        {
            if (!Provider.TryGetContentType(fileName, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}