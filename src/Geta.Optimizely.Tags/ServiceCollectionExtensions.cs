﻿using System.Linq;
using EPiServer.DependencyInjection;
using EPiServer.Shell.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Geta.Optimizely.Tags
{
    public static class ServiceCollectionExtensions
    {
        const string ModuleName = "Geta.Optimizely.Tags";

        public static IServiceCollection AddGetaTags(this IServiceCollection services)
        {
            services.Configure<ProtectedModuleOptions>(
                pm =>
                {
                    if (!pm.Items.Any(i => i.Name.Equals(ModuleName, System.StringComparison.OrdinalIgnoreCase)))
                    {
                        pm.Items.Add(new ModuleDetails
                        {
                            Name = ModuleName
                        });
                    }
                });

            return services;
        }
    }
}