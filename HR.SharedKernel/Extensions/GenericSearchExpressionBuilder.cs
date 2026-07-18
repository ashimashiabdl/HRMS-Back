using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace HR.SharedKernel.Extensions;

internal static class PropertyPathExpressionBuilder
{
    public static bool TryBuildMemberAccess(
        Type rootType,
        string propertyPath,
        out ParameterExpression parameter,
        out Expression memberAccess,
        out Type memberType)
    {
        parameter = Expression.Parameter(rootType, "p");
        memberAccess = parameter;
        memberType = rootType;

        foreach (var segment in propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var declaringType = memberAccess.Type;
            if (Nullable.GetUnderlyingType(declaringType) is Type underlyingType)
            {
                declaringType = underlyingType;
            }

            var property = declaringType.GetProperty(
                segment,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                memberAccess = parameter!;
                memberType = rootType;
                return false;
            }

            memberAccess = Expression.Property(memberAccess, property);
        }

        memberType = memberAccess.Type;
        return true;
    }
}

internal static class GenericSearchExpressionBuilder
{
    private static readonly string[] EmployeeSearchFields =
    [
        "FirstName",
        "LastName",
        "PersonelCode",
        "IdentityNo",
        "NationalNo"
    ];

    public static Expression<Func<T, bool>>? BuildPredicate<T>(string filter)
    {
        var filterLower = filter.ToLowerInvariant();
        var parameter = Expression.Parameter(typeof(T), "item");
        Expression? combined = null;

        foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!IsSearchable(property))
            {
                continue;
            }

            var condition = BuildPropertyCondition(parameter, property, filter, filterLower);
            if (condition == null)
            {
                continue;
            }

