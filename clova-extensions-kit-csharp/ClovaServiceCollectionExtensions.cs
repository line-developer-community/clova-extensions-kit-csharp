﻿using Microsoft.Extensions.DependencyInjection;

namespace LineDC.CEK
{
    public static class ClovaServiceCollectionExtensions
    {
        public static IServiceCollection AddClova<T1, T2>(this IServiceCollection services)
            where T1 : class, IClova
            where T2 : ClovaBase, T1, new()
        {
            var clova = new T2();
            return services.AddSingleton<T1, T2>(_ => clova);
        }
    }
}
