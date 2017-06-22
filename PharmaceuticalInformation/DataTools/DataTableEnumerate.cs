extern alias SLD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Dynamic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections;
using sld = SLD::System.Linq.Dynamic;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using ServerOfSystem.PharmaceuticalInformation.Model;
//using  sld = System.Linq.Dynamic;

namespace ServerOfSystem.PharmaceuticalInformation.DataTools
{
    public partial class LocalDataContext : DataContext
    {
        public LocalDataContext(IDbConnection connection) : base(connection) {}

        [Function(Name = "GetDate", IsComposable = true)]
        public DateTime GetSystemDate()
        {
            MethodInfo mi = MethodBase.GetCurrentMethod() as MethodInfo;
            return (DateTime)this.ExecuteMethodCall(this, mi, new object[] { }).ReturnValue;
        }
    }

    public static class DataTableEnumerate
    {
        public static bool ret_true(this price_list pl)
        {
            return pl.Is_deleted;
        }

        public static bool JoinDataTableProd(ref DataTable DT_Join, dynamic id, string col_name)
        {
            IEnumerable<dynamic> dt_en = DT_Join.AsEnumerable();
            IEnumerable<dynamic> dt_enw = dt_en.Where(idm => (int)idm.Id_Product == (int)id);
            return dt_enw.Any();
            //return true;
        }

        public static bool JoinDataTable(this DataTable DT_Join, dynamic id, string col_name)
        {
            return DT_Join.AsEnumerable().Where(idm => (int) idm.GetType().GetProperty(col_name).GetValue(idm) == (int) id).Any();
        }

        public static void Fill<T> (this IEnumerable<T> Ts, ref DataTable dt) where T : class
        {
            //Get Enumerable Type
            Type tT = typeof(T);

            //Get Collection of NoVirtual properties
            var T_props = tT.GetProperties().Where(p => !p.GetGetMethod().IsVirtual).ToArray();

            //Fill Schema
            foreach (PropertyInfo p in T_props)
                dt.Columns.Add(p.Name, p.GetMethod.ReturnParameter.ParameterType.BaseType);

            //Fill Data
            foreach (T t in Ts)
            {
                DataRow row = dt.NewRow();

                foreach (PropertyInfo p in T_props)
                    row[p.Name] = p.GetValue(t); // tT.GetProperty(p.Name).GetValue(t);

                dt.Rows.Add(row);
            }

        }

        public static IEnumerable<dynamic> AsEnumerable(this DataTable dt)
        {
            List<dynamic> result = new List<dynamic>();
            Dictionary<string, object> d;
            foreach (DataRow dr in dt.Rows)
            {
                d = new Dictionary<string, object>();

                foreach (DataColumn dc in dt.Columns)
                    d.Add(dc.ColumnName, dr[dc]);

                result.Add(GetDynamicObject(d));
            }
            return result.AsEnumerable<dynamic>();
        }

        public static dynamic GetDynamicObject(Dictionary<string, object> properties)
        {
            return new MyDynObject(properties);
        }

        public sealed class MyDynObject : DynamicObject
        {
            private readonly Dictionary<string, object> _properties;

            public MyDynObject(Dictionary<string, object> properties)
            {
                _properties = properties;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return _properties.Keys;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (_properties.ContainsKey(binder.Name))
                {
                    result = _properties[binder.Name];
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                if (_properties.ContainsKey(binder.Name))
                {
                    _properties[binder.Name] = value;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        class FooConverter : ExpandableObjectConverter
        {
            private static readonly List<Tuple<string, Type>> customProps = new List<Tuple<string, Type>>();
            public static void AddProperty(string name, Type type)
            {
                lock (customProps) customProps.Add(Tuple.Create(name, type));
            }
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, System.Attribute[] attributes)
            {
                var orig = base.GetProperties(context, value, attributes);
                lock (customProps)
                {
                    if (customProps.Count == 0) return orig;

                    PropertyDescriptor[] props = new PropertyDescriptor[orig.Count + customProps.Count];
                    orig.CopyTo(props, 0);
                    int i = orig.Count;
                    foreach (var prop in customProps)
                    {
                        props[i++] = new SimpleDescriptor(prop.Item1, prop.Item2);
                    }
                    return new PropertyDescriptorCollection(props);
                }
            }
            class SimpleDescriptor : PropertyDescriptor
            {
                private readonly Type type;
                public SimpleDescriptor(string name, Type type)
                    : base(name, new Attribute[0])
                {
                    this.type = type;
                }
                public override Type PropertyType { get { return type; } }
                public override bool SupportsChangeEvents { get { return false; } }
                public override void ResetValue(object component) { SetValue(component, null); }
                public override bool CanResetValue(object component) { return true; }
                public override bool ShouldSerializeValue(object component) { return GetValue(component) != null; }
                public override bool IsReadOnly { get { return false; } }
                public override Type ComponentType { get { return typeof(Foo); } }
                public override object GetValue(object component) { return ((Foo)component).GetExtraValue(Name); }
                public override void SetValue(object component, object value) { ((Foo)component).SetExtraValue(Name, value); }
                public override string Category { get { return "Extra values"; } }
            }
        }

        [TypeConverter(typeof(FooConverter))]
        public class Foo
        {
            Dictionary<string, object> extraValues;
            internal object GetExtraValue(string name)
            {
                object value;
                if (extraValues == null || !extraValues.TryGetValue(name, out value)) value = null;
                return value;
            }
            internal void SetExtraValue(string name, object value)
            {
                if (extraValues == null && value != null) extraValues = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
                if (value == null) extraValues.Remove(name);
                else extraValues[name] = value;
            }
            public int Id { get; set; }
            public string Name { get; set; }
        }

        //public static IQueryable Join(this IQueryable outer, IEnumerable inner, string outerSelector, string innerSelector, string resultsSelector, params object[] values)
        //{
        //    if (inner == null) throw new ArgumentNullException("inner");
        //    if (outerSelector == null) throw new ArgumentNullException("outerSelector");
        //    if (innerSelector == null) throw new ArgumentNullException("innerSelector");
        //    if (resultsSelector == null) throw new ArgumentNullException("resultsSelctor");


        //    LambdaExpression outerSelectorLambda = sld.DynamicExpression .ParseLambda(outer.ElementType, null, outerSelector, values);
        //    LambdaExpression innerSelectorLambda = sld.DynamicExpression.ParseLambda(inner.AsQueryable().ElementType, null, innerSelector, values);

        //    ParameterExpression[] parameters = new ParameterExpression[] {
        //    Expression.Parameter(outer.ElementType, "outer"), Expression.Parameter(inner.AsQueryable().ElementType, "inner") };
        //    LambdaExpression resultsSelectorLambda = sld.DynamicExpression.ParseLambda(parameters, null, resultsSelector, values);

        //    return outer.Provider.CreateQuery(
        //        Expression.Call(
        //            typeof(Queryable), "Join",
        //            new Type[] { outer.ElementType, inner.AsQueryable().ElementType, outerSelectorLambda.Body.Type, resultsSelectorLambda.Body.Type },
        //            outer.Expression, inner.AsQueryable().Expression, Expression.Quote(outerSelectorLambda), Expression.Quote(innerSelectorLambda), Expression.Quote(resultsSelectorLambda)));
        //}


        ////The generic overload.
        //public static IQueryable<T> Join<T>(this IQueryable<T> outer, IEnumerable<T> inner, string outerSelector, string innerSelector, string resultsSelector, params object[] values)
        //{
        //    return (IQueryable<T>)Join((IQueryable)outer, (IEnumerable)inner, outerSelector, innerSelector, resultsSelector, values);
        //}
    }
}
