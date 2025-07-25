

using AutoMapper.Features;
using SchoolPilot.Data.Entities.Staffs;
using SchoolPilot.Common.Extensions;

namespace SchoolPilot.Infrastructure.Extensions
{
    public static class ProviderFeatureDictionaryExtensions
    {
        /// <summary>
        /// Converts the enum value of feature setting, which can only be on or off, to a boolean.
        /// If the provider does not have the feature set yet, the default will be off/false.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        /// 
        public static bool CheckTwoStateToggleFeature(this Dictionary<Feature, ProviderFeatureSetting> settings, Feature feature)
        {
            return settings.GetOrDefault(feature)?.CheckFeature(FeatureToggle.Enabled) == true;
        }


        /// <summary>
        /// Updates a feature setting, which is only in 2 states: on or off.
        /// If the feature setting doesn't already exist in the provider then the method will add it.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="feature"></param>
        /// <param name="state">On/Off state of the feature</param>
        /// <param name="accountId">Provider's Account id; to be used in creating the feature if it does not exist.</param>
        /// <param name="providerId">Id of the Provider; to be used in creating the feature if it does not exist.</param>
        /// <param name="category">Category of the feature; to be used in creating the feature if it does not exist.</param>
        /// <param name="locationId">Location that the feature applies to;will be null if it applies to all locations.</param>
        /// <param name="modifiedBy">Id of user modifying the feature.</param>
        /// <returns>Returns a boolean which states that the feature was updated.</returns>
        /// 
        public static bool UpdateTwoStateToggleFeature(this Dictionary<Feature, ProviderFeatureSetting> settings, Feature feature, bool state,
                                               Guid accountId, Guid providerId, FeatureCategory category, Guid? locationId = null,
                                               Guid? modifiedBy = null)
        {
            var isUpdating = settings.TryGetValue(feature, out var featureSetting);
            if (featureSetting == null)
            {
                featureSetting = new ProviderFeatureSetting
                {
                    AccountId = accountId,
                    ProviderId = providerId,
                    LocationId = locationId,
                    Category = category,
                    Feature = feature,
                    ModifiedBy = modifiedBy
                };
            }

            var priorState = (FeatureToggle)featureSetting.Value;
            var newState = state ? FeatureToggle.Enabled : FeatureToggle.Disabled;
            featureSetting.SetValue(newState);

            //Only set the value in the dictionary if it doesn't already exist
            if (!isUpdating)
            {
                settings[feature] = featureSetting;
            }
            else
            {
                if (priorState != newState)
                {
                    featureSetting.ModifiedBy = modifiedBy;
                }
            }

            return isUpdating;
        }

    }
}
