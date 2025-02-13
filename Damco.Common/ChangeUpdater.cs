using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Damco.Common.Specification;
//using NORA.Model.Base;

namespace Damco.Common
{
    public class ChangeUpdater<T>
        where T : class, new()
    {
        Dictionary<string, Func<T, object>> _parentGetters = new Dictionary<string, Func<T, object>>();
        Dictionary<string, Action<T, object>> _parentSetters = new Dictionary<string, Action<T, object>>();
        Dictionary<string, Func<object, string>> _parentInfoGetters = new Dictionary<string, Func<object, string>>();
        public ChangeUpdater<T> AddParent<Tparent>(Expression<Func<T, Tparent>> parentGetter, ChangeUpdater<Tparent> parentChangeUpdater)
            where Tparent : class, new()
        {
            return AddParent(parentGetter, parentChangeUpdater?.DescriptionFactory);
        }
        public ChangeUpdater<T> AddParent<Tparent>(Expression<Func<T, Tparent>> parentGetter, Func<Tparent, string> parentInfoGetter)
                where Tparent : class, new()
        {
            string key = ((MemberExpression)parentGetter.Body).Member.Name;

            _parentGetters.Add(key, Expression.Lambda<Func<T, object>>(Expression.Convert(parentGetter.Body, typeof(object)), parentGetter.Parameters.First()).Compile());
            var parentParam = Expression.Parameter(typeof(object), "p");
            _parentSetters.Add(key, Expression.Lambda<Action<T, object>>(
                Expression.Assign(parentGetter.Body, Expression.Convert(parentParam, typeof(Tparent))),
                parentGetter.Parameters.First(), parentParam
            ).Compile());

            if (parentInfoGetter != null)
                _parentInfoGetters.Add(key, x => parentInfoGetter((Tparent)x));

            return this;
        }

        public Func<T, string> DescriptionFactory { get; set; }
        public ChangeUpdater<T> SetDescription(Func<T, string> entityDescriptionFactory)
        {
            this.DescriptionFactory = entityDescriptionFactory;
            return this;
        }

        List<Dictionary<Func<T, T, bool>, Func<T, object>>> _mandatoryMatchers = new List<Dictionary<Func<T, T, bool>, Func<T, object>>>();
        public ChangeUpdater<T> AddMandatoryMatches(params Func<T, object>[] matchObjectGetters)
        {
            return AddMandatoryMatches_Internal(false, (IEnumerable<Func<T, object>>)matchObjectGetters);
        }
        public ChangeUpdater<T> AddMandatoryMatches(IEnumerable<Func<T, object>> matchObjectGetters)
        {
            return AddMandatoryMatches_Internal(false, matchObjectGetters);
        }
        public ChangeUpdater<T> AddMandatoryMatchesWithNulls(params Func<T, object>[] matchObjectGetters)
        {
            return AddMandatoryMatches_Internal(true, (IEnumerable<Func<T, object>>)matchObjectGetters);
        }
        public ChangeUpdater<T> AddMandatoryMatchesWithNulls(IEnumerable<Func<T, object>> matchObjectGetters)
        {
            return AddMandatoryMatches_Internal(true, matchObjectGetters);
        }
        private ChangeUpdater<T> AddMandatoryMatches_Internal(bool includeNulls, IEnumerable<Func<T, object>> matchObjectGetters)
        {
            var matchers = new Dictionary<Func<T, T, bool>, Func<T, object>>();
            foreach (var getter in matchObjectGetters)
            {
                if (includeNulls)
                    matchers.Add((a, b) => object.Equals(getter(a), getter(b)), getter);
                else
                    matchers.Add((a, b) => !object.Equals(getter(a), null) && object.Equals(getter(a), getter(b)), getter);
            }
            _mandatoryMatchers.Add(matchers);
            return this;
        }

        public ChangeUpdater<T> AddMandatoryMatcher(Func<T, T, bool> matcher, Func<T, object> infoGetter)
        {
            _mandatoryMatchers.Add(new Dictionary<Func<T, T, bool>, Func<T, object>>()
            {
                [matcher] = infoGetter
            });
            return this;
        }

        Dictionary<Func<T, T, bool>, Func<T, object>> _optionalMatchers = new Dictionary<Func<T, T, bool>, Func<T, object>>();
        public ChangeUpdater<T> AddOptionalMatches(params Func<T, object>[] matchObjectGetters)
        {
            foreach (var getter in matchObjectGetters)
                AddOptionalMatcher((a, b) => object.Equals(getter(a), getter(b)), getter);
            return this;
        }
        public ChangeUpdater<T> AddOptionalMatcher(Func<T, T, bool> matcher, Func<T, object> infoGetter)
        {
            if (!this.FuzzyMatching)
                throw new InvalidOperationException("Fuzzy matching is disabled, so optional matchers are not allowed");
            _optionalMatchers.Add(matcher, infoGetter);
            return this;
        }

        Action<T> _addAction;
        public ChangeUpdater<T> SetAddAction(Action<T> action)
        {
            _addAction = action;
            return this;
        }
        Action<T> _deleteAction;
        public ChangeUpdater<T> SetDeleteAction(Action<T> action)
        {
            _deleteAction = action;
            return this;
        }

        List<string> _nonUpdatedProperties = new List<string>();
        public ChangeUpdater<T> AddNonUpdatedProperties(params Expression<Func<T, object>>[] properties)
        {
            return this.AddNonUpdatedProperties(properties.Select(p => p.GetPropertyInfo().Name).ToArray());
        }
        public ChangeUpdater<T> AddNonUpdatedProperties(params string[] properties)
        {
            _nonUpdatedProperties.AddRange(properties);
            return this;
        }

        Dictionary<object, List<string>> _allUpdatedProperties;
        public ChangeUpdater<T> SetAllUpdatedProperties(Dictionary<object, List<string>> allPropertiesPerObject)
        {
            _allUpdatedProperties = allPropertiesPerObject;
            return this;
        }

        Dictionary<object, List<string>> _allNonUpdatedProperties;
        public ChangeUpdater<T> SetAllNonUpdatedProperties(Dictionary<object, List<string>> allNonPropertiesPerObject)
        {
            _allNonUpdatedProperties = allNonPropertiesPerObject;
            return this;
        }

        List<string> _updatedProperties = null;
        public ChangeUpdater<T> AddUpdatedProperties(params Expression<Func<T, object>>[] properties)
        {
            return this.AddUpdatedProperties(properties.Select(p => p.GetPropertyInfo().Name).ToArray());
        }
        public ChangeUpdater<T> AddUpdatedProperties(params string[] properties)
        {
            return this.AddUpdatedProperties((IEnumerable<string>)properties);
        }
        public ChangeUpdater<T> AddUpdatedProperties(IEnumerable<string> properties)
        {
            if (_updatedProperties == null) _updatedProperties = new List<string>();
            _updatedProperties.AddRange(properties);
            return this;
        }

        public bool FuzzyMatching { get; set; } = false;
        public bool RequireExplicitPropertyInclusion { get; set; }
        public bool RequireExplicitInclusionInParent { get; set; }
        public ChangeUpdater<T> SetInclusionRequirements(bool properties, bool childInParent)
        {
            this.RequireExplicitPropertyInclusion = properties;
            this.RequireExplicitInclusionInParent = childInParent;
            return this;
        }

        public ChangeUpdater<T> DoFuzzyMatching(bool fuzzyMatching = true)
        {
            this.FuzzyMatching = fuzzyMatching;
            return this;
        }

        public IEnumerable<SourceAndTargetMatch<T>> FindMatches(IEnumerable<T> source, IEnumerable<T> target, IEnumerable<T> additionalTargets = null, IEnumerable<SourceAndTargetMatch> parents = null, bool allowMultipleTargets = false, bool allowMultipleSources = false, ISpecification<T> specificationForMatches = null)
        {
            List<SourceAndTargetMatch<T>> result = new List<SourceAndTargetMatch<T>>();
            var sourceItems = new List<T>(source); //Prevent issues if caller changes them
            var targetItems = new List<T>(target); //Prevent issues if caller changes them
            var touchedItems = new List<T>();

            if (_includeSourceWhen != null)
            {
                foreach (var excluded in sourceItems.Where(x => !_includeSourceWhen(x)).ToList())
                {
                    result.Add(new Common.SourceAndTargetMatch<T>(excluded, null, MatchType.ExcludedSource, null));
                    sourceItems.Remove(excluded);
                }
            }
            if (_includeTargetWhen != null)
            {
                foreach (var excluded in targetItems.Where(x => !_includeTargetWhen(x)).ToList())
                {
                    result.Add(new Common.SourceAndTargetMatch<T>(null, excluded, MatchType.ExcludedTarget, null));
                    targetItems.Remove(excluded);
                }
            }

            //Get parents
            var allParentMatches = new Dictionary<T, Dictionary<string, SourceAndTargetMatch>>();
            foreach (var sourceItem in sourceItems)
            {
                var matchesThisItem = new Dictionary<string, SourceAndTargetMatch>();
                foreach (var parentGetter in _parentGetters)
                {
                    if (parents == null)
                        throw new InvalidOperationException($"{nameof(parents)} is required if a parent is set up");
                    var sourceParent = parentGetter.Value(sourceItem);
                    if (sourceParent != null)
                    {
                        var parentMatch = parents.SingleOrDefault(p => p != null && p.MatchType != MatchType.ExcludedSource && p.MatchType != MatchType.ExcludedTarget && p.Source == sourceParent);
                        matchesThisItem.Add(parentGetter.Key, parentMatch);
                    }
                    else
                        matchesThisItem.Add(parentGetter.Key, null);
                }
                allParentMatches.Add(sourceItem, matchesThisItem);
            }

            foreach (var mandatoryMatchers in _mandatoryMatchers.DefaultIfEmpty(null)) //Each matcher is an opportunity to match
            {
                var unmatched = new List<T>();
                foreach (var sourceItem in sourceItems)
                {
                    List<T> possibleMatches = targetItems.ToList();
                    if (!allowMultipleSources)
                        possibleMatches = possibleMatches.Except(touchedItems).ToList();

                    //Mandatory matcher
                    bool additionalTarget = false;
                    if (mandatoryMatchers != null && mandatoryMatchers.Any())
                        possibleMatches = possibleMatches.Where(l => mandatoryMatchers.All(m => m.Key(sourceItem, l))).ToList();
                    if (!possibleMatches.Any() && additionalTargets != null)
                    {
                        possibleMatches = additionalTargets.Where(l => mandatoryMatchers.All(m => m.Key(sourceItem, l))).ToList();
                        additionalTarget = true;
                    }

                    //Parent matchers
                    var parentMatches = allParentMatches[sourceItem];
                    foreach (var parentMatch in parentMatches)
                        if (parentMatch.Value != null)
                            possibleMatches = possibleMatches.Where(l => parentMatch.Value?.Target == _parentGetters[parentMatch.Key](l)).ToList();
                        else
                            possibleMatches = possibleMatches.Where(l => _parentGetters[parentMatch.Key](l) == null).ToList();

                    List<T> matches = new List<T>();
                    if(specificationForMatches != null)
                        possibleMatches = possibleMatches.Where(specificationForMatches.IsSatisfiedByCompile()).ToList();

                    if (possibleMatches.Count() == 1)
                    {
                        matches.Add(possibleMatches.Single());
                    }
                    else if (possibleMatches.Any()) //>= 2
                    {
                        var fuzzyMatching = this.FuzzyMatching;
                        if (!fuzzyMatching && !allowMultipleTargets)
                        {
                            var error = GetMultipleMatchesError(mandatoryMatchers, sourceItem, parentMatches);
                            if (error != null)
                                throw error;
                            else
                                fuzzyMatching = true;
                        }
                        var bestMatches = possibleMatches;
                        if (fuzzyMatching)
                        {
                            foreach (var matcher in _optionalMatchers)
                            {
                                var matchesThisGetter = bestMatches.Where(l => matcher.Key(sourceItem, l)).ToList();
                                if (matchesThisGetter.Any())
                                {
                                    bestMatches = matchesThisGetter;
                                    if (bestMatches.Count() == 1)
                                        break;
                                }
                            }
                        }

                        if (allowMultipleTargets)
                            matches.AddRange(bestMatches);
                        else
                            matches.Add(bestMatches.First());
                    }

                    if (matches.Count == 0) //Not found
                        unmatched.Add(sourceItem);
                    else
                    {
                        touchedItems.AddRange(matches);
                        foreach (var match in matches)
                            result.Add(new SourceAndTargetMatch<T>(sourceItem, match, additionalTarget ? MatchType.InSourceAndAdditionalTarget : MatchType.InSourceAndTarget, allParentMatches[sourceItem]));
                    }
                }
                sourceItems = unmatched;
            }

            //Left over sources (added)
            foreach (var sourceItem in sourceItems)
            {
                var sourceTargetMatch = new SourceAndTargetMatch<T>(sourceItem, new T(), MatchType.OnlyInSource, allParentMatches[sourceItem]);
                result.Add(sourceTargetMatch);
            }

            //Left over targets (deleted)
            foreach (var deleted in targetItems.Except(touchedItems))
            {
                var match = new SourceAndTargetMatch<T>(null, deleted, MatchType.OnlyInTarget, null);
                result.Add(match);
            }

            return result;
        }

        public Func<Type, string, List<object>, Exception> MultipleMatchesExceptionGetter { get; set; } = GetDefaultMultipleMatchesError;
        public ChangeUpdater<T> SetMultipleMatchesExceptionGetter(Func<Type, string, List<object>, Exception> value)
        {
            this.MultipleMatchesExceptionGetter = value ?? GetDefaultMultipleMatchesError;
            return this;
        }
        private Exception GetMultipleMatchesError(Dictionary<Func<T, T, bool>, Func<T, object>> mandatoryMatchers, T sourceItem, Dictionary<string, SourceAndTargetMatch> parentMatches)
        {
            Type itemType = typeof(T);
            string itemDescription = this.DescriptionFactory?.Invoke(sourceItem);
            var matchValues = (mandatoryMatchers == null
                    ? Enumerable.Empty<object>()
                    : mandatoryMatchers.Where(m => m.Value != null).Select(m => m.Value(sourceItem))
                )
                .Union(parentMatches
                    .Select(x => _parentInfoGetters.TryGetValue(x.Key, default(Func<object, string>))?.Invoke(x.Value.Source))
                )
                .ToList();
            return this.MultipleMatchesExceptionGetter(itemType, itemDescription, matchValues);
        }

        public static Exception GetDefaultMultipleMatchesError(Type itemType, string itemDescription, List<object> matchValues)
        {
            throw new ArgumentException($"Multiple matches found for {itemType.Name + (itemDescription == null ? "" : ": " + itemDescription)}: " +
                matchValues
                .Select(v => v == null ? "<null>" : string.IsNullOrWhiteSpace(v.ToString()) ? "<whitespace>" : v.ToString())
                .JoinStrings(", ")
            );
        }

        public Dictionary<object, object> ObjectSubstituations { get; set; } = new Dictionary<object, object>();
        public ChangeUpdater<T> SetObjectSubstituations(Dictionary<object, object> value)
        {
            this.ObjectSubstituations = value;
            return this;
        }

        public SourceAndTargetMatch<T> UpdateChanges<Tparent>(SourceAndTargetMatch<Tparent> parent, Expression<Func<Tparent, T>> getChild, string requiredSourceProperty = null)
            where Tparent : class, new()
        {
            if (parent == null || parent.MatchType == MatchType.ExcludedSource || parent.MatchType == MatchType.ExcludedTarget)
                return null;

            if (this.RequireExplicitInclusionInParent || requiredSourceProperty != null)
            {
                if (requiredSourceProperty == null)
                    requiredSourceProperty = GetPropertyName(getChild);
                List<string> parentUpdatedProperties;
                if (parent.Source != null && (!_allUpdatedProperties.TryGetValue(parent.Source, out parentUpdatedProperties) || !parentUpdatedProperties.Contains(requiredSourceProperty)))
                {
                    //Debug.WriteLine(requiredSourceProperty);
                    return null;
                }
                else if (parent.Source == null)
                    //Note in case the parent was deleted (parent.Source == null) we assume cascade delete will work
                    return null;
            }

            var getChildFunc = getChild.Compile();
            T sourceChild = parent.Source == null ? null : getChildFunc(parent.Source);
            T targetChild = getChildFunc(parent.Target);
            var result = this.UpdateChanges(sourceChild, targetChild);
            if (result == null)
                return null;
            if (result.MatchType != MatchType.InSourceAndTarget && result.MatchType != MatchType.InSourceAndAdditionalTarget)
            {
                var parentParam = getChild.Parameters.First();
                var childParam = Expression.Parameter(typeof(T), "c");
                var setChildFunc = Expression.Lambda<Action<Tparent, T>>(
                    Expression.Assign(getChild.Body, childParam),
                    parentParam, childParam
                ).Compile();
                if (result.MatchType == MatchType.OnlyInSource)
                    setChildFunc(parent.Target, result.Target);
                else
                    setChildFunc(parent.Target, null);
            }
            return result;
        }

        public IEnumerable<SourceAndTargetMatch<T>> UpdateChanges<Tparent>(
            SourceAndTargetMatch<Tparent> parent, 
            Expression<Func<Tparent, IEnumerable<T>>> getRelated, 
            IEnumerable<T> additionalTargets = null, 
            IEnumerable<SourceAndTargetMatch> otherParents = null, 
            string requiredSourceProperty = null, 
            bool allowMultipleTargets = false, 
            bool allowMultipleSources = false,
            ISpecification<T> specificationForMatches = null)
            where Tparent : class, new()
        {
            if (parent == null || parent.MatchType == MatchType.ExcludedSource || parent.MatchType == MatchType.ExcludedTarget)
                return Enumerable.Empty<SourceAndTargetMatch<T>>();

            if (this.RequireExplicitInclusionInParent || requiredSourceProperty != null)
            {
                if (requiredSourceProperty == null)
                    requiredSourceProperty = GetPropertyName(getRelated);
                List<string> parentUpdatedProperties;
                if (parent.Source != null && (!_allUpdatedProperties.TryGetValue(parent.Source, out parentUpdatedProperties) || !parentUpdatedProperties.Contains(requiredSourceProperty)))
                {
                    //Debug.WriteLine(requiredSourceProperty);
                    return Enumerable.Empty<SourceAndTargetMatch<T>>();
                }
                else if (parent.Source == null)
                    //Note in case the parent was deleted (parent.Source == null) we assume cascade delete will work
                    return Enumerable.Empty<SourceAndTargetMatch<T>>();
            }

            var getRelatedFunc = getRelated.Compile();
            var sourceRelated = parent.Source == null ? new List<T>() : getRelatedFunc(parent.Source);
            var targetRelated = getRelatedFunc(parent.Target);
            return this.UpdateChanges(sourceRelated, targetRelated, additionalTargets, otherParents, allowMultipleTargets: allowMultipleTargets, allowMultipleSources: allowMultipleSources, specificationForMatches: specificationForMatches);
        }

        private string GetPropertyName(LambdaExpression expression)
        {
            var exp = expression.Body;
            while (exp.NodeType == ExpressionType.Convert)
                exp = ((UnaryExpression)exp).Operand;
            if (exp is MemberExpression && ((MemberExpression)exp).Expression is ParameterExpression)
                return ((MemberExpression)exp).Member.Name;
            else
                throw new ArgumentException($"Property name for expression {expression.ToString()} cannot be determined");
        }

        public IEnumerable<SourceAndTargetMatch<T>> UpdateChanges(IEnumerable<T> source, IEnumerable<T> target, IEnumerable<T> additionalTargets = null, IEnumerable<SourceAndTargetMatch> parents = null, bool allowMultipleTargets = false, bool allowMultipleSources = false, ISpecification<T> specificationForMatches = null)
        {
            var matches = FindMatches(source, target, additionalTargets, parents, allowMultipleTargets: allowMultipleTargets, allowMultipleSources: allowMultipleSources, specificationForMatches: specificationForMatches);
            foreach (var match in matches)
                UpdateChanges(match, target as List<T>);
            return matches;
        }

        public SourceAndTargetMatch<T> UpdateChanges(SourceAndTargetMatch<T> match, List<T> targetList = null)
        {
            if (match.MatchType != MatchType.ExcludedSource && match.MatchType != MatchType.ExcludedTarget)
            {
                if (match.MatchType != MatchType.OnlyInTarget)
                {
                    UpdateRelevantProperties(match.Source, match.Target, null);
                    MatchParent(match);
                }

                if (match.MatchType == MatchType.OnlyInTarget)
                {
                    DoDelete(match.Target, targetList);
                    MatchParent(match);
                }
                else if (match.MatchType == MatchType.OnlyInSource)
                {
                    MatchParent(match);
                    DoAdd(match.Target, targetList);
                }
            }

            return match;
        }

        private void MatchParent(SourceAndTargetMatch<T> match)
        {
            if (match.ParentMatches != null)
            {
                foreach (var parentMatch in match.ParentMatches)
                {
                    _parentSetters[parentMatch.Key].Invoke(match.Target, parentMatch.Value?.Target);
                }
            }
        }

        public SourceAndTargetMatch<T> UpdateChanges(T source, T target)
        {
            if (source == null && target == null)
                return null;
            var targetIsNew = false;
            if (target == null)
            {
                target = new T();
                targetIsNew = true;
            }
            if (source != null)
                UpdateRelevantProperties(source, target, null);
            if (source == null) //Delete
                DoDelete(target, null);
            else if (targetIsNew)
                DoAdd(target, null);
            return new Common.SourceAndTargetMatch<T>(source, target, targetIsNew ? MatchType.OnlyInSource : MatchType.InSourceAndTarget, null);
        }

        public Func<T, bool> DeletePredicate { get; set; }
        public ChangeUpdater<T> SetDeletePredicate(Func<T, bool> predicate)
        {
            this.DeletePredicate = predicate;
            return this;
        }

        public Func<T, bool> AddPredicate { get; set; }
        public ChangeUpdater<T> SetAddPredicate(Func<T, bool> predicate)
        {
            this.AddPredicate = predicate;
            return this;
        }


        public void DoDelete(T item, List<T> targetList = null)
        {
            if (_deleteAction != null && (this.DeletePredicate == null || this.DeletePredicate(item)))
            {
                _deleteAction.Invoke(item);
                if (targetList != null)
                    targetList.Remove(item);
            }
        }

        public void DoAdd(T item, List<T> targetList = null)
        {
            if (_addAction != null && (this.AddPredicate == null || this.AddPredicate(item)))
            {
                if (targetList != null)
                {
                    targetList.Add(item);
                }

                _addAction.Invoke(item);
            }
        }

        public void UpdateRelevantProperties(T source, T target, params string[] properties)
        {
            UpdateRelevantProperties(source, target, properties != null && properties.Length > 0 ? (IEnumerable<string>)properties : default(IEnumerable<string>));
        }
        public void UpdateRelevantProperties(T source, T target, IEnumerable<string> properties)
        {
            this.ObjectSubstituations[source] = target;
            this.ObjectSubstituations[target] = source;
            List<string> propertiesFromAllUpdatedProperties;
            if (_allUpdatedProperties == null
                || (!_allUpdatedProperties.TryGetValue(source, out propertiesFromAllUpdatedProperties)
                    && !_allUpdatedProperties.TryGetValue(target, out propertiesFromAllUpdatedProperties)))
                propertiesFromAllUpdatedProperties = null;

            if (!this.RequireExplicitPropertyInclusion || properties != null || propertiesFromAllUpdatedProperties != null || _updatedProperties != null)
            {
                List<string> propertiesFromAllNonUpdatedProperties;
                if (_allNonUpdatedProperties == null
                    || (!_allNonUpdatedProperties.TryGetValue(source, out propertiesFromAllNonUpdatedProperties)
                        && !_allNonUpdatedProperties.TryGetValue(target, out propertiesFromAllNonUpdatedProperties)))
                    propertiesFromAllNonUpdatedProperties = null;
                foreach (var property in target.GetType().GetProperties()
                    .Where(p =>
                        (p.PropertyType.IsSimpleType() ||( _updatedProperties != null && _updatedProperties.Contains(p.Name)))
                        && (properties == null || properties.Contains(p.Name))
                        && (propertiesFromAllUpdatedProperties == null || propertiesFromAllUpdatedProperties.Contains(p.Name))
                        && (propertiesFromAllNonUpdatedProperties == null || !propertiesFromAllNonUpdatedProperties.Contains(p.Name))
                        && (_updatedProperties == null || _updatedProperties.Contains(p.Name))
                        && !_nonUpdatedProperties.Contains(p.Name)
                    ))
                {
                    var getter = property.GetGetterFunc();
                    var sourceValue = getter(source);
                    var targetValue = getter(target);
                    if (!object.Equals(sourceValue, targetValue))
                        property.GetSetterAction()(target, sourceValue);
                }
            }
        }

        Func<T, bool> _includeSourceWhen;
        public ChangeUpdater<T> IncludeSourceWhen(Func<T, bool> predicate)
        {
            _includeSourceWhen = predicate;
            return this;
        }
        public ChangeUpdater<T> IncludeTargetWhen(Func<T, bool> predicate)
        {
            _includeTargetWhen = predicate;
            return this;
        }
        Func<T, bool> _includeTargetWhen;
        public ChangeUpdater<T> IncludeWhen(Func<T, bool> predicate)
        {
            return this.IncludeSourceWhen(predicate).IncludeTargetWhen(predicate);
        }
    }