            combined = combined == null ? condition : Expression.OrElse(combined, condition);
        }

        return combined == null
            ? null
            : Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    private static bool IsSearchable(PropertyInfo property) =>
        property.GetCustomAttribute<IsEffectiveInGenericSearch>()?.IsEffective == true;

    private static Expression? BuildPropertyCondition(
        ParameterExpression parameter,
        PropertyInfo property,
        string filter,
        string filterLower)
    {
        var propertyType = UnwrapNullable(property.PropertyType);

        if (IsStringType(propertyType) || IsNumericType(propertyType))
        {
            return BuildStringContainsCondition(
                Expression.Property(parameter, property),
                property.PropertyType,
                filterLower,
                ignoreCase: true);
        }

        if (IsDateTimeType(propertyType))
        {
            return BuildStringContainsCondition(
                Expression.Property(parameter, property),
                property.PropertyType,
                filter,
                ignoreCase: false);
        }

        if (IsBoolType(propertyType))
        {
            return BuildBoolCondition(
                Expression.Property(parameter, property),
                property.PropertyType,
                filterLower);
        }

        if (IsNavigationEntity(propertyType))
        {
            return BuildNavigationCondition(parameter, property, filterLower);
        }

        return null;
    }

    private static Expression? BuildNavigationCondition(
        ParameterExpression parameter,
        PropertyInfo navigationProperty,
        string filterLower)
    {
        var navigationAccess = Expression.Property(parameter, navigationProperty);
        var navigationNotNull = Expression.NotEqual(
            navigationAccess,
            Expression.Constant(null, navigationProperty.PropertyType));

        Expression? combined = null;

        if (string.Equals(navigationProperty.Name, "Employee", StringComparison.Ordinal))
        {
            foreach (var fieldName in EmployeeSearchFields)
            {
                var nestedProperty = navigationProperty.PropertyType.GetProperty(
                    fieldName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (nestedProperty == null)
                {
                    continue;
                }

                var fieldCondition = BuildNestedStringContainsCondition(
                    navigationAccess,
                    nestedProperty,
                    filterLower);

                combined = combined == null
                    ? fieldCondition
                    : Expression.OrElse(combined, fieldCondition);
            }
        }
        else
        {
            var titleProperty = navigationProperty.PropertyType.GetProperty(
                "title",
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (titleProperty != null && titleProperty.PropertyType == typeof(string))
            {
                var titleCondition = BuildNestedStringContainsCondition(
                    navigationAccess,
                    titleProperty,
                    filterLower);

                combined = titleCondition;
            }

            foreach (var nestedProperty in navigationProperty.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!IsSearchable(nestedProperty))
                {
                    continue;
                }

                var nestedType = UnwrapNullable(nestedProperty.PropertyType);
                if (!IsNavigationEntity(nestedType))
                {
                    continue;
                }

                var nestedTitleProperty = nestedType.GetProperty(
                    "title",
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (nestedTitleProperty == null || nestedTitleProperty.PropertyType != typeof(string))
                {
                    continue;
                }

                var nestedNavigationAccess = Expression.Property(navigationAccess, nestedProperty);
                var nestedNavigationNotNull = Expression.NotEqual(
                    nestedNavigationAccess,
                    Expression.Constant(null, nestedProperty.PropertyType));
                var nestedTitleCondition = BuildNestedStringContainsCondition(
                    nestedNavigationAccess,
                    nestedTitleProperty,
                    filterLower);
                var nestedCondition = Expression.AndAlso(nestedNavigationNotNull, nestedTitleCondition);

                combined = combined == null
                    ? nestedCondition
                    : Expression.OrElse(combined, nestedCondition);
            }
        }

        if (combined == null)
        {
            return null;
        }

        return Expression.AndAlso(navigationNotNull, combined);
    }

    private static Expression BuildNestedStringContainsCondition(
        Expression navigationAccess,
        PropertyInfo nestedProperty,
        string filterLower)
    {
        var nestedAccess = Expression.Property(navigationAccess, nestedProperty);
        return BuildStringContainsCondition(nestedAccess, nestedProperty.PropertyType, filterLower, ignoreCase: true)!;
    }

    private static Expression? BuildStringContainsCondition(
        Expression valueAccess,
        Type declaredType,
        string filter,
        bool ignoreCase)
    {
        var underlyingType = UnwrapNullable(declaredType);
        Expression stringExpression;

        if (underlyingType == typeof(string))
        {
            stringExpression = valueAccess;
        }
        else
        {
            var toString = underlyingType.GetMethod(nameof(ToString), Type.EmptyTypes);
            if (toString == null)
            {
                return null;
            }

            var valueForToString = valueAccess;
            if (IsNullableType(declaredType))
            {
                valueForToString = Expression.Property(valueAccess, "Value");
            }

            stringExpression = Expression.Call(valueForToString, toString);
        }

        Expression? notNull = null;

        if (underlyingType == typeof(string))
        {
            notNull = Expression.AndAlso(
                Expression.NotEqual(valueAccess, Expression.Constant(null, declaredType)),
                Expression.NotEqual(stringExpression, Expression.Constant(string.Empty)));
        }
        else if (IsNullableType(declaredType))
        {
            notNull = Expression.NotEqual(valueAccess, Expression.Constant(null, declaredType));
        }

        var containsSource = ignoreCase
            ? Expression.Call(
                stringExpression,
                typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!)
            : stringExpression;

        var contains = Expression.Call(
            containsSource,
            typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!,
            Expression.Constant(filter));

        return notNull == null
            ? contains
            : Expression.AndAlso(notNull, contains);
    }

    private static Expression? BuildBoolCondition(
        Expression valueAccess,
        Type declaredType,
        string filterLower)
    {
        bool? searchValue = filterLower switch
        {
            "true" or "1" or "بله" or "بلی" => true,
            "false" or "0" or "خیر" => false,
            _ => null
        };

        if (searchValue == null)
        {
            return null;
        }

        Expression boolExpression = valueAccess;
        Expression? notNull = null;

        if (IsNullableType(declaredType))
        {
            notNull = Expression.NotEqual(valueAccess, Expression.Constant(null, declaredType));
            boolExpression = Expression.Property(valueAccess, "Value");
        }

        var equals = Expression.Equal(
            boolExpression,
            Expression.Constant(searchValue.Value, typeof(bool)));

        return notNull == null
            ? equals
            : Expression.AndAlso(notNull, equals);
    }

    private static bool IsBoolType(Type type) => type == typeof(bool);

    private static bool IsNullableType(Type type) =>
        !type.IsValueType || Nullable.GetUnderlyingType(type) != null;

    private static bool IsStringType(Type type) => type == typeof(string);

    private static bool IsNumericType(Type type) =>
        type == typeof(int) ||
        type == typeof(long) ||
        type == typeof(short) ||
        type == typeof(byte) ||
        type == typeof(decimal) ||
        type == typeof(double) ||
        type == typeof(float);

    private static bool IsDateTimeType(Type type) =>
        type == typeof(DateTime) ||
        type == typeof(DateTimeOffset);

    private static bool IsNavigationEntity(Type type) =>
        typeof(BaseEntity).IsAssignableFrom(type);

    private static Type UnwrapNullable(Type type) =>
        Nullable.GetUnderlyingType(type) ?? type;
}
