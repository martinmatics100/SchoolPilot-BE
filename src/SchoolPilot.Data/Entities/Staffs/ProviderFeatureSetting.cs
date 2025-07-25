

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;

namespace SchoolPilot.Data.Entities.Staffs
{
    public class ProviderFeatureSetting : ModeledEntity, IEntity, IAccountScope, IAuditable
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public Guid ProviderId { get; set; }

        public Guid? LocationId { get; set; }

        public FeatureCategory Category { get; set; }

        public Feature Feature { get; set; }

        [MaxLength(100)]
        public string ValueType { get; private set; }

        public int Value { get; private set; }

        public bool CheckFeature<TEnum>(TEnum value) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ((TEnum)GetValue()).Equals(value);
        }

        public object GetValue()
        {
            var enumType = Type.GetType(ValueType);
            Debug.Assert(enumType != null, nameof(enumType) + " != null");
            return Enum.ToObject(enumType, Value);
        }

        public void SetValue<TEnum>(TEnum value) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Value = value.ToInt32(new NumberFormatInfo());
            ValueType = typeof(TEnum).FullName;
        }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }

        internal override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProviderFeatureSetting>()
                .AddIndex(builder => builder
                    .HasIndex(featureSetting => featureSetting.AccountId)
                    .HasDatabaseName("IDX_ProviderFeatureSetting_AccountId")
                )
                .AddIndex(builder => builder
                    .HasIndex(featureSetting => new
                    {
                        featureSetting.ProviderId,
                        featureSetting.Category
                    })
                    .HasDatabaseName("IDX_ProviderFeatureSetting_PId_Category"))
                .AddIndex(builder => builder.HasIndex(featureSetting => featureSetting.LocationId));
        }
    }

    public enum FeatureCategory
    {
        Clinical = 1,
        Operational = 2,
        Billing = 3,
        Integrations = 4,
        Notifications = 5
    }

    public enum Feature
    {
        OrdersDrivenScheduling = 1,
        IdgCompliance = 2,
        PrintDocumentAddendum = 3,
        SignatureDateRequired = 4,
        SignatureTimeRequired = 5,
        PatientFaceSheetSsn = 6,
        ClosedAccountingOverride = 7,
        WorldViewOrdersManagement = 8,
        WorldViewAutomaticSubmission = 9,
        PayrollCenter = 10,
        AutoGenerateMrn = 11,
        AutoCalculateMileage = 12,
        AutoCalculateWeekendMileage = 13,
        AutoCalculateMileageToHome = 14,
        Tag = 15,
        TieCarePlanToVisit = 16, // both hospice aide and homemaker care plans use this
        ForcuraOrdersManagement = 17,
        ForcuraAutomaticSubmission = 18,
        PayrollCycles = 19,
        MultiLocationPayroll = 20,
        HisExported = 21,
        ScheduleChangeNotification = 22,
        ValidateVisitTravelTime = 23,
        SkippableSection = 24,
        ScheduledChangeAssignedUserNotification = 25,
        ScheduledChangeSpecificOtherUsersNotification = 26, //Setting the encompasses the 3 more specifics
        ScheduleChangeSpecificTitles = 27,
        ScheduleChangeSpecificRecipients = 28, //specifically for specifying individuals
        ScheduleChangeSpecificTeams = 29,
        MissedVisitsPayroll = 30,
        AllowFutureDocumentation = 31,
        CommercialEligibilityCheck = 32,
        AutomaticEligibilityCheck = 33,
        OptimizedProviderPdf = 34,
        PatientStatusChangeNotification = 35,
        PatientStatusChangeSpecificTitles = 36,
        PatientStatusChangeSpecificUsersNotification = 37,
        PatientStatusChangeSpecificTeamRecipients = 38,
        VisitCommunicationEnabled = 39,
        EarningCodes = 40
    }

    public enum FeatureToggle
    {
        Disabled,
        Enabled
    }
}
