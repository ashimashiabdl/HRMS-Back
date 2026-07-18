using System;
using System.Collections.Generic;

namespace HR.Order.Core.DTOs
{
    /// <summary>
    /// DTO برای نمایش نتیجه مقایسه دو حکم
    /// </summary>
    public class OrderComparisonResultDTO
    {
        public InterdictOrderComparisonDTO? InterdictOrderComparison { get; set; }
        public RecruitOrderComparisonDTO? RecruitOrderComparison { get; set; }
        public List<InterdictOrderWageItemComparisonDTO>? WageItemComparisons { get; set; }
        public List<InterdictOrderCoefficientItemComparisonDTO>? CoefficientItemComparisons { get; set; }
        public string? Message { get; set; }
        public bool HasPreviousOrder { get; set; }
    }

    /// <summary>
    /// نتیجه مقایسه InterdictOrder
    /// </summary>
    public class InterdictOrderComparisonDTO
    {
        public long CurrentOrderId { get; set; }
        public long? PreviousOrderId { get; set; }
        public List<PropertyComparisonDTO> PropertyDifferences { get; set; } = new();
    }

    /// <summary>
    /// نتیجه مقایسه RecruitOrder
    /// </summary>
    public class RecruitOrderComparisonDTO
    {
        public long CurrentRecruitOrderId { get; set; }
        public long? PreviousRecruitOrderId { get; set; }
        public List<PropertyComparisonDTO> PropertyDifferences { get; set; } = new();
    }

    /// <summary>
    /// مقایسه یک ویژگی
    /// </summary>
    public class PropertyComparisonDTO
    {
        /// <summary>
        /// نام ویژگی به انگلیسی
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// نام ویژگی به فارسی
        /// </summary>
        public string PropertyNameFa { get; set; } = string.Empty;

        /// <summary>
        /// مقدار قبلی
        /// </summary>
        public string? OldValue { get; set; }

        /// <summary>
        /// مقدار جدید
        /// </summary>
        public string? NewValue { get; set; }

        /// <summary>
        /// نوع تغییر
        /// </summary>
        public string ChangeType { get; set; } = "Modified";
    }

    /// <summary>
    /// نتیجه مقایسه InterdictOrderWageItem
    /// </summary>
    public class InterdictOrderWageItemComparisonDTO
    {
        public long? Id { get; set; }
        public long WageItemId { get; set; }
        public string? WageItemName { get; set; }
        public int? OldValue { get; set; }
        public int? NewValue { get; set; }
        /// <summary>
        /// Added, Deleted, Modified
        /// </summary>
        public string ChangeType { get; set; } = string.Empty;
    }

    /// <summary>
    /// نتیجه مقایسه InterdictOrderCoefficientItem
    /// </summary>
    public class InterdictOrderCoefficientItemComparisonDTO
    {
        public long? Id { get; set; }
        public long CoefficientId { get; set; }
        public string? CoefficientName { get; set; }
        public double? OldValue { get; set; }
        public double? NewValue { get; set; }
        /// <summary>
        /// Added, Deleted, Modified
        /// </summary>
        public string ChangeType { get; set; } = string.Empty;
    }
}