    public class SourceAndTargetMatch
    {
        public Dictionary<string, SourceAndTargetMatch> ParentMatches { get; private set; }

        public SourceAndTargetMatch(object source, object target, MatchType matchType, Dictionary<string, SourceAndTargetMatch> parentMatches)
        {
            if (source == null && target == null)
                throw new ArgumentNullException("Source and target cannot both be null");
            this.Source = source;
            this.Target = target;
            this.MatchType = matchType;
            this.ParentMatches = parentMatches;
        }

        public MatchType MatchType { get; private set; }
        public object Source { get; private set; }
        public object Target { get; private set; }

        public override int GetHashCode()
        {
            return (this.Source?.GetHashCode() ?? 0) ^ (this.Target?.GetHashCode() ?? 0) ^ this.MatchType.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var match = obj as SourceAndTargetMatch;
            return match == null || (
                object.Equals(this.Source, match.Source)
                && object.Equals(this.Target, match.Target)
                && this.MatchType == match.MatchType
            );
        }

        public SourceAndTargetMatch<Tparent> GetParentMatch<Tparent>(string key)
            where Tparent : class, new()
        {
            return (SourceAndTargetMatch<Tparent>)this.ParentMatches?[key];
        }
    }

    public enum MatchType
    {
        OnlyInSource = 1,
        InSourceAndTarget = 2,
        OnlyInTarget = 3,
        ExcludedSource = 4,
        ExcludedTarget = 5,
        InSourceAndAdditionalTarget = 6
    }


