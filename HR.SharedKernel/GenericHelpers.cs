using FastMember;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Globalization;
using System.Reflection;

namespace HR.SharedKernel
{
    /// <summary>
    /// Node in a hierarchy produced by <see cref="GenericHelpers.GenerateTree{T,TKey}"/>.
    /// </summary>
    public sealed class TreeItem<T>
    {
        public T Item { get; set; } = default!;

        public IList<TreeItem<T>> Children { get; set; } = new List<TreeItem<T>>();
    }

    /// <summary>
    /// Shared helpers for SqlDataReader mapping, type discovery, and flat-to-tree projection.
    /// </summary>
    public static class GenericHelpers
    {
        private static readonly ConcurrentDictionary<Type, TypeMap> TypeMaps = new();

        private sealed class TypeMap
        {
            public TypeMap(TypeAccessor accessor, Dictionary<string, Member> membersByName)
            {
                Accessor = accessor;
                MembersByName = membersByName;
            }

            public TypeAccessor Accessor { get; }
            public Dictionary<string, Member> MembersByName { get; }
        }

        /// <summary>
        /// Maps the current row of a data reader onto a new instance of <typeparamref name="T"/>.
        /// Column names are matched to writable members (case-insensitive). DBNull is skipped.
        /// Type metadata is cached per <typeparamref name="T"/> for hot read loops.
        /// </summary>
        public static T ConvertToObject<T>(this DbDataReader rd) where T : class, new()
        {
            ArgumentNullException.ThrowIfNull(rd);

            var map = GetTypeMap(typeof(T));
            var instance = new T();
            var accessor = map.Accessor;
            var membersByName = map.MembersByName;

            for (int i = 0; i < rd.FieldCount; i++)
            {
                if (rd.IsDBNull(i))
                {
                    continue;
                }

                if (!membersByName.TryGetValue(rd.GetName(i), out var member))
                {
                    continue;
                }

                object value = rd.GetValue(i);
                try
                {
                    accessor[instance, member.Name] = CoerceSqlValue(value, member.Type);
                }
                catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
                {
                    throw new InvalidCastException(
                        $"Failed to map column '{rd.GetName(i)}' (SQL type '{value.GetType().Name}') to '{typeof(T).Name}.{member.Name}' ({member.Type.Name}).",
                        ex);
                }
            }

            return instance;
        }

        /// <summary>
        /// Reads all remaining rows into a list of <typeparamref name="T"/>.
        /// </summary>
        public static List<T> ConvertToList<T>(this DbDataReader rd) where T : class, new()
        {
            ArgumentNullException.ThrowIfNull(rd);

            var list = new List<T>();
            while (rd.Read())
            {
                list.Add(rd.ConvertToObject<T>());
            }

            return list;
        }

        /// <summary>
        /// SqlDataReader / ADO.NET may return narrower or wider numeric types than the DTO
        /// (e.g. Int32 for COUNT(*) while the property is Int64). Coerce safely.
        /// </summary>
        private static object? CoerceSqlValue(object value, Type targetType)
        {
            if (value is null or DBNull)
            {
                return null;
            }

            var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (underlying.IsInstanceOfType(value))
            {
                return value;
            }

            if (underlying.IsEnum)
            {
                if (value is string enumName)
                {
                    return Enum.Parse(underlying, enumName, ignoreCase: true);
                }

                return Enum.ToObject(underlying, value);
            }

            if (underlying == typeof(Guid))
            {
                return value switch
                {
                    Guid g => g,
                    string s => Guid.Parse(s),
                    byte[] bytes => new Guid(bytes),
                    _ => throw new InvalidCastException($"Cannot convert '{value.GetType().Name}' to Guid.")
                };
            }

            if (underlying == typeof(bool))
            {
                return value switch
                {
                    bool b => b,
                    byte by => by != 0,
                    sbyte sb => sb != 0,
                    short s => s != 0,
                    int i => i != 0,
                    long l => l != 0,
                    string str when str == "1" || str.Equals("true", StringComparison.OrdinalIgnoreCase) => true,
                    string str when str == "0" || str.Equals("false", StringComparison.OrdinalIgnoreCase) => false,
                    _ => Convert.ToBoolean(value, CultureInfo.InvariantCulture)
                };
            }

            if (underlying == typeof(DateTimeOffset) && value is DateTime dt)
            {
                return new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Unspecified), TimeSpan.Zero);
            }

            if (underlying == typeof(DateTime) && value is DateTimeOffset dto)
            {
                return dto.DateTime;
            }

            if (underlying == typeof(string))
            {
                return Convert.ToString(value, CultureInfo.InvariantCulture);
            }

            return Convert.ChangeType(value, underlying, CultureInfo.InvariantCulture);
        }

        private static TypeMap GetTypeMap(Type type)
        {
            return TypeMaps.GetOrAdd(type, static t =>
            {
                var accessor = TypeAccessor.Create(t);
                var membersByName = new Dictionary<string, Member>(StringComparer.OrdinalIgnoreCase);

                foreach (var member in accessor.GetMembers())
                {
                    if (!member.CanWrite)
                    {
                        continue;
                    }

                    // First writable member wins when names collide (case-insensitive).
                    membersByName.TryAdd(member.Name, member);
                }

                return new TypeMap(accessor, membersByName);
            });
        }

        /// <summary>
        /// Returns concrete types in <paramref name="assembly"/> assignable to <paramref name="baseType"/>.
        /// Excludes <paramref name="baseType"/> itself and, by default, abstract types.
        /// </summary>
        public static IEnumerable<Type> FindDerivedTypes(
            Assembly assembly,
            Type baseType,
            bool includeAbstract = false)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            ArgumentNullException.ThrowIfNull(baseType);

            Type?[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            foreach (var type in types)
            {
                if (type is null || type == baseType)
                {
                    continue;
                }

                if (!includeAbstract && type.IsAbstract)
                {
                    continue;
                }

                if (baseType.IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
        }

        /// <summary>
        /// Builds a forest/tree from a flat collection in O(n) using a parent lookup.
        /// Detects cycles and truncates them to avoid stack overflow on bad data.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <typeparam name="TKey">Id / parent-id type</typeparam>
        /// <param name="collection">Flat items</param>
        /// <param name="idSelector">Item id</param>
        /// <param name="parentIdSelector">Parent id</param>
        /// <param name="rootId">Parent id of root nodes (default: default(TKey), e.g. null or 0)</param>
        public static IEnumerable<TreeItem<T>> GenerateTree<T, TKey>(
            this IEnumerable<T> collection,
            Func<T, TKey> idSelector,
            Func<T, TKey> parentIdSelector,
            TKey rootId = default!)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(idSelector);
            ArgumentNullException.ThrowIfNull(parentIdSelector);

            var items = collection as IList<T> ?? collection.ToList();
            if (items.Count == 0)
            {
                return Array.Empty<TreeItem<T>>();
            }

            var comparer = EqualityComparer<TKey>.Default;
            var childrenByParent = items.ToLookup(parentIdSelector, comparer);
            var visiting = new HashSet<TKey>(comparer);

            TreeItem<T> Build(T item)
            {
                var id = idSelector(item);
                IList<TreeItem<T>> children;

                if (!visiting.Add(id))
                {
                    // Cycle: return node without descending again.
                    children = new List<TreeItem<T>>();
                }
                else
                {
                    children = childrenByParent[id].Select(Build).ToList();
                    visiting.Remove(id);
                }

                return new TreeItem<T>
                {
                    Item = item,
                    Children = children
                };
            }

            return childrenByParent[rootId].Select(Build).ToList();
        }
    }
}
