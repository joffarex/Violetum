﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Violetum.Application;
using Violetum.Domain.Infrastructure;
using Violetum.Web.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceRegister
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection @this)
        {
            Type serviceType = typeof(Service);
            IEnumerable<TypeInfo> definedType = serviceType.Assembly.DefinedTypes;

            IEnumerable<TypeInfo> services = definedType
                .Where(x => x.GetTypeInfo().GetCustomAttribute<Service>() != null);

            foreach (TypeInfo service in services)
            {
                @this.AddTransient(service);
            }

            @this.AddTransient<ITokenManager, TokenManager>();
            return @this;
        }
    }
}