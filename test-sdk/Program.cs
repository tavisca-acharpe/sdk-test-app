using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavisca.FeatureConfigurations.Sdk;
using Tavisca.Platform.Common;
using Tavisca.Platform.Common.ExceptionManagement;
using Tavisca.Platform.Common.Logging;

namespace test_sdk
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IFeatureConfigurationsLogger featureConfigurationsLogger = new FeatureConfigLogger();

            var featureConfigurationsClient = new FeatureConfigurationClientBuilder()
                .WithApplicationName("checkout")
                .WithBaseUri("https://feature-configuration.qa.cnxloyalty.com/api/v1.0")
                .WithLogger(featureConfigurationsLogger)
                .Build();

            var settingKeys = new List<string>()
            {
                "Responsive_Client_Harbor", "Global_Enable_IsOrderFeeRefundable", "Global_Enable_RefundProcess","Global_Enable_CartLevelPayments"
            };

            var apiOptions = new ApiOptions
            {
                TenantId = "2o9o3ae99ts"
            };

            var facets = new Dictionary<string, string>()
            {
                { "clientid","103"},
                { "cpgid", "354" },
                { "programid","1389"}
            };

            var settingsRequest = new GetSettingsRequest
            {
                SettingKeys = settingKeys,
                ApiOptions = apiOptions,
                Facets = facets,
                AppInstanceName = "base"
            };

            var settingsResult = await featureConfigurationsClient.GetCommonSettingsAsync(settingsRequest);
            if (settingsResult != null)
            {
                Console.WriteLine("settingsResult");
            }

            var configurationsRequest = new GetConfigurationsRequest
            {
                FeatureName = "checkout",  //need to check 
                ApiOptions = apiOptions,
                Facets = facets,
                AppInstanceName = "base"
            };

            var configurationsResult = await featureConfigurationsClient.GetConfigurationForCommonFeatureAsync(configurationsRequest);

            if (configurationsResult != null)
            {
                Console.WriteLine("configurationsResult");
            }
        }
    }

    internal class FeatureConfigLogger : IFeatureConfigurationsLogger
    {
        public FeatureConfigLogger()
        {
            ExceptionPolicy.Configure(new ErrorHandler());
        }
        public async Task LogApiAsync(ApiLog apiLog)
        {
            Logger.WriteLogAsync(apiLog);
        }

        public async Task LogExceptionAsync(ExceptionLog exceptionLog)
        {
            Logger.WriteLogAsync(exceptionLog);
        }

        public async Task LogTraceAsync(TraceLog traceLog)
        {
            Logger.WriteLogAsync(traceLog);
        }
    }

    public class ErrorHandler : IErrorHandler
    {
        public bool HandleException(Exception ex, string policy, out Exception newException)
        {
            Logger.WriteLogAsync(new ExceptionLog(ex)).GetAwaiter().GetResult();
            newException = null;
            return false;
        }
    }
}
    