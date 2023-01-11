﻿using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NET.AutoWebApi
{
    public class AutoApiServiceConventionWrapper : IAutoApiServiceConvention
    {
        private readonly IAutoApiServiceConvention _convention;

        public AutoApiServiceConventionWrapper(IServiceCollection services)
        {
            _convention = services.BuildServiceProvider().GetRequiredService<IAutoApiServiceConvention>();
        }

        public void Apply(ApplicationModel application)
        {
            _convention.Apply(application);
        }
    }
}
