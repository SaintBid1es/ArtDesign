using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ArtDesign.Model.Attributes;

namespace ArtDesign.Helpers
{
    public static class SearchFilterSortHelper
    {
        /// <summary>
        /// Применяет "поиск" по строковым полям, помеченным [Searchable].
        /// Для каждого такого поля (string) генерируется условие Contains(searchTerm).
        /// Все условия объединяются по OR.
        /// </summary>
        public static IQueryable<T> Search<T>(IQueryable<T> query, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return query;

            // собираем все свойства, помеченные [Searchable], и являющиеся string
            var searchableProps = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<SearchableAttribute>() != null 
                            && p.PropertyType == typeof(string));

            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            Expression accumulatedOr = null;

            foreach (var prop in searchableProps)
            {
                var propertyAccess = Expression.Property(param, prop);
                // Защита от null: (x.Prop != null) && x.Prop.Contains(searchTerm)
                var notNullExpr = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var searchTermExp = Expression.Constant(searchTerm, typeof(string));
                if (containsMethod == null) continue;
                var containsCall = Expression.Call(propertyAccess, containsMethod, searchTermExp);
                var notNullAndContains = Expression.AndAlso(notNullExpr, containsCall);

                if (accumulatedOr == null)
                    accumulatedOr = notNullAndContains;
                else
                    accumulatedOr = Expression.OrElse(accumulatedOr, notNullAndContains);
            }

            if (accumulatedOr == null)
                return query; // нет свойств для поиска

            var lambda = Expression.Lambda<Func<T, bool>>(accumulatedOr, param);
            return query.Where(lambda);
        }

        /// <summary>
        /// Применяет фильтрацию по свойствам, помеченным [Filterable].
        /// Передаём словарь (propertyName -> value).
        /// Для каждого такого свойства генерируется условие равенства (x => x.property == value).
        /// Все условия объединяются через AND.
        /// </summary>
        public static IQueryable<T> Filter<T>(IQueryable<T> query, Dictionary<string, object> filters)
        {
            if (filters == null || filters.Count == 0)
                return query;

            var param = Expression.Parameter(typeof(T), "x");
            Expression accumulatedAnd = null;

            foreach (var equalExpr in from kvp in filters
                     let propertyName = kvp.Key
                     let filterValue = kvp.Value
                     let prop = typeof(T).GetProperty(propertyName,
                         BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                     where prop != null
                     where prop.GetCustomAttribute<FilterableAttribute>() != null
                     let left = Expression.Property(param,
                         prop)
                     let right = Expression.Constant(filterValue,
                         prop.PropertyType)
                     select Expression.Equal(left,
                         right))
            {
                accumulatedAnd = accumulatedAnd == null ? equalExpr : Expression.AndAlso(accumulatedAnd, equalExpr);
            }

            if (accumulatedAnd == null)
                return query;

            var lambda = Expression.Lambda<Func<T, bool>>(accumulatedAnd, param);
            return query.Where(lambda);
        }

        /// <summary>
        /// Применяет сортировку по имени свойства, помеченного [Sortable].
        /// sortPropertyName – имя свойства,
        /// ascending – true/false.
        /// </summary>
        public static IQueryable<T> Sort<T>(IQueryable<T> query, string sortPropertyName, bool ascending)
        {
            if (string.IsNullOrEmpty(sortPropertyName))
                return query;

            var prop = typeof(T).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<SortableAttribute>() != null
                                     && p.Name.Equals(sortPropertyName, StringComparison.OrdinalIgnoreCase));

            if (prop == null)
                return query; // нет подходящего свойства

            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(param, prop);
            var orderByLambda = Expression.Lambda(propertyAccess, param);

            string methodName = ascending ? "OrderBy" : "OrderByDescending";
            var resultExp = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), prop.PropertyType },
                query.Expression,
                Expression.Quote(orderByLambda));

            return query.Provider.CreateQuery<T>(resultExp);
        }
    }
}