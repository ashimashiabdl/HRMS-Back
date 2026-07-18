using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HR.SharedKernel.Extensions
{
    public static class DateValidityExtension<T> where T : BaseEntity
    {
        public static Expression<Func<T, bool>> GetDateValidationPredicate(bool IgnoreExpired = true, DateTime? ImpleDate = null)
        {
            if (IgnoreExpired)
            {
                if (ImpleDate == null)
                {
                    ImpleDate = DateTime.Now.Date;
                }
                var predicate = PredicateBuilder.New<T>();
             
                return predicate.And(a => ( (a.StartDate != null &&  a.StartDate.Value.Date <= ImpleDate.Value.Date) || a.StartDate == null) && (a.EndDate > ImpleDate.Value.Date || a.EndDate == null)).And(i => i.IsDeleted != true);
            }
            else
            {
                return PredicateBuilder.New<T>(i => i.IsDeleted != true);
            }
        }



        public static Expression<Func<T, bool>> CheckDateRangeOverLap(T Entity)
        {
            if (Entity.StartDate == null) return PredicateBuilder.New<T>(i => i.IsDeleted != true);
            var type = typeof(T);
            var predicate = PredicateBuilder.New<T>(i =>
            (Entity.StartDate <= i.StartDate && (Entity.EndDate >= i.StartDate)) ||
            (Entity.StartDate <= i.StartDate && ((Entity.EndDate >= i.EndDate || Entity.EndDate == null) || (i.EndDate == null && (Entity.EndDate >= i.StartDate || Entity.EndDate == null)))) ||
            (Entity.StartDate == i.StartDate) ||
            (Entity.StartDate >= i.StartDate && ((Entity.EndDate <= i.EndDate || Entity.EndDate == null) || i.EndDate == null)) ||
            (Entity.StartDate >= i.StartDate && ((Entity.EndDate >= i.EndDate || Entity.EndDate == null) || i.EndDate == null))
            );

            return predicate.And(i => i.IsDeleted != true);
        }


        public static Expression<Func<T, bool>> CheckEntityUniqueEffectiveColumns(T Entity)
        {
            if (Entity.StartDate == null) return PredicateBuilder.New<T>(false);
            var type = typeof(T);
            var predicate = PredicateBuilder.New<T>();
            foreach (var property in type.GetProperties())
            {
                if (property.CustomAttributes.Where(i => i.AttributeType == typeof(IsEffectiveInDateOverLapChecking)).Any())
                {
                    if (Convert.ToBoolean(property.CustomAttributes.Where(i => i.AttributeType == typeof(IsEffectiveInDateOverLapChecking)).SingleOrDefault().NamedArguments.Where(i => i.MemberName == "IsEffective").SingleOrDefault().TypedValue.Value) == true)
                    {
                        predicate = predicate.And(i => property.GetValue(i) == Share.Helper.GetPropValue(Entity, property.Name));
                    }
                }
            }
            return predicate;
        }

    }




}