    public class SourceAndTargetMatch<T> : SourceAndTargetMatch
        where T : class, new()
    {
        public SourceAndTargetMatch(T source, T target, MatchType matchType, Dictionary<string, SourceAndTargetMatch> parentMatches) : base(source, target, matchType, parentMatches)
        {
        }
        public new T Source { get { return (T)base.Source; } }
        public new T Target { get { return (T)base.Target; } }

        public SourceAndTargetMatch<T> FindAndSetTargetParent<Tparent>(IEnumerable<SourceAndTargetMatch<Tparent>> allParents, Expression<Func<T, Tparent>> getParent, string parentKey = null)
            where Tparent : class, new()
        {
            if (this.Source != null)
            {
                var sourceParent = getParent.Compile()(this.Source);
                if (sourceParent != null)
                {
                    var parentMatch = FindMatchForSource(allParents, sourceParent);
                    var childParam = getParent.Parameters.First();
                    var parentParam = Expression.Parameter(typeof(Tparent), "p");
                    var setParentAction = Expression.Lambda<Action<T, Tparent>>(
                        Expression.Assign(getParent.Body, parentParam),
                        childParam, parentParam
                    ).Compile();
                    setParentAction(this.Target, parentMatch?.Target);
                    if (parentKey != null)
                        this.ParentMatches[parentKey] = parentMatch;
                }
            }
            return this;
        }

        private static SourceAndTargetMatch<Titem> FindMatchForSource<Titem>(IEnumerable<SourceAndTargetMatch<Titem>> matches, Titem source)
            where Titem : class, new()
        {
            if (source == null)
                return null;
            else
                return matches.Where(x => x != null && x.MatchType != MatchType.ExcludedSource && x.MatchType != MatchType.ExcludedTarget).Single(x => x.Source == source);
        }

    }

}
