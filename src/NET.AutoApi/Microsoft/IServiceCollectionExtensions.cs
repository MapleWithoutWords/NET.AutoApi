﻿using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NET.AutoApi;
using NET.AutoWebApi;
using NET.AutoWebApi.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddAutoApiService(this IServiceCollection services, Action<AutoApiConventionalControllerOptions> optionAction = null, Action<MvcOptions> mvcOptionAction = null)
        {

            services.Configure<AutoApiConventionalControllerOptions>(opt =>
            {
                optionAction?.Invoke(opt);
            });

            services.AddSingleton<IConventionalRouteBuilder, ConventionalRouteBuilder>();
            services.AddTransient<IAutoApiServiceConvention, AutoApiServiceConvention>();

            services.Configure<MvcOptions>(opt =>
            {
                opt.Conventions.Add(new AutoApiServiceConventionWrapper());
                mvcOptionAction?.Invoke(opt);
            });
        }


        public static void UseAutoApiService(this IHost host)
        {
            ServiceProviderManager.ServiceProvider = host.Services;
            var partManager = host.Services.GetRequiredService<ApplicationPartManager>();
            partManager.FeatureProviders.Add(new AutoApiConventionalControllerFeatureProvider(host.Services));

            var conventionalOptions = host.Services.GetRequiredService<IOptions<AutoApiConventionalControllerOptions>>();
            foreach (var moduleAssembly in conventionalOptions.Value.ConventionalControllerSettings)
            {
                partManager.ApplicationParts.AddIfNotContains(moduleAssembly.Assembly);
            }
        }

        public static void AddIfNotContains(this IList<ApplicationPart> applicationParts, Assembly assembly)
        {
            if (applicationParts.Any(
                p => p is AssemblyPart assemblyPart && assemblyPart.Assembly == assembly))
            {
                return;
            }

            applicationParts.Add(new AssemblyPart(assembly));
        }
    }
}