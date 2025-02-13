using Damco.Model.DataSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Damco.Model
{
    public static class DataSourceExtensions
    {

        public static IEnumerable<Tuple<string, Type>> GetKeysInOutput(this IEnumerable<DataField> fields)
        {
            ParameterExpression dummyParam = Expression.Parameter(fields.First().DataSource.EntityType, "p");
            if (fields.Any(f => f.AggregateField))
                return fields.Where(f => !f.AggregateField).Select(f => Tuple.Create($"K{f.Id}", f.GetSource(dummyParam).Body.Type));
            else
                return fields.GetKeysInInput().Select(f => Tuple.Create(f.OutputName, f.DataField.GetSource(dummyParam).Body.Type));
        }
        private static IEnumerable<OutputField> GetKeysInInput(this IEnumerable<DataField> fields)
        {
            var dataSource = fields.First().DataSource;
            var param = Expression.Parameter(dataSource.EntityType, "p");
            IEnumerable<Tuple<string, LambdaExpression>> keyFields;
            if (dataSource.StoreDataSource != null)
                keyFields = dataSource.StoreDataSource.Fields.Where(f => f.PartOfKey).Select(f => Tuple.Create(f.NameInDataSource, DynamicEntity.GetValueGetterLambda(param, f.NameInDataSource, f.DataType)));
            else if (param.Type == typeof(DynamicEntity)) //Code data source returning a dynamic entity
                keyFields = Enumerable.Empty<Tuple<string, LambdaExpression>>();
            else
                keyFields = ModelUtils.GetPrimaryKeyProperties(dataSource.EntityType).Select(p => Tuple.Create(p.Name, Expression.Lambda(Expression.PropertyOrField(param, p.Name), param)));
            foreach (var key in keyFields)
                yield return GetOutputField(key.Item1, key.Item2, dataSource, "K");
            //Add foreign keys for used related objects
            List<string> doneForeignKeys = new List<string>();
            foreach (var field in fields)
            {
                var body = field.GetSource(param).Body;
                if (body is MemberExpression)
                {
                    var parent = ((MemberExpression)body).Expression;
                    if (parent is MemberExpression)
                    {
                        //TODO: Other field names besides ...Id
                        //TODO: More levels
                        string propertyName = $"{((MemberExpression)parent).Member.Name}Id";
                        if (!doneForeignKeys.Contains(propertyName)
                            && dataSource.EntityType.GetProperties().Any(p => p.Name == propertyName))
                        {
                            yield return GetOutputField(propertyName, Expression.Lambda(Expression.Property(param, propertyName), param), dataSource, "A");
                            doneForeignKeys.Add(propertyName);
                        }
                    }
                    else if (parent is MethodCallExpression)
                    {
                        var firstOrDefault = (MethodCallExpression)parent;
                        var collectionProperty = firstOrDefault.Arguments.First() as MemberExpression;
                        if (collectionProperty != null && collectionProperty.Expression == param) //e.g. party.Contacts.FirstOrDefault().Something
                        {
                            OutputField result = null;
                            try
                            {
                                //First child
                                //string propertyName = $"{((MemberExpression)parent).Member.Name}Id";
                                var childType = ((MethodCallExpression)parent).Method.GetGenericArguments().First();
                                var propertyName = collectionProperty.Member.Name + "_FirstOrDefault_Id";
                                if (!doneForeignKeys.Contains(propertyName)
                                    && childType.GetProperties().Any(p => p.Name == "Id"))
                                {
                                    result = GetOutputField(propertyName, Expression.Lambda(Expression.Convert(Expression.Property(firstOrDefault, "Id"), typeof(int?)), param), dataSource, "A");
                                    doneForeignKeys.Add(propertyName);
                                }
                            }
                            catch { } //
                            if (result != null)
                                yield return result;
                        }
                    }
                }
            }
        }

        private static OutputField GetOutputField(string fieldName, LambdaExpression expression, DataSource dataSource, string prefix)
        {
            var fieldSourceAsString = expression.SerializeToString();
            var existingField = dataSource.Fields.FirstOrDefault(f => f.SourceAsString == fieldSourceAsString);
            if (existingField != null)
                return new OutputField()
                {
                    DataField = existingField,
                    OutputName = $"{prefix}{existingField.Id}"
                };
            else
                return new OutputField()
                {
                    DataField = new DataField()
                    {
                        AggregateField = false,
                        DataSource = dataSource,
                        SourceAsString = fieldSourceAsString
                    },
                    OutputName = $"{prefix}{fieldName}"
                };
        }

        public static IQueryable<DynamicEntity> SelectMany<T>(this IQueryable<T> source, TimeZoneInfo timeZone, DataField selectManyField, IEnumerable<DataField> fields, bool addPrimaryKey, bool fieldIdBasedNamesInOutput)
        {
            var selectManyType = selectManyField?.FieldType.GetGenericArguments().First();
            if (selectManyType == null)
                return source.Select(timeZone, fields, addPrimaryKey, fieldIdBasedNamesInOutput);
            else
                return (IQueryable<DynamicEntity>)typeof(DataSourceExtensions).GetMethod(nameof(SelectMany_Tnternal), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    .MakeGenericMethod(typeof(T), selectManyType)
                    .Invoke(null, new object[] { source, timeZone, selectManyField, fields, addPrimaryKey, fieldIdBasedNamesInOutput });
        }
        private static IQueryable<DynamicEntity> SelectMany_Tnternal<T, Tmany>(this IQueryable<T> source, TimeZoneInfo timeZone, DataField selectManyField, IEnumerable<DataField> fields, bool addPrimaryKey, bool fieldIdBasedNamesInOutput)
        {
            return source
                .SelectMany((Expression<Func<T, IEnumerable<Tmany>>>)selectManyField.GetSource(Expression.Parameter(typeof(T), "p")))
                .Select(timeZone, fields, addPrimaryKey, fieldIdBasedNamesInOutput);
        }

        public static IQueryable<DynamicEntity> Select(this IQueryable source, TimeZoneInfo timeZone, IEnumerable<DataField> fields, bool addPrimaryKey, bool fieldIdBasedNamesInOutput)
        {
            var entityType = source.GetType().GetInterfaces().Union(new Type[] { source.GetType() }).SingleOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryable<>)).GetGenericArguments().Single();
            return (IQueryable<DynamicEntity>)typeof(DataSourceExtensions)
                .GetMethods().Single(m => m.Name == nameof(Select) && m.IsGenericMethodDefinition)
                .MakeGenericMethod(entityType)
                .Invoke(null, new object[] { source, timeZone, fields, addPrimaryKey, fieldIdBasedNamesInOutput });
        }

        public static IQueryable<DynamicEntity> Select<T>(this IQueryable<T> source, TimeZoneInfo timeZone, IEnumerable<DataField> fields, bool addPrimaryKey, bool fieldIdBasedNamesInOutput)
        {
            var outputFields = fields.GetOutputFields(timeZone, fieldIdBasedNamesInOutput).ToList();

            //if (fields.Any(f => f.PivotType == PivotType.Column))
            //{
            //    var column = fields.First(f => f.PivotType == PivotType.Column);
            //    //TODO: Support multipel different pivot column sources?
            //    var dates = Tuple.Create(
            //        GetPivotDates(column.PivotColumnSource.Value, timeZone).Min(t => t.Item1),
            //        GetPivotDates(column.PivotColumnSource.Value, timeZone).Max(t => t.Item2)
            //    );
            //    //Filter to only retrieve items that are needed for the data range
            //    var param = Expression.Parameter(typeof(DynamicEntity), "p");
            //    source = source.Where(Expression.Lambda<Func<DynamicEntity, bool>>(
            //        Expression.AndAlso(
            //            Expression.Not(Expression.Equal(column.GetSource(param).Body, Expression.Constant(default(DateTime?), typeof(DateTime?)))),
            //            Expression.AndAlso(
            //                Expression.GreaterThanOrEqual(Expression.Convert(column.GetSource(param).Body, typeof(DateTime)), Expression.Constant(dates.Item1, typeof(DateTime))),
            //                Expression.LessThan(Expression.Convert(column.GetSource(param).Body, typeof(DateTime)), Expression.Constant(dates.Item2, typeof(DateTime)))
            //            )
            //        ),
            //        param
            //    ));
            //}

            IQueryable<DynamicEntity> query;


            if (fields.Any(f => f.AggregateField)) // || f.PivotType == PivotType.Column))
            {
                var param1 = Expression.Parameter(typeof(T), "p");
                var param2 = Expression.Parameter(typeof(IGrouping<DynamicEntity, T>), "g");
                var param2Key = Expression.PropertyOrField(param2, "Key");
                query = source
                    .GroupBy(DynamicEntity.GetConstructorLambda<T>(param1,
                        fields
                            .Where(f => !f.AggregateField) // && f.PivotType != PivotType.Column)
                            .Select(f => new NamedExpression(
                                GetOutputName(f, fieldIdBasedNamesInOutput),
                                GetGroupByExpression(f, param1, timeZone))
                            )
                    ))
                    .Select(DynamicEntity.GetConstructorLambda<IGrouping<DynamicEntity, T>>(param2,
                        outputFields
                        .Select(f => new NamedExpression($"{f.OutputName}", GetOutputAfterGroupSelectExpression(f, param1, param2, param2Key, timeZone, fieldIdBasedNamesInOutput)))
                        .Union(addPrimaryKey
                            ? fields.Where(f => !f.AggregateField).Select(f => new NamedExpression($"K{f.Id}", DynamicEntity.GetValueGetter(param2Key, GetOutputName(f, fieldIdBasedNamesInOutput), f.GetSource(param1).Body.Type)))
                            : Enumerable.Empty<NamedExpression>()
                        )
                    ));
            }
            else
            {
                var param = Expression.Parameter(typeof(T), "p");
                query = source.Select(DynamicEntity.GetConstructorLambda<T>(param,
                    outputFields.Select(f => new NamedExpression($"{f.OutputName}", GetOutputSelectExpression(f, param, timeZone)))
                    .Union(addPrimaryKey
                        ? (fields.GetKeysInInput().Select(f => new NamedExpression(f.OutputName, f.DataField.GetSource(param).Body)))
                        : Enumerable.Empty<NamedExpression>()
                    )
                ));
            }

            //TODO AKO: Pivottables
            //Probably SUM(CASE WHEN X BETWEEN startdate and enddate THEN ... THEN END) situation
            /* Abandoned attempt:
                  var data = this.GetData(dataRequest);

            IEnumerable<IEnumerable<DynamicEntity>> rowsToGenerate;
            if (fieldsWithRequiredBy.Keys.Any(f => f.PivotType == PivotType.Column))
            {
                //Each table row needs to get the records for all columns
                //In other words, we need to group the data on all columns except for the group 
                //and data columns
                ParameterExpression param = Expression.Parameter(typeof(DynamicEntity), "p");
                rowsToGenerate = data.GroupBy(
                    DynamicEntity.GetConstructorLambda<DynamicEntity>(param, fieldsWithRequiredBy.Keys
                        .Where(f => f.PivotType != PivotType.Column && f.PivotType != PivotType.Data && f.PivotType != PivotType.DataWithDedicatedRow)
                        .Select(f =>
                            DynamicEntity.GetValueGetter(param, f.Id.ToString(), f.GetSource(f.AggregateField ? Expression.Parameter(typeof(IGrouping<DynamicEntity, DynamicEntity>), "g") : param).Body.Type)
                        )
                    ).Compile()
                );
            }
            else
                rowsToGenerate = data.Select(d => d.ToSingletonCollection()); //One record per table row

            var outputFields = GetFieldsInOutput(fieldsWithRequiredBy, timeZone);
            var propertyMapping = outputFields.Select((o, i) => new { Key = o.OutputName, Value = i }).ToDictionary(kv => kv.Key, kv => kv.Value);

            List<DynamicEntity> result = new List<DynamicEntity>();
            foreach (var row in rowsToGenerate)
            {
                foreach (var dedicatedRowColumn in fieldsWithRequiredBy.Where(f => f.Key.PivotType == PivotType.DataWithDedicatedRow).DefaultIfEmpty(new KeyValuePair<DataField, int?>(null, null)))
                {
                    DynamicEntity target = new DynamicEntity(propertyMapping);
                    foreach (var fieldInOutput in outputFields)
                    {
                        DynamicEntity sourceData;
                        if (fieldInOutput.RangeStart != null && fieldInOutput.RangeEnd != null)
                            sourceData = row.FirstOrDefault(d => d.GetValue<DateTime?>($"F{fieldInOutput.DataField.Id}") >= (DateTime?)fieldInOutput.RangeStart && d.GetValue<DateTime?>($"F{fieldInOutput.DataField.Id}") < ((DateTime?)fieldInOutput.RangeEnd));
                        else
                            sourceData = row.FirstOrDefault();
                        if (fieldInOutput.IsRowType)
                            target.SetValue(fieldInOutput.OutputName, dedicatedRowColumn.Value ?? dedicatedRowColumn.Key.Id);
                        else if (sourceData != null)
                            target.SetValue(fieldInOutput.OutputName, sourceData.GetValue($"F{fieldInOutput.DataField.Id}"));
                    }
                    result.Add(target);
                }
            }
            */

            return query;
        }

        private static string GetOutputName(DataField field, bool fieldIdBasedNamesInOutput)
        {
            return fieldIdBasedNamesInOutput ? $"F{field.Id}" : field.RealTechnicalName;
        }

        private static Expression GetGroupByExpression(DataField field, ParameterExpression input, TimeZoneInfo timeZone)
        {
            var originalValue = field.GetSource(input).Body;
            var result = originalValue;
            //if (field.PivotType == PivotType.Column && field.PivotColumnSource != null
            //    && (field.PivotColumnSource.Value == PivotColumnSource.Past7Days || field.PivotColumnSource.Value == PivotColumnSource.MonthsCurrentYear))
            //{
            //    if (originalValue.Type != typeof(DateTime) && originalValue.Type != typeof(DateTime?))
            //        throw new InvalidOperationException($"Field '{field.GetSource().ToString()}' must be a date/time value for this setup to work");
            //    if (originalValue.Type == typeof(DateTime?)) //nullable
            //        result = Expression.Convert(result, typeof(DateTime));
            //    //The data needs to be grouped on date level - after conversion to local timezone
            //    var localTime = Expression.Call(
            //            typeof(TimeZoneInfo).GetMethod("ConvertTimeFromUtc", new Type[] { typeof(DateTime), typeof(TimeZoneInfo) }),
            //            result,
            //            Expression.Constant(timeZone, typeof(TimeZoneInfo))
            //        );

            //    if (field.PivotColumnSource.Value == PivotColumnSource.Past7Days)
            //        //Group on date
            //        result = Expression.Property(localTime, "Date");
            //    else
            //    {
            //        //Group on month (first day of the month)
            //        result = Expression.New(
            //            typeof(System.DateTime).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) }),
            //            Expression.Property(localTime, nameof(DateTime.Year)),
            //            Expression.Property(localTime, nameof(DateTime.Month)),
            //            Expression.Constant(1, typeof(int)),
            //            Expression.Constant(0, typeof(int)),
            //            Expression.Constant(0, typeof(int)),
            //            Expression.Constant(0, typeof(int))
            //        );
            //    }

            //    if (originalValue.Type == typeof(DateTime?)) //nullable
            //    {
            //        //Handle nulls and return nullable
            //        result = Expression.Condition(
            //            Expression.Equal(originalValue, Expression.Constant(default(DateTime?), typeof(DateTime?))),
            //            Expression.Constant(default(DateTime?), typeof(DateTime?)),
            //            Expression.Convert(result, typeof(DateTime?))
            //        );
            //    }
            //}
            return result;
        }
        private static Expression GetOutputAfterGroupSelectExpression(OutputField field, ParameterExpression input, ParameterExpression group, Expression groupKey, TimeZoneInfo timeZone, bool fieldIdBasedNamesInOutput)
        {
            if (field.DataField.AggregateField)
                return field.DataField.GetSource(group).Body;
            else
            {
                var originalValue = DynamicEntity.GetValueGetter(groupKey, GetOutputName(field.DataField, fieldIdBasedNamesInOutput), field.DataField.GetSource(input).Body.Type);
                var result = originalValue;
                //if (field.DataField.PivotType == PivotType.Column && field.DataField.PivotColumnSource != null
                //    && (field.DataField.PivotColumnSource.Value == PivotColumnSource.Past7Days || field.DataField.PivotColumnSource.Value == PivotColumnSource.MonthsCurrentYear))
                //{
                //    //We converted from Utc for the grouping, now we need to convert back since all display logic
                //    //assumes Utc
                //    if (originalValue.Type == typeof(DateTime?)) //nullable
                //        result = Expression.Convert(result, typeof(DateTime));
                //    result = Expression.Call(
                //        typeof(TimeZoneInfo).GetMethod("ConvertTimeToUtc", new Type[] { typeof(DateTime), typeof(TimeZoneInfo) }),
                //        result,
                //        Expression.Constant(timeZone, typeof(TimeZoneInfo))
                //    );
                //    if (originalValue.Type == typeof(DateTime?)) //nullable
                //    {
                //        //Handle nulls and return nullable
                //        result = Expression.Condition(
                //            Expression.Equal(originalValue, Expression.Constant(default(DateTime?), typeof(DateTime?))),
                //            Expression.Constant(default(DateTime?), typeof(DateTime?)),
                //            Expression.Convert(result, typeof(DateTime?))
                //        );
                //    }
                //}
                return result;
            }
        }
        private static Expression GetOutputSelectExpression(OutputField field, ParameterExpression input, TimeZoneInfo timeZone)
        {
            return field.DataField.GetSource(input).Body;
        }

        //private static IEnumerable<Tuple<DateTime, DateTime>> GetPivotDates(PivotColumnSource columnSource, TimeZoneInfo timeZone)
        //{
        //    if (columnSource == PivotColumnSource.Past7Days)
        //        for (int intDay = -6; intDay <= 0; intDay++)
        //            yield return Tuple.Create(
        //                DateTime.UtcNow.AddDays(intDay).ToStartOfLocalDay(timeZone),
        //                DateTime.UtcNow.AddDays(intDay + 1).ToStartOfLocalDay(timeZone)
        //            );
        //    else if (columnSource == PivotColumnSource.MonthsCurrentYear)
        //        for (int intMonth = 1; intMonth <= 12; intMonth++)
        //            yield return Tuple.Create(
        //                new DateTime(timeZone.LocalNow().Year, intMonth, 1, 0, 0, 0, DateTimeKind.Local).ToUtc(timeZone),
        //                new DateTime(timeZone.LocalNow().Year, intMonth, 1, 0, 0, 0, DateTimeKind.Local).ToUtc(timeZone).AddMonths(1)
        //            );
        //    else
        //        throw new ArgumentException($"Value {columnSource} is unexpected");
        //}



        public static IEnumerable<OutputField> GetOutputFields(this IEnumerable<DataField> fields, TimeZoneInfo timeZone, bool fieldIdBasedNamesInOutput)
        {
            return GetOutputFields(fields.Select(f => new KeyValuePair<DataField, int?>(f, default(int?))), timeZone, fieldIdBasedNamesInOutput);
        }


        public static IEnumerable<OutputField> GetOutputFields(this IEnumerable<KeyValuePair<DataField, int?>> fieldsWithRequiredBy, TimeZoneInfo timeZone, bool fieldIdBasedNamesInOutput)
        {
            //bool rowTypeDone = false;
            foreach (var field in fieldsWithRequiredBy)
            {
                //if (field.Key.PivotType == PivotType.Column)
                //{
                //    foreach (var dataField in fieldsWithRequiredBy.Where(f => f.Key.PivotType == PivotType.Data).DefaultIfEmpty(new KeyValuePair<DataField, int?>(null, null)))
                //        foreach (var dates in GetPivotDates(field.Key.PivotColumnSource.Value, timeZone))
                //        {
                //            var param = Expression.Parameter(typeof(DynamicEntity), "p");
                //            yield return new OutputField()
                //            {
                //                RangeStart = dates.Item1,
                //                RangeEnd = dates.Item2,
                //                DataField = dataField.Key,
                //                OutputName = $"F{field.Key.Id}_" + dates.Item1.ToString("yyyyMMddHHmmssffffff"),
                //                RequiredById = field.Value
                //            };
                //        }
                //}
                //else if (field.Key.PivotType == PivotType.DataWithDedicatedRow && !rowTypeDone)
                //{
                //    yield return new OutputField()
                //    {
                //        OutputName = $"F_ROWTYPE",
                //        IsRowType = true,
                //        RequiredById = field.Value
                //    };
                //    rowTypeDone = true;
                //}
                //else if (field.Key.PivotType != PivotType.Data && field.Key.PivotType != PivotType.DataWithDedicatedRow) //Normal

                yield return new OutputField()
                {
                    OutputName = GetOutputName(field.Key, fieldIdBasedNamesInOutput),
                    DataField = field.Key,
                    RequiredById = field.Value

                };
                //}
            }
        }
    }
}
