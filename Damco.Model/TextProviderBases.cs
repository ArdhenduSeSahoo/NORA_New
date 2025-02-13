//Base classes that can be inherited to facilitate the translation of texts in the application
//This provides a uniform way for applications to be completely translatable and facilitate in an easy way for developers to 
//reference a specific text to show to the user.

//The idea is that each application inherits RootTextProvider and that there is one instance per user of it.
//at all times, if a developer needs to show a text to the user, the text provider is used to retrieve
//the text. It has an hierarchical structure and texts are easy to find via intellisense.

//The RootTextProvider has a StoreTexts property that is a dictionary of text paths (IEnumerable<string> unique for each text) 
//and translations. The default - English - text is always determined hard-coded and used if the dictionary is not provided or 
//if the specific text path does not exist in the dictionary.

//These are the types used:
//- GroupTextProvider: Has properties or methods to return other text providers. Used to categorize texts.
//  Note RootTextProvider is also a GroupTextProvider.
//- MessageTextProvider contains methods and properties that return actual texts. The properties just return the text, the methods
//  have parameters that are replaced inside the text.
//- EntityTextProvider represents a single entity. It has entity-level information like Name and Description but also method that
//  returns a ModelTextProvider of a given property. The RootTextProvider base class has an Entity method that can be used to get 
//  the EntityTextProvider for any entity in the application. It's default values are taken from the Entity attribute of the entity 
//  class.
//- ModelTextProvider represents a code item (class or property). It has information like Name and Description. Note EntityTextProvider
//  inherits this class.

//Examples code in text providers:

//Root text provider / group text provider:
//public class TextProvider : RootTextProvider<TextProvider>
//{
//    public LogsTextProvider Logs { get { return GetProvider<LogsTextProvider>(); } }
//    public ApplicationMonitoringTextProvider ApplicationMonitoring { get { return GetProvider<ApplicationMonitoringTextProvider>(); } }
//    public override Damco.Model.ApplicationInfoTextProvider ApplicationInfo { get { return GetProvider<ApplicationInfoTextProvider>(); } }
//}

//Message text provider
//public class LogsTextProvider : MessageTextProvider
//{
//    public string NotFound { get { return GetText("Log was not found"); } }
//    public string ValueNotFilledIn(TextPlaceHolder field) { return GetText("{0} is not filled in", Params(field)); }
//    public string Down(string applicationName, DateTime dateTime) { return GetText("Application {0} is down since {1}", Params(applicationName, dateTime)); }
//}

//Examples on retrieving texts from these providers (_textProvider is an instance of RootTextProvider):
//string text = _textProvider.Logs.NotFound; //"Log was not found"
//string text = _textProvider.Logs.Down("ILSE", new DateTime(2000, 1, 1, 14, 55, 26, DateTimeKind.Utc))); //"Application ILSE is down since 1/1/2000 6:55:26 AM"
//string text = _textProvider.Entity<Customer>().Description; //"A company that we sell goods or services to" (set via Entity(Description="A company that we sell goods or services to") attribute)
//string text = _textProvider.Entity<Customer>().Property(x => x.AddDateTime).Name; //"Add date/time" (based on default pascalcase-to-display translation)
//string text = _textProvider.Logs.ValueNotFilledIn(TextProvider.GetPlaceHolder(tp => tp.Entity<Customer>().Property(c => c.EditDateTime).Name))); //"Modified date/time is not filled in"

