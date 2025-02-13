//Object structure - IQueryable<T> - that stores the linq actions (Select, Where etc) done on it.
//This can be used to store or communicate the details for a request of data
//The class is serializable.
using Damco.Model;
using Damco.Model.DataSourcing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{

    public static class DataRequestExtensions
    {
        public static IQueryable<T> AllowUseOfCache<T>(this IQueryable<T> dataRequest, bool allowUseOfCache = true)
        {
            if (!(dataRequest is DataRequest<T>))
                throw new ArgumentException($"'{nameof(dataRequest)}' must be of type DataRequest<T>", nameof(dataRequest));
            ((DataRequest<T>)dataRequest).AllowUseOfCache = allowUseOfCache;
            return dataRequest;
        }
        public static IQueryable<T> NoLock<T>(this IQueryable<T> dataRequest, bool noLock = true)
        {
            if (!(dataRequest is DataRequest<T>))
                throw new ArgumentException($"'{nameof(dataRequest)}' must be of type DataRequest<T>", nameof(dataRequest));
            ((DataRequest<T>)dataRequest).NoLock = noLock;
            return dataRequest;
        }

        //private class SimpleReplaceExpressionVisitor : ExpressionVisitor
        //{
        //    Func<Expression, bool> _predicate;
        //    Expression _replacement;
        //    public SimpleReplaceExpressionVisitor(Func<Expression, bool> predicate, Expression replacement)
        //    {
        //        _predicate = predicate;
        //        _replacement = replacement;
        //    }
        //    public override Expression Visit(Expression node)
        //    {
        //        if (_predicate(node))
        //            return _replacement;
        //        else
        //            return base.Visit(node);
        //    }
        //}
        //public static IQueryable<Toutput> ApplyQuery<Toutput>(this IQueryable source, IQueryable<Toutput> query)
        //{
        //    return source.Provider.CreateQuery<Toutput>(
        //        new SimpleReplaceExpressionVisitor(
        //            e => e is ConstantExpression && typeof(IQueryable).IsAssignableFrom(e.Type),
        //            source.Expression
        //        ).Visit(query.Expression)
        //    );
        //}

        //TODO: Make working and re-enable (it's tricky)
        //public static IEnumerable<Toutput> ApplyQuery<Toutput>(this IEnumerable source, IQueryable<Toutput> query)
        //{
        //    var param = Expression.Parameter(typeof(IEnumerable), "s");
        //    var expression = query.Expression;
        //    expression = new SimpleReplaceExpressionVisitor(
        //            e => e is ConstantExpression && typeof(IQueryable).IsAssignableFrom(e.Type),
        //            param
        //        ).Visit(query.Expression);
        //    return Expression.Lambda<Func<IEnumerable, IEnumerable<Toutput>>>(query.Expression, param).Compile().Invoke(source);
        //}

    }

    public abstract class DataRequest
    {
        protected Expression Expression { get; set; }

        public static IQueryable<T> Get<T>()
        {
            return new DataRequest<T>(ExpressionSerialization.GetNameForType(typeof(T)), default(int?), default(string), default(Dictionary<string, object>), default(IQueryProvider));
        }
        public static IQueryable<DynamicEntity> Get(int dataSourceId, string nameOrConnectionString = null) 
            => new DataRequest<DynamicEntity>(
                default(string),
                dataSourceId,
                default(string),
                default(Dictionary<string, object>),
                default(IQueryProvider),
                nameOrConnectionString);

        public static IQueryable<T> Get<T>(
            string codeDataSourceTag,
            Dictionary<string, object> codeDataSourceParameters = null,
            string nameOrConnectionString = null) 
                => new DataRequest<T>(
                    ExpressionSerialization.GetNameForType(typeof(T)),
                    default(int?),
                    codeDataSourceTag,
                    codeDataSourceParameters,
                    default(IQueryProvider),
                    nameOrConnectionString);

        public static IQueryable<T> Get<T>(DataSource dataSource, Dictionary<string, object> codeDataSourceParameters = null)
        {
            if (typeof(T) == typeof(DynamicEntity) && dataSource.StoreDataSource != null)
                return (IQueryable<T>)Get(dataSource.StoreDataSource.Id);
            else if (dataSource.CodeDataSourceTag != null)
                return Get<T>(dataSource.CodeDataSourceTag, codeDataSourceParameters);
            else
                return Get<T>();
        }

        int? _dataSourceId;
        public int? DataSourceId
        {
            get { SetSource(); return _dataSourceId; }
            protected set { _dataSourceId = value; }
        }
        string _entityName;
        public string EntityName
        {
            get { SetSource(); return _entityName; }
            protected set { _entityName = value; }
        }

        bool _allowUseOfCache = false;
        public bool AllowUseOfCache
        {
            get { SetSource(); return _allowUseOfCache; }
            set { _allowUseOfCache = value; }
        }

        bool _noLock = false;
        public bool NoLock
        {
            get { SetSource(); return _noLock; }
            set { _noLock = value; }
        }

        string _codeDataSourceTag;
        public string CodeDataSourceTag
        {
            get { SetSource(); return _codeDataSourceTag; }
            protected set { _codeDataSourceTag = value; }
        }
        Dictionary<string, object> _codeDataSourceParameters;
        public Dictionary<string, object> CodeDataSourceParameters
        {
            get { SetSource(); return _codeDataSourceParameters; }
            protected set { _codeDataSourceParameters = value; }
        }

        /// <summary>
        /// Optional name or connection string to change the default one for the query
        /// </summary>
        public string NameOrConnectionString { get; protected set; }

        private void SetSource()
        {
            if (_dataSourceId == null && _entityName == null && _codeDataSourceTag == null)
            {
                //Find the root data source and get it from that
                var current = this.Expression;
                var currentMethodCall = current as MethodCallExpression;
                while (currentMethodCall != null)
                {
                    if (currentMethodCall.Object == null)
                        current = currentMethodCall.Arguments.First();
                    else
                        current = currentMethodCall.Object;
                    currentMethodCall = current as MethodCallExpression;
                }
                var rootRequest = (DataRequest)((ConstantExpression)current).Value;
                this.DataSourceId = rootRequest.DataSourceId;
                this.EntityName = rootRequest.EntityName;
                this.CodeDataSourceTag = rootRequest.CodeDataSourceTag;
                this.CodeDataSourceParameters = rootRequest.CodeDataSourceParameters;
                this.AllowUseOfCache = rootRequest.AllowUseOfCache;
                this.NoLock = rootRequest.NoLock;
                this.NameOrConnectionString = rootRequest.NameOrConnectionString;
            }
        }
    }

    [DataContract()]
    public class DataRequest<T> : DataRequest, IOrderedQueryable<T>, IQueryProvider
    {
        IQueryProvider _provider = null;

        public DataRequest(Expression expression, IQueryProvider provider)
        {
            this.Expression = expression;
            _provider = provider;
        }

        public DataRequest(
            string entityName,
            int? dataSourceId,
            string codeDataSourceTag,
            Dictionary<string, object> codeDataSourceParameters,
            IQueryProvider provider,
            string nameOrConnectionString = null)
        {
            this.EntityName = entityName;
            this.DataSourceId = dataSourceId;
            this.CodeDataSourceTag = codeDataSourceTag;
            this.CodeDataSourceParameters = codeDataSourceParameters;
            _provider = provider;
            this.Expression = Expression.Constant(this, typeof(IQueryable<T>));
            NameOrConnectionString = nameOrConnectionString;
        }

        internal DataRequest(object[] array)
        {
            this.Expression = array.DeserializeExpression();
        }

        public object[] ToObjectArray()
        {
            return this.Expression.SerializeToObjectArray();
        }

        public static DataRequest<T> Parse(string s)
        {
            return Parse(s.FromJson<object[]>());
        }
        public static DataRequest<T> Parse(object[] array)
        {
            return new DataRequest<T>(array);
        }
        public override string ToString()
        {
            return this.ToObjectArray().ToJson();
        }


        #region IQueryable
        Expression IQueryable.Expression { get { return this.Expression; } }

        Type IQueryable.ElementType { get { return typeof(T); } }

        IQueryProvider IQueryable.Provider { get { return _provider ?? this; } }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (_provider == null)
                throw new InvalidOperationException("DataRequest<T> only stores operations. Executions are not supported. Use the ApplyQuery extension method on an IQueryable to apply the stored operations on it.");
            else
                return _provider.Execute<IEnumerator<T>>(this.Expression);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_provider == null)
                throw new InvalidOperationException("DataRequest<T> only stores operations. Executions are not supported. Use the ApplyQuery extension method on an IQueryable to apply the stored operations on it.");
            else
                return _provider.Execute<IEnumerator<T>>(this.Expression);
        }

        #endregion

        #region IQueryProvider
        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return (IQueryable)typeof(IQueryProvider).GetMethods().Where(m => m.Name == "CreateQuery" && m.IsGenericMethod).Single()
                .MakeGenericMethod(GetLambdaParameterType(expression.Type))
                .Invoke(this, new object[] { expression });
        }

        private Type GetLambdaParameterType(Type inputType)
        {
            if (inputType.IsGenericType
                && inputType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    || inputType.GetGenericTypeDefinition() == typeof(IQueryable<>))
                return inputType.GetGenericArguments().First();
            else
                return inputType;
        }

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return new DataRequest<TElement>(expression, null);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            throw new InvalidOperationException("DataRequest<T> only stores operations. Executions are not supported. Use the ApplyQuery extension method on an IQueryable to apply the stored operations on it.");
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            throw new InvalidOperationException("DataRequest<T> only stores operations. Executions are not supported. Use the ApplyQuery extension method on an IQueryable to apply the stored operations on it.");
        }

        #endregion


    }
}