using Damco.Model;
using Damco.Model.ScreenTemplating;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Damco.Model
{
    public abstract class TextProviderBase
    {

        protected TextProviderBase(RootTextProvider root, IEnumerable<string> path)
        {
            this.Root = root;
            this.Path = path;
        }
        protected internal RootTextProvider Root { get; set; }
        protected internal IEnumerable<string> Path { get; set; }

        protected internal abstract void RecursiveTouchAllTexts();

        //Find the default text - top to bottom
        protected internal abstract string GetTextByPath(IEnumerable<string> textPath, params object[] parameters);

        protected internal object[] Params(params object[] parameters)
        {
            return parameters;
        }

        //protected internal abstract void SetTexts(IEnumerable<KeyValuePair<string[], string>> source, int myPlaceInPath);

        //internal static IEnumerable<string> AddItemToPathStart(IEnumerable<string> path, string item)
        //{
        //    yield return item;
        //    foreach (string pathPiece in path)
        //        yield return pathPiece;
        //}
        internal static IEnumerable<string> AddItemToPathEnd(IEnumerable<string> path, string item)
        {
            foreach (string pathPiece in path)
                yield return pathPiece;
            yield return item;
        }

        public static string MakePlural(string value)
        {
            if (value == null)
                return null;
            if (value.EndsWith("y") && !value.EndsWith("ey"))
                //Change y into ies (e.g. "Company" becomes "Companies") - but don't e.g. change Key in Keies
                return value.Substring(0, value.Length - 1) + "ies";
            else if (value.EndsWith("s") || value.EndsWith("ch"))
                //Add es
                return value + "es";
            else
                //Just add an "s"
                return value + "s";
        }

        public static string DisplayTextToPascal(string name)
        {
            StringBuilder result = new StringBuilder();
            bool upperCase = true;
            foreach (var c in name)
            {
                if (char.IsWhiteSpace(c))
                    upperCase = true;
                else if (upperCase)
                {
                    result.Append(char.ToUpper(c));
                    upperCase = false;
                }
                else
                    result.Append(c);
            }
            //TODO: Remove special characters
            return result.ToString();
        }

        public static string PascalToDisplayText(Type enumType, Enum value, bool shortVersion)
        {
            if(!enumType.IsEnum)
                throw new ArgumentException($"{nameof(enumType)} must be an enum", nameof(enumType));
            return PascalToDisplayText(Enum.GetName(enumType, value), false);
        }

        public static string PascalToDisplayText(string value, bool shortVersion)
        {
            if (value.Contains("DateTime"))
                value = value.Replace("DateTime", (shortVersion ? "Dt/tm" : "Date/time"));
            StringBuilder strbldResult = new StringBuilder();
            bool blnPreviousIsUpperCase = false;
            for (int intChar = 0; intChar < value.Length; intChar++)
            {
                bool blnNextIsLowerCase = (value.Length > (intChar + 1)) && Char.IsLower(value[intChar + 1]);
                if (intChar > 0 && (!blnPreviousIsUpperCase || blnNextIsLowerCase) && Char.IsUpper(value[intChar]))
                    strbldResult.Append(" " + (!blnNextIsLowerCase ? value[intChar] : Char.ToLower(value[intChar])));
                else
                    strbldResult.Append(value[intChar]);
                blnPreviousIsUpperCase = (char.IsUpper(value[intChar]));
            }
            return strbldResult.ToString();
        }
    }

    public abstract class GroupTextProvider : TextProviderBase
    {
        protected GroupTextProvider(RootTextProvider root, IEnumerable<string> path) : base(root, path) { }

        ConcurrentDictionary<string, EntityTextProvider> _entities = new ConcurrentDictionary<string, EntityTextProvider>();
        protected virtual EntityTextProvider Entity_Internal(string nameSpace, string entityName)
        {
            return (EntityTextProvider)_entities.GetOrAdd(entityName, name =>
            {
                Type entityType = this.GetType().Assembly.GetType(string.Concat(nameSpace + "." + name));
                return (EntityTextProvider)typeof(EntityTextProvider<>)
                    .MakeGenericType(entityType)
                    .GetConstructor(new Type[] { typeof(RootTextProvider), typeof(IEnumerable<string>) })
                    .Invoke(new object[] { this.Root, AddItemToPathEnd(AddItemToPathEnd(this.Path, "Entity"), name) });
            });
        }
        protected virtual EntityTextProvider<T> GetEntity<T>()
        {
            return (EntityTextProvider<T>)_entities.GetOrAdd(typeof(T).Name, type =>
            {
                return new EntityTextProvider<T>(this.Root, AddItemToPathEnd(AddItemToPathEnd(this.Path, "Entity"), type));
            });
        }

        ConcurrentDictionary<string, TextProviderBase> _providers = new ConcurrentDictionary<string, TextProviderBase>();
        protected T GetProvider<T>([CallerMemberName]string providerCode = "<default is set to member name by the compiler>") where T : TextProviderBase, new()
        {
            return (T)_providers.GetOrAdd(providerCode, code =>
            {
                TextProviderBase result = new T();
                result.Root = this.Root;
                result.Path = AddItemToPathEnd(this.Path, code);
                _providers[providerCode] = result;
                return (T)result;
            });
        }
        protected T GetProvider<T>() where T : TextProviderBase, new()
        {
            string providerCode = typeof(T).Name;
            if (providerCode == "TextProvider")
                providerCode = typeof(T).Namespace.Substring(typeof(T).Namespace.LastIndexOf(".") + 1);
            else if (providerCode.EndsWith("TextProvider"))
                providerCode = providerCode.Substring(0, providerCode.Length - "TextProvider".Length);
            return GetProvider<T>(providerCode);
        }

        protected internal override string GetTextByPath(IEnumerable<string> textPath, params object[] parameters)
        {
            TextProviderBase provider;
            int skip = 1;
            var providerCode = textPath.First();
            var matchingMembers = this.GetType().GetMember(providerCode, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Where(mem => !(mem is MethodInfo) || !(((MethodInfo)mem).IsGenericMethodDefinition));
            if (matchingMembers.Count() == 0)
                throw new ArgumentException(string.Concat("Invalid text path: Provider '", providerCode, "' not found", "textPath"));
            else if (matchingMembers.Count() > 1)
                throw new ArgumentException(string.Concat("Invalid text path: Multiple providers found for '", providerCode, "'"));
            var memberToUse = matchingMembers.First();
            if (memberToUse is FieldInfo)
                provider = (TextProviderBase)((FieldInfo)memberToUse).GetValue(this);
            else if (memberToUse is PropertyInfo)
                provider = (TextProviderBase)((PropertyInfo)memberToUse).GetValue(this);
            else if (memberToUse is MethodInfo)
            {
                if (typeof(EntityTextProvider).IsAssignableFrom(((MethodInfo)memberToUse).ReturnType))
                {
                    provider = (TextProviderBase)((MethodInfo)memberToUse).Invoke(this, new object[] { textPath.ElementAt(1) });
                    skip++;
                }
                else
                    provider = (TextProviderBase)((MethodInfo)memberToUse).Invoke(this, new object[] { });
            }
            else
                throw new ArgumentException(string.Concat("Invalid text path: Member '" + memberToUse.Name + "' is not of the correct type to get the provider for '", providerCode, "'"));
            return provider.GetTextByPath(textPath.Skip(skip), parameters);
        }

        protected internal override void RecursiveTouchAllTexts()
        {
            foreach (PropertyInfo providerProperty in this.GetType().GetProperties())
            {
                if (typeof(TextProviderBase).IsAssignableFrom(providerProperty.PropertyType))
                {
                    TextProviderBase provider = (TextProviderBase)providerProperty.GetValue(this);
                    provider.RecursiveTouchAllTexts();
                }
            }
        }
    }

    public abstract partial class RootTextProvider : GroupTextProvider
    {

        public static TextPlaceHolder GetPlaceHolderForTranslatable<Tentity>(Tentity entity, Expression<Func<Tentity, string>> property)
        {
            //TODO: Real
            return new TextPlaceHolder(property.Compile().Invoke(entity));
        }

        public string GetTextForTranslatable<Tentity>(Tentity entity, Expression<Func<Tentity, string>> property)
        {
            //TODO: Real
            return property.Compile().Invoke(entity);
        }

        public string ForDisplay(object value)
        {
            return ForDisplay(value, null);
        }

        public string ForDisplay(object value, string format)
        {
            if (value == null)
                return "";
            else
            {
                var valueToUse = ConvertParameterForDisplay(value);
                if (format == null) return valueToUse.ToString();
                else if (valueToUse is sbyte) return ((sbyte)valueToUse).ToString(format);
                else if (valueToUse is short) return ((short)valueToUse).ToString(format);
                else if (valueToUse is int) return ((int)valueToUse).ToString(format);
                else if (valueToUse is long) return ((long)valueToUse).ToString(format);
                else if (valueToUse is byte) return ((byte)valueToUse).ToString(format);
                else if (valueToUse is ushort) return ((ushort)valueToUse).ToString(format);
                else if (valueToUse is uint) return ((uint)valueToUse).ToString(format);
                else if (valueToUse is ulong) return ((ulong)valueToUse).ToString(format);
                else if (valueToUse is float) return ((float)valueToUse).ToString(format);
                else if (valueToUse is double) return ((double)valueToUse).ToString(format);
                else if (valueToUse is bool)
                {
                    //Format is TrueText:FalseText, possibly with \ to escape
                    string[] trueFalse;
                    if (format.Contains(@"\"))
                    {
                        //Complex version, has escapes
                        string colonTempvalueToUse = Guid.NewGuid().ToString();
                        string backSlashTempvalueToUse = Guid.NewGuid().ToString();
                        trueFalse = format
                            .Replace(@"\\", backSlashTempvalueToUse)
                            .Replace(@"\:", colonTempvalueToUse)
                            .Split(':');
                        for (int i = 0; i < trueFalse.Length; i++)
                            trueFalse[i] = trueFalse[i]
                                .Replace(backSlashTempvalueToUse, @"\")
                                .Replace(colonTempvalueToUse, @":");
                    }
                    else
                        trueFalse = format.Split(':');
                    if ((bool)valueToUse)
                        return trueFalse[0];
                    else if (trueFalse.Length > 1)
                        return trueFalse[1];
                    else //No "false" valueToUse in format, use default
                        return false.ToString();
                }
                else if (valueToUse is decimal) return ((decimal)valueToUse).ToString(format);
                else if (valueToUse is DateTime)
                {
                    string checkForTime = new DateTime(1900, 1, 1, 2, 2, 3).ToString(format); //Will hold a "2" only if the format includes time
                    if (format != null && !format.Contains("2") && !format.Contains("2") && ((DateTime)valueToUse).TimeOfDay != TimeSpan.Zero)
                        //Format does not have a time, but there is a time in the value
                        format += " HH:mm" + (checkForTime.Contains("3") ? ":ss" : ""); //Show the time anyway to avoid issues with timezones
                    return ((DateTime)valueToUse).ToString(format);
                }
                else return valueToUse.ToString();
            }
        }

        private static IEnumerable<string> GetEmptyPath()
        {
            yield break;
        }

        public RootTextProvider()
            : base(null, GetEmptyPath())
        {
            this.Root = this;
        }

        public virtual EntityTextProvider Entity(string entityName) { return Entity_Internal(this.GetType().Namespace, entityName); }
        public virtual EntityTextProvider<T> Entity<T>() { return GetEntity<T>(); }

        public Dictionary<IEnumerable<string>, string> GetEmptyStoreDictionary()
        {
            return new Dictionary<IEnumerable<string>, string>(new EnumerableComparer<string>());
        }

        public Dictionary<IEnumerable<string>, string> GetAllDefaults()
        {
            //Note we use a temporary object to not mess up the current instance
            RootTextProvider temporaryTextProvider = (RootTextProvider)this.GetType().GetConstructor(System.Type.EmptyTypes).Invoke(new object[] { });
            temporaryTextProvider._gettingDefaults = true;
            temporaryTextProvider.StoreTexts = GetEmptyStoreDictionary();
            temporaryTextProvider.RecursiveTouchAllTexts();
            //RecursiveTouchAllTexts will use the text getter for all texts
            //These all eventually call GetTextFromStore which - because _gettingDefaults is true -
            //will save the textpath and default text to the StoreTexts dictionary.
            return temporaryTextProvider.StoreTexts;
        }

        public IFormatProvider FormatProvider { get; set; }
        public TimeZoneInfo TimeZone { get; set; }

        static Regex _findComplexPlaceHolders = new Regex(@"\{[0-9]\.[A-Za-z0-9_]*[\,\:\}]");
        protected internal virtual string FinishText(string text, params object[] parameters)
        {
            if (text == null)
                return null;
            else if (parameters == null || parameters.Length == 0 || !text.Contains("{"))
                return text;
            else
            {
#if !DEBUG
                try //Text entered by users might have wrong place holders
                {
#endif
                if (!(parameters[0] is string) && parameters[0] is IEnumerable && text.Contains("{0") && text.LastIndexOf("}") > text.IndexOf("{0"))
                {
                    //Format e.g.: "List: {0}, {0} and {0} no more"
                    //Or           "List: {0.A}xx{0.B}, {0.A}xx{0.B} and {0.A}xx{0.B} no more"
                    //Actual format text must repeat.

                    string firstParamUsage = text.Substring(text.IndexOf("{0"), text.IndexOf("}", text.IndexOf("{0") + 1) - text.IndexOf("{0") + 1);
                    string lastParamUsage = text.Substring(text.LastIndexOf("{0"), text.LastIndexOf("}") - text.LastIndexOf("{0") + 1);
                    string repeatedPart = text.Substring(text.IndexOf(firstParamUsage), text.IndexOf(lastParamUsage) - text.IndexOf(firstParamUsage) + lastParamUsage.Length);
                    string[] parts = text.Split(new string[] { repeatedPart }, StringSplitOptions.None);
                    string beforeAll = parts[0];
                    string inTheMiddle;
                    if (parts.Length > 2)
                        inTheMiddle = parts[1];
                    else
                        inTheMiddle = "";
                    string beforeTheLast;
                    if (parts.Length > 3)
                        beforeTheLast = parts[2];
                    else
                        beforeTheLast = inTheMiddle;
                    string afterAll = parts[parts.Length - 1];
                    List<string> texts = new List<string>();
                    foreach (object obj in (IEnumerable)parameters[0])
                        texts.Add(this.FinishText(repeatedPart, new object[] { obj }.Union(parameters.Skip(1)).ToArray()));
                    object[] otherParams = new object[] { null }.Union(parameters.Skip(1)).ToArray();
                    StringBuilder result = new StringBuilder();
                    result.Append(this.FinishText(beforeAll, otherParams)); //"List: "
                    for (int i = 0; i < texts.Count; i++)
                    {
                        if (i == 0) //First part
                        {
                            //Do nothing
                        }
                        else if (i < texts.Count - 1) //Not the first, not the last
                            result.Append(this.FinishText(inTheMiddle, otherParams)); //", "
                        else if (i == texts.Count - 1) //Last part
                            result.Append(this.FinishText(beforeTheLast, otherParams)); //" and "
                        result.Append(texts[i]);
                    }
                    result.Append(afterAll); //" no more"
                    return result.ToString();
                }
                else
                {
                    var matches = _findComplexPlaceHolders.Matches(text).Cast<Match>().Select(m => m.Value).ToList();
                    if (matches.Count == 0)
                        return string.Format(this.FormatProvider ?? CultureInfo.CreateSpecificCulture("en-US"), text, parameters.Select(param => ConvertParameterForDisplay(param)).ToArray());
                    else
                    {
                        List<object> parametersToUse = new List<object>(parameters);
                        foreach (var property in matches.GroupBy(m => m.Substring(1, m.Length - 2))) //Remove start and finish to get unique prop name ({0.Item1:yyyy-MM-dd} is in the same group as {0.Item1}
                        {
                            var propParts = property.Key.Split('.');
                            object source = parameters[int.Parse(propParts[0])];
                            object valueToUse;
                            if (source == null)
                                valueToUse = null;
                            else
                                valueToUse = source.GetType().GetProperty(propParts[1]).GetValue(source);
                            parametersToUse.Add(valueToUse);
                            int parameterIndex = parametersToUse.Count - 1;
                            foreach (var match in property.Distinct())
                                text = text.Replace(match, match.Replace(property.Key, parameterIndex.ToString()));
                        }
                        return string.Format(this.FormatProvider ?? CultureInfo.CreateSpecificCulture("en-US"), text, parametersToUse.Select(param => ConvertParameterForDisplay(param)).ToArray());
                    }
                }
#if !DEBUG
                }
                catch
                {
                    //Just in case the text uses incorrect place holders
                    return text;
                }
#endif
            }
        }

        protected virtual object ConvertParameterForDisplay(object value)
        {
            if (value == null)
                return null;
            else if (value is TextPlaceHolder)
                return this.GetTextForPlaceHolder((TextPlaceHolder)value);
            else if (value is DateTime && ((DateTime)value).Kind == DateTimeKind.Utc && this.TimeZone != null)
            {
                if (this.TimeZone != null)
                    return TimeZoneInfo.ConvertTimeFromUtc((DateTime)value, this.TimeZone);
            }
            return value;
        }

        private IEnumerable<Type> GetEntities()
        {
            return this.GetType().Assembly.GetTypes().Where(type => EntityAttribute.IsEntity(type));
        }

        protected internal override void RecursiveTouchAllTexts()
        {
            base.RecursiveTouchAllTexts();
            var entityMethod = this.GetType().GetMethod("Entity", new Type[] { });
            foreach (Type entityType in GetEntities())
            {
                TextProviderBase entityProvider = (TextProviderBase)entityMethod.MakeGenericMethod(new Type[] { entityType }).Invoke(this, new object[] { });
                entityProvider.RecursiveTouchAllTexts();
            }
        }

        public virtual string GetTextForPlaceHolder(TextPlaceHolder placeHolder)
        {
            if (placeHolder.TextPath == null)
                return placeHolder.FixedText;
            else
                return GetTextByPath(placeHolder.TextPath, placeHolder.Parameters);
        }

        bool _gettingDefaults = false;

        Dictionary<IEnumerable<string>, string> _storeTexts;
        public Dictionary<IEnumerable<string>, string> StoreTexts
        {
            get { return _storeTexts; }
            set
            {
                if (value != null && !(value.Comparer is EnumerableComparer<string>))
                    throw new ArgumentException("The comparer for the store dictionary is incorrect. Please use the GetEmptyStoreDictionary method to get a correct dictionary");
                _storeTexts = value;
            }
        }
        protected internal string GetTextFromStore(IEnumerable<string> path, string defaultText, params object[] parameters)
        {
            if (_gettingDefaults)
            {
                this.StoreTexts[path] = defaultText;
                return null;
            }
            else
            {
                string result;
                if (this.StoreTexts != null && this.StoreTexts.TryGetValue(path, out result) && result != null)
                    return this.FinishText(result, parameters);
                else
                    return this.FinishText(defaultText, parameters);
            }
        }

        private class EnumerableComparer<T> : IEqualityComparer<IEnumerable<T>>
        {
            public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
            {
                using (var enumeratorY = y.GetEnumerator())
                {
                    foreach (var tX in x)
                    {
                        if (!enumeratorY.MoveNext())
                            return false;
                        if (!object.Equals(tX, enumeratorY.Current))
                            return false;
                    }
                    if (enumeratorY.MoveNext())
                        return false;
                }
                return true;
            }

            public int GetHashCode(IEnumerable<T> obj)
            {
                int result = 0;
                foreach (T t in obj)
                    if (t != null)
                        result ^= t.GetHashCode();
                return result;
            }
        }
    }

    public abstract class RootTextProvider<Tthis> : RootTextProvider where Tthis : RootTextProvider, new()
    {
        protected RootTextProvider() { }

        public static TextPlaceHolder GetPlaceHolder(Expression<Func<Tthis, string>> textGetter)
        {
            List<string> textPath = new List<string>();
            object[] parameters = null;

            Expression exp = textGetter.Body;
            if (exp is MethodCallExpression)
                parameters = ((MethodCallExpression)exp).Arguments.Select(arg => Expression.Lambda(arg).Compile().DynamicInvoke()).ToArray();
            while (exp != null)
            {
                if (exp is MethodCallExpression)
                {
                    var methodCall = ((MethodCallExpression)exp);
                    if (typeof(ModelTextProvider).IsAssignableFrom(methodCall.Method.ReturnType))
                    {
                        if (methodCall.Arguments.Count > 0)
                        {
                            Expression param = methodCall.Arguments.First();
                            if (param is UnaryExpression)
                                param = ((UnaryExpression)param).Operand;
                            if (param is LambdaExpression)
                                param = ((LambdaExpression)param).Body;
                            if (param is MemberExpression)
                                textPath.Add(((MemberExpression)param).Member.Name);
                            else if (param is ConstantExpression)
                                textPath.Add(((ConstantExpression)param).Value.ToString());
                        }
                        else if (methodCall.Method.GetGenericArguments().Count() > 0)
                            textPath.Add(methodCall.Method.GetGenericArguments()[0].Name);
                    }
                    textPath.Add(methodCall.Method.Name);
                    exp = methodCall.Object;
                }
                else if (exp is MemberExpression)
                {
                    textPath.Add(((MemberExpression)exp).Member.Name);
                    exp = ((MemberExpression)exp).Expression;
                }
                else
                    exp = null;
            }

            textPath.Reverse();

            var result = new TextPlaceHolder(textPath.ToArray(), parameters);
            //There should be no need to get the text here, it will be gotten when the place holder is displayed
            //result.Text = textGetter.Compile().Invoke(new Tthis());

            return result;
        }
    }

    public class ModelTextProvider : TextProviderBase
    {
        protected ModelTextProvider(RootTextProvider root, IEnumerable<string> path) : base(root, path) { }

        protected internal ModelTextProvider(RootTextProvider root, IEnumerable<string> path, DisplayAttribute displayAttribute, string codeName) :
            this(root, path)
        {
            if (displayAttribute != null)
            {
                _defaultName = displayAttribute.GetName();
                _defaultShortName = displayAttribute.GetShortName();
                _defaultDescription = displayAttribute.GetDescription();
            }
            if (string.IsNullOrEmpty(_defaultName)) _defaultName = PascalToDisplayText(codeName, false);
            if (string.IsNullOrEmpty(_defaultShortName))
            {
                if (displayAttribute != null && !string.IsNullOrEmpty(displayAttribute.GetName())) //Overrules default
                    _defaultShortName = _defaultName;
                else
                    _defaultShortName = PascalToDisplayText(codeName, true);
            }
            if (_defaultDescription == "") _defaultDescription = null;
        }

        protected string _defaultName;
        public string Name { get { return this.Root.GetTextFromStore(AddItemToPathEnd(this.Path, "Name"), _defaultName); } }
        protected string _defaultShortName;
        public string ShortName { get { return this.Root.GetTextFromStore(AddItemToPathEnd(this.Path, "ShortName"), _defaultShortName); } }
        protected string _defaultDescription;
        public string Description { get { return this.Root.GetTextFromStore(AddItemToPathEnd(this.Path, "Description"), _defaultDescription); } }

        protected internal override void RecursiveTouchAllTexts()
        {
            //Note we don't need the value of the properties, we just need their getter
            //to be used
            if (this.Name == null) { }
            if (this.ShortName == null) { }
            if (this.Description == null) { }
        }

        protected internal override string GetTextByPath(IEnumerable<string> textPath, params object[] parameters)
        {
            if (textPath.Count() > 1)
                throw new ArgumentException("Invalid textPath", "textPath");
            else
            {
                var propertyName = textPath.First();
                if (propertyName == "Name") return this.Name;
                else if (propertyName == "ShortName") return this.ShortName;
                else if (propertyName == "Description") return this.Description;
                else throw new ArgumentException("Invalid textPath", "textPath");
            }
        }
    }

    public abstract class EntityTextProvider : ModelTextProvider
    {
        internal Type _entityType;
        ConcurrentDictionary<string, ModelTextProvider> _properties = new ConcurrentDictionary<string, ModelTextProvider>();

        public EntityTextProvider(RootTextProvider root, IEnumerable<string> path, Type entityType)
            : base(root, path)
        {
            _entityType = entityType;
            EntityAttribute entityAttr = _entityType.GetCustomAttributes<EntityAttribute>(false).LastOrDefault();
            if (entityAttr == null)
                throw new InvalidOperationException(_entityType.Name + " is not an entity (does not have the Entity attribute)");
            _defaultName = entityAttr.DisplayName;
            _defaultShortName = entityAttr.ShortName;
            _defaultDescription = entityAttr.Description;
            _defaultSetName = entityAttr.SetDisplayName;
            if (string.IsNullOrEmpty(_defaultName)) _defaultName = PascalToDisplayText(_entityType.Name, false);
            if (string.IsNullOrEmpty(_defaultShortName))
            {
                if (entityAttr.DisplayName != null) //Overrules default
                    _defaultShortName = _defaultName;
                else
                    _defaultShortName = PascalToDisplayText(_entityType.Name, true);
            }
            if (_defaultDescription == "") _defaultDescription = null;
            if (string.IsNullOrEmpty(_defaultSetName))
                _defaultSetName = TextProviderBase.MakePlural(_defaultName);
        }

        protected string _defaultSetName;
        public string SetName { get { return this.Root.GetTextFromStore(AddItemToPathEnd(this.Path, "SetName"), _defaultSetName); } }

        public ModelTextProvider Property(string propertyName)
        {
            return _properties.GetOrAdd(propertyName, name =>
            {
                var realMember = _entityType.GetMember(name, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();
                if (realMember == null)
                    throw new ArgumentException("Property '" + propertyName + "' is not a property of " + _entityType.Name, "propertyName");
                return new ModelTextProvider(
                    this.Root,
                    AddItemToPathEnd(AddItemToPathEnd(this.Path, "Property"), propertyName),
                    realMember.GetCustomAttributes<DisplayAttribute>(true).LastOrDefault(),
                    name);
            });
        }

        protected internal override void RecursiveTouchAllTexts()
        {
            base.RecursiveTouchAllTexts();
            //Note we don't need the value of the properties, we just need their getter
            //to be used
            if (this.SetName == null) { }
            foreach (PropertyInfo prop in _entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                TextProviderBase prov = (TextProviderBase)this.Property(prop.Name);
                prov.RecursiveTouchAllTexts();
            }
        }

        protected internal override string GetTextByPath(IEnumerable<string> textPath, params object[] parameters)
        {
            if (textPath.Count() == 1)
            {
                var text = textPath.First();
                if (text == "SetName") return this.SetName;
                else return base.GetTextByPath(textPath, parameters);
            }
            else if (textPath.First() == "Property")
                return this.Property(textPath.ElementAt(1)).GetTextByPath(textPath.Skip(2));
            else
                throw new ArgumentException("Invalid textPath", "textPath");
        }
    }

    public class EntityTextProvider<Tentity> : EntityTextProvider
    {
        public EntityTextProvider(RootTextProvider root, IEnumerable<string> path)
            : base(root, path, typeof(Tentity))
        {
        }
        public ModelTextProvider Property<T>(Expression<Func<Tentity, T>> property)
        {
            MemberExpression memberExpression = property.Body as MemberExpression;
            if (memberExpression != null)
                return Property(memberExpression.Member.Name);
            else
                throw new ArgumentException("property must the getter for a specific property of " + typeof(Tentity).Name);
        }
    }

    public abstract class MessageTextProvider : TextProviderBase
    {
        public MessageTextProvider() : base(null, null) { } //Assume root and path are set afterwards

        protected MessageTextProvider(RootTextProvider root, IEnumerable<string> path) : base(root, path) { }

        protected string GetDefaultText([CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            return GetText(PascalToDisplayText(code, false), default(object[]), code);
        }

        protected string GetText(string defaultText, [CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            return GetText(defaultText, default(object[]), code);
        }

        protected string GetText(string defaultText, object[] parameters, [CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            return this.Root.GetTextFromStore(AddItemToPathEnd(this.Path, code), defaultText, parameters);
        }

        protected string GetDefaultTextForEnum<T>(T value, [CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            return GetTextForEnum<T>(value, PascalToDisplayText(value.ToString(), false), default(object[]), code);
        }

        protected string GetDefaultTextForEnum<T>(T value, object[] remainingParameters, [CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            return GetTextForEnum<T>(value, PascalToDisplayText(value.ToString(), false), remainingParameters, code);
        }

        protected string GetTextForEnum<T>(T value, string defaultText, [CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            return GetTextForEnum<T>(value, defaultText, default(object[]), code);
        }

        protected string GetTextForEnum<T>(T value, string defaultText, object[] remainingParameters, [CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            return this.Root.GetTextFromStore(AddItemToPathEnd(AddItemToPathEnd(this.Path, code), value.ToString()), defaultText, remainingParameters);
        }

        static Regex _findMultiOptionText = new Regex(@"\[(?<one>[^]]*)\]\[(?<multi>[^]]*)\]"); //There [is][are] ...
        static Regex _findBracketedText = new Regex(@"\[[^]]*\]"); //{0} rate[s]
        private string GetOneText(string multipleTextWithBracketsAroundMultipleParts)
        {
            return
                _findBracketedText.Replace(
                    _findMultiOptionText.Replace(multipleTextWithBracketsAroundMultipleParts, m => m.Groups["one"].Value) //use "one" text.
                , m => "");
        }
        private string GetMultipleText(string multipleTextWithBracketsAroundMultipleParts)
        {
            return
                _findBracketedText.Replace(
                    _findMultiOptionText.Replace(multipleTextWithBracketsAroundMultipleParts, m => m.Groups["multi"].Value)
                , m => m.Value.Substring("[".Length, m.Value.Length - "[]".Length));
        }

        //The overloads without "oneText" will use the defaultText as oneText with logic using [ and ]:
        //"There [is][are]" will become "There is" or "There are" (note the [ for "are" follows the ] for "is" immediately
        //"{0} rate[s]" will become "1 rate" or "2 rates"
        //This logic only applies to the hardcoded versions, separate translations can be given for one and multiple items so users can do what they like

        protected string GetTextForList<T>(string defaultText, T value, [CallerMemberName]string code = "<default is set to member name by the compiler>")
            where T : IEnumerable
        {
            return GetTextForList<T>(GetMultipleText(defaultText), GetOneText(defaultText), default(string), value, default(object[]), code);
        }

        protected string GetTextForList<T>(string defaultText, T value, object[] remainingParameters, [CallerMemberName]string code = "<default is set to member name by the compiler>")
            where T : IEnumerable
        {
            return GetTextForList<T>(GetMultipleText(defaultText), GetOneText(defaultText), default(string), value, remainingParameters, code);
        }

        protected string GetTextForList<T>(string defaultText, string emptyText, T value, [CallerMemberName]string code = "<default is set to member name by the compiler>")
            where T : IEnumerable
        {
            return GetTextForList<T>(GetMultipleText(defaultText), GetOneText(defaultText), emptyText, value, default(object[]), code);
        }

        protected string GetTextForList<T>(string defaultText, string emptyText, T value, object[] remainingParameters, [CallerMemberName]string code = "<default is set to member name by the compiler>")
            where T : IEnumerable
        {
            return GetTextForList<T>(GetMultipleText(defaultText), GetOneText(defaultText), emptyText, value, remainingParameters, code);
        }

        protected string GetTextForList<T>(string multipleText, string oneText, string emptyText, T value, [CallerMemberName]string code = "<default is set to member name by the compiler>")
            where T : IEnumerable
        {
            return GetTextForList(GetMultipleText(multipleText), GetOneText(oneText), emptyText, value, default(object[]), code);
        }

        protected string GetTextForList<T>(string multipleText, string oneText, string emptyText, T value, object[] remainingParameters, [CallerMemberName]string code = "<default is set to member name by the compiler>")
            where T : IEnumerable
        {
            if (typeof(T) == typeof(string))
                throw new ArgumentException("A string is not supported here. Please check the parameter list.", "value");
            int numberOfItems = value == null ? 0 : value.Cast<object>().Count();
            string extraCode;
            string defaultText;
            if (numberOfItems == 0 && emptyText != null)
            {
                extraCode = "Empty";
                defaultText = emptyText;
            }
            else if (numberOfItems == 1 && oneText != null)
            {
                extraCode = "Single";
                defaultText = oneText ?? multipleText;
            }
            else
            {
                extraCode = "Multiple";
                defaultText = multipleText;
            }
            return this.Root.GetTextFromStore(AddItemToPathEnd(AddItemToPathEnd(this.Path, code), extraCode), defaultText, (new object[] { value }).Union(remainingParameters ?? new object[] { }).ToArray());
        }


        protected string GetTextForCount(string defaultText, int count, [CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            return GetTextForCount(GetMultipleText(defaultText), GetOneText(defaultText), default(string), count, default(object[]), code);
        }

        protected string GetTextForCount(string defaultText, int count, object[] remainingParameters, [CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            return GetTextForCount(GetMultipleText(defaultText), GetOneText(defaultText), default(string), count, remainingParameters, code);
        }

        protected string GetTextForCount(string defaultText, string emptyText, int count, [CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            return GetTextForCount(GetMultipleText(defaultText), GetOneText(defaultText), emptyText, count, default(object[]), code);
        }

        protected string GetTextForCount(string defaultText, string emptyText, int count, object[] remainingParameters, [CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            return GetTextForCount(GetMultipleText(defaultText), GetOneText(defaultText), emptyText, count, remainingParameters, code);
        }

        protected string GetTextForCount<T>(string multipleText, string oneText, string emptyText, int count, [CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            return GetTextForCount(GetMultipleText(multipleText), GetOneText(oneText), emptyText, count, default(object[]), code);
        }

        protected string GetTextForCount(string multipleText, string oneText, string emptyText, int count, object[] remainingParameters, [CallerMemberName]string code = "<default is set to member name by the compiler>")
        {
            string extraCode;
            string defaultText;
            if (count == 0 && emptyText != null)
            {
                extraCode = "Empty";
                defaultText = emptyText;
            }
            else if (count == 1 && oneText != null)
            {
                extraCode = "Single";
                defaultText = oneText ?? multipleText;
            }
            else
            {
                extraCode = "Multiple";
                defaultText = multipleText;
            }
            return this.Root.GetTextFromStore(AddItemToPathEnd(AddItemToPathEnd(this.Path, code), extraCode), defaultText, (new object[] { count }).Union(remainingParameters ?? new object[] { }).ToArray());
        }


        protected internal override string GetTextByPath(IEnumerable<string> textPath, params object[] parameters)
        {
            string textCode = textPath.First();
            MemberInfo[] matchingMembers = this.GetType().GetMember(textCode, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (matchingMembers.Length == 0)
                throw new InvalidOperationException(string.Concat("Text '", textCode, "' not found"));
            else if (matchingMembers.Length > 1)
                throw new InvalidOperationException(string.Concat("Multiple texts found for '", textCode, "'"));
            if (matchingMembers[0] is FieldInfo)
                return (string)((FieldInfo)matchingMembers[0]).GetValue(this);
            else if (matchingMembers[0] is PropertyInfo)
                return (string)((PropertyInfo)matchingMembers[0]).GetValue(this);
            else if (matchingMembers[0] is MethodInfo)
            {
                object[] paramsToUse = parameters;
                MethodInfo method = (MethodInfo)matchingMembers[0];
                var methodParameters = method.GetParameters();
                if (textPath.Count() > 1 && method.GetParameters().Length > 0
                    && methodParameters.First().ParameterType.IsEnum)
                {
                    //If the first parameter is an enum, the value can be part of the text code.
                    paramsToUse =
                        new object[] { Enum.Parse(method.GetParameters().First().ParameterType, textPath.ElementAt(1)) }
                        .Union(parameters)
                        .ToArray();
                }
                else
                {
                    for (int i = 0; i < parameters.Length; i++)
                        paramsToUse[i] = ConvertParameter(paramsToUse[i], methodParameters[i].ParameterType);
                }
                return (string)method.Invoke(this, paramsToUse);
            }
            else
                throw new InvalidOperationException(string.Concat("Member '" + matchingMembers[0].Name + "' is not of the correct type to get text for '", textCode, "'"));
        }

        //Because the TextPlaceHolder might not have been (de)serialized as expected, we need to convert to what 
        //is needed
        private object ConvertParameter(object value, Type targetType)
        {
            //Note we can't just serialize + deserialize because tuples won't work.
            if (value == null)
                return null;
            else if (value.GetType() == targetType)
                return value;
            else if (typeof(IEnumerable).IsAssignableFrom(value.GetType())
                && targetType.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(targetType.GetGenericTypeDefinition()))
            {
                Type elementType = targetType.GetGenericArguments()[0];
                IList result = (IList)typeof(List<>).MakeGenericType(elementType).GetConstructor(new Type[] { }).Invoke(new object[] { });
                foreach (object element in ((IEnumerable)value))
                    result.Add(ConvertParameter(element, elementType));
                return result;
            }
            else if (value is Dictionary<string, object> && targetType.Namespace == "System" && targetType.Name.StartsWith("Tuple`"))
            {
                var constructor = targetType.GetConstructors().First();
                Type[] constructorParamTypes = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
                object[] parameters = new object[constructor.GetParameters().Length];
                int i = 0;
                foreach (var property in (Dictionary<string, object>)value)
                {
                    parameters[i] = ConvertParameter(property.Value, constructorParamTypes[i]);
                    i++;
                }
                return constructor.Invoke(parameters);
            }
            else if (value is Dictionary<string, object>)
            {
                //Property names and values
                object result = targetType.GetConstructor(new Type[] { }).Invoke(new object[] { });
                foreach (var property in (Dictionary<string, object>)value)
                {
                    var targetProperty = targetType.GetProperty(property.Key);
                    targetProperty.SetValue(result, ConvertParameter(property.Value, targetProperty.PropertyType));
                }
                return result;
            }
            else if (targetType == typeof(string[]) && !(value is string) && value is IEnumerable)
                return ((IEnumerable)value).Cast<Object>().Select(v => (v == null ? default(string) : v.ToString())).ToArray();
            else if (value.GetType() == Nullable.GetUnderlyingType(targetType))
                return value;
            else
                return ExpressionSerialization.ChangeType(value, targetType);
        }

        protected internal override void RecursiveTouchAllTexts()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (MethodInfo textMethod in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                if (textMethod.ReturnType == typeof(string))
                {
                    MethodInfo touchAllMethod = this.GetType().GetMethod($"{textMethod.Name}_TouchAllTexts", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    if (touchAllMethod != null)
                        touchAllMethod.Invoke(this, null);
                    else if (textMethod.GetParameters().Length > 0 && textMethod.GetParameters().First().ParameterType.IsEnum)
                        //If the first parameter is an enum, there is a separate translation per item
                        foreach (Enum enm in Enum.GetValues(textMethod.GetParameters().First().ParameterType))
                            textMethod.Invoke(this,
                                new object[] { enm }
                                .Union(
                                    textMethod.GetParameters()
                                    .Skip(1)
                                    .Select(param => param.ParameterType.IsValueType ? Activator.CreateInstance(param.ParameterType) : null)
                                )
                                .ToArray()
                            );
                    else if (textMethod.GetParameters().Length > 0
                        && textMethod.GetParameters().First().ParameterType != typeof(string)
                        && typeof(IEnumerable).IsAssignableFrom(textMethod.GetParameters().First().ParameterType)
                        && textMethod.GetParameters().First().ParameterType.IsGenericType)
                    {
                        //First parameter is an IEnumerable, we might need multiple texts depending on there being 0, 1 or more items
                        Type elementType = textMethod.GetParameters().First().ParameterType.GetGenericArguments().First();
                        IList dummyList = (IList)typeof(List<>).MakeGenericType(elementType).GetConstructor(new Type[] { }).Invoke(new object[] { });
                        for (int number = 0; number <= 2; number++)
                        {
                            textMethod.Invoke(this,
                                new object[] { dummyList }
                                .Union(
                                    textMethod.GetParameters()
                                    .Skip(1)
                                    .Select(param => param.ParameterType.IsValueType ? Activator.CreateInstance(param.ParameterType) : null)
                                )
                                .ToArray()
                            );
                            dummyList.Add(null);
                        }
                    }
                    else if (textMethod.GetParameters().Length > 0
                            && textMethod.GetParameters().First().ParameterType == typeof(int))
                    {
                        //First parameter is an int, we might need multiple texts depending on it being 0, 1 or more
                        for (int number = 0; number <= 2; number++)
                        {
                            textMethod.Invoke(this,
                                new object[] { number }
                                .Union(
                                    textMethod.GetParameters()
                                    .Skip(1)
                                    .Select(param => param.ParameterType.IsValueType ? Activator.CreateInstance(param.ParameterType) : null)
                                )
                                .ToArray()
                            );
                        }
                    }
                    else
                        textMethod.Invoke(this,
                            textMethod.GetParameters()
                            .Select(param => param.ParameterType.IsValueType ? Activator.CreateInstance(param.ParameterType) : null)
                            .ToArray()
                        );
                }
            }
        }
    }
}
