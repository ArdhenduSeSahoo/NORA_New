//Utilies to compile code coming from different sources.
//Basically wrappers over the CSharpCodeProvider object.
//using Microsoft.CSharp;
//using System.CodeDom.Compiler;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Damco.Common;
using System.IO;
using System.Security.Policy;
using System.Security;
using System.Runtime.Remoting;
using System.Linq.Expressions;
using System.CodeDom.Compiler;

namespace Damco.Common
{
    public interface IDynamicCodeClass
    {
        object DoWork(int method, params object[] parameters);
    }
    //public class DynamicCodeClassWrapper : IDynamicCodeClass
    //{
    //    IDynamicCodeClass _wrapped;
    //    public DynamicCodeClassWrapper(IDynamicCodeClass wrapped)
    //    {
    //        _wrapped = wrapped;
    //    }
    //    public object DoWork(int method, params object[] parameters)
    //    {
    //        return _wrapped.DoWork(method, parameters);
    //    }
    //}

    public class DynamicCode : IDisposable
    {
        AppDomain _appDomain = null;
        string _tempFile = null;

        public DynamicCode(bool fullAccess)
        {
            this.FullAccess = fullAccess;
        }

        public bool FullAccess { get; private set; }

        public List<string> ReferencedAssemblyFileNames { get; } = new List<string>(new string[] { "mscorlib.dll", "System.Core.dll", "System.Data.dll", @"Damco.Common.dll", @"NORA.Model.dll" });

        public List<string> UsingNamespaces { get; } = new List<string>(new string[] { "System", "System.Data", "System.Collections.Generic", "System.Linq", "System.Text", "Damco.Common", "NORA.Model", "System.Globalization" });

        public void AddReferencesAndUsings(params string[] references)
        {
            foreach (var reference in references)
            {   // For Email Real fields Include NORA.Model for Enum 
                this.ReferencedAssemblyFileNames.Add(reference + (reference.EndsWith(".dll") ? "" : ".dll"));
                this.UsingNamespaces.Add(reference.EndsWith(".dll") ? reference.SubstringBefore(".dll", StringFindOptions.SearchFromEnd) : reference);
            }
        }

        List<string> _classes = new List<string>();
        List<Tuple<string, string, int>> _methods = new List<Tuple<string, string, int>>();

        IDynamicCodeClass _program;

        private IDynamicCodeClass GetProgram()
        {
            if (_program == null)
                throw new InvalidOperationException("Methods cannot be called until after .Compile() was used");
            return _program;
        }

        private int GetNextTextStartLine(string previousText)
        {
            if (string.IsNullOrWhiteSpace(previousText))
                return 1;
            else
                return previousText.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n').Length;
        }

        public void Compile()
        {
            List<int> _methodStarts = new List<int>();

            StringBuilder codeBuilder = new StringBuilder(
$@"
{this.UsingNamespaces.Distinct().Select(item => "using " + item + ";").JoinStrings("\n")}
namespace Damco.Common.DynamicCode.Dynamic
{{
    public class Program: MarshalByRefObject, IDynamicCodeClass
    {{
        object IDynamicCodeClass.DoWork(int method, params object[] parameters) 
        {{ 
            switch(method)
            {{
{
                    _methods.Select((m, i) => $@"                case {i}: return {m.Item2};").JoinStrings("\n")
}
                default: throw new ArgumentException(""Invalid method index"", ""method"");
            }}
        }}
        {_additionalCode.JoinStrings("\n")}
");

            foreach (var method in _methods)
            {
                _methodStarts.Add(GetNextTextStartLine(codeBuilder.ToString()) + method.Item3);
                codeBuilder.AppendLine(method.Item1);
            }

            codeBuilder.Append(
$@"
    }}
}}");

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            string dllPath = Path.GetDirectoryName(typeof(Damco.Common.DynamicCode).Assembly.GetName().CodeBase.SubstringAfter("file:///").Replace("/", "\\"));
            foreach (string reference in this.ReferencedAssemblyFileNames)
            {
                string realReference = reference;
                if (!reference.Contains("\\"))
                {
                    string possibleFileName = Files.CombinePathsAndSimplify(dllPath, reference);
                    if (System.IO.File.Exists(possibleFileName))
                        realReference = possibleFileName;
                }
                parameters.ReferencedAssemblies.Add(realReference);
            }
            string dllFile = null;
            if (this.FullAccess)
                parameters.GenerateInMemory = true;
            else
            {
                //We need a real dll to give to the separate app domain
                dllFile = Path.GetTempFileName();
                _tempFile = dllFile;
                parameters.GenerateInMemory = false;
                parameters.OutputAssembly = dllFile;
            }

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, codeBuilder.ToString());
            if (results.Errors.HasErrors)
            {
                StringBuilder errors = new StringBuilder();
                foreach (var error in results.Errors.Cast<CompilerError>())
                {
                    int lineNumber = error.Line - _methodStarts.Where(x => x <= error.Line).DefaultIfEmpty(0).Max() + 1;
                    errors.AppendLine(
                        string.Format("Error {0} at line {1}: {2}",
                            error.ErrorNumber,
                            lineNumber,
                            error.ErrorText
                        )
                    );
                }
                throw new InvalidOperationException(errors.ToString());
            }

            if (this.FullAccess)
            {
                Assembly assembly = results.CompiledAssembly;
                _program = (IDynamicCodeClass)assembly.GetType("Damco.Common.DynamicCode.Dynamic.Program").GetConstructor(new Type[] { }).Invoke(new object[] { });
            }
            else
            {
                ////Run in a separate AppDomain with limited access
                ////so that code can't e.g. delete files or format the C drive
                //AppDomainSetup info = new AppDomainSetup();
                ////info.ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //info.ApplicationBase = dllPath; // Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //Evidence ev = new Evidence();
                //ev.AddHostEvidence(new System.Security.Policy.Zone(SecurityZone.Internet));
                //_appDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, info, SecurityManager.GetStandardSandbox(ev));
                ////TODO: Check if the current app domain is reachable from the new one somehow
                //ObjectHandle handle = Activator.CreateInstanceFrom(_appDomain, dllFile, "Damco.Common.DynamicCode.Dynamic.Program");
                //_program = (Damco.Common.IDynamicCodeClass)handle.Unwrap();
            }
        }

        static FieldInfo _compilerSettingsField = typeof(CSharpCodeProvider).GetField("_compilerSettings", BindingFlags.Instance | BindingFlags.NonPublic);
        public string GetCompilerPath(CSharpCodeProvider provider)
        {
            // Little hack here, see http://stackoverflow.com/a/40311406/1676558.
            object compilerSettings = _compilerSettingsField.GetValue(provider);
            FieldInfo compilerSettingsFullPathField = compilerSettings
                .GetType()
                .GetField("_compilerFullPath", BindingFlags.Instance | BindingFlags.NonPublic);
            return (string)compilerSettingsFullPathField.GetValue(compilerSettings);
        }
        public void SetCompilerPath(CSharpCodeProvider provider, string path)
        {
            object compilerSettings = _compilerSettingsField.GetValue(provider);
            FieldInfo compilerSettingsFullPathField = compilerSettings
                .GetType()
                .GetField("_compilerFullPath", BindingFlags.Instance | BindingFlags.NonPublic);
            compilerSettingsFullPathField.SetValue(compilerSettings, path);
        }

        List<string> _additionalCode = new List<string>();
        public void AddCode(string code)
        {
            _additionalCode.Add(code);
        }


        public T CreateTextTemplateMethod<T>(string template, string initialization, params string[] parameterNames)
        {
            template = template.Replace("\r\n", "\n");

            //Normal C# stuff
            StringBlock normalString = new StringBlock("\"", "\"");
            StringBlock atSignString = new StringBlock("@\"", "\"");
            StringBlock interpolatedString = new StringBlock("$\"", "\"");
            StringBlock interpolatedAtSignString = new StringBlock("$@\"", "\"");
            StringBlock interpolationExpression = new StringBlock("{", "}");
            StringBlock commentBlock = new StringBlock("/*", "*/");
            StringBlock inlineComment = new StringBlock("//", "\n");
            StringBlock escapeInString = new StringBlock("\\", 2);
            StringBlock escapeInAtSignString = new StringBlock("\"\"", 2);
            StringBlock escapedStartAccolade = new StringBlock("{{", 2);
            StringBlock escapedEndAccolade = new StringBlock("}}", 2);
            normalString.AddChildren(escapeInString);
            atSignString.AddChildren(escapeInAtSignString);
            interpolatedString.AddChildren(escapeInString, interpolationExpression, escapedStartAccolade, escapedEndAccolade);
            interpolatedAtSignString.AddChildren(escapeInAtSignString, interpolationExpression, escapedStartAccolade, escapedEndAccolade);
            interpolationExpression.AddChildren(normalString, atSignString, interpolatedString, interpolatedAtSignString, commentBlock, inlineComment);

            //Special template stuff
            StringBlock quotesInRoot = new StringBlock("\"", 1); //So the user does not have to double the quotes
            StringBlock codeBlockInRoot1 = new StringBlock("\n{%", StringFindOptions.None, new string[] { "%}\n", "%}" }, StringFindOptions.None);
            StringBlock codeBlockInRoot2 = new StringBlock("{%", StringFindOptions.None, new string[] { "%}\n", "%}" }, StringFindOptions.None);
            StringBlock forEach = new StringBlock("{ForEach", StringFindOptions.CaseInsensitive, "}", StringFindOptions.CaseInsensitive);
            StringBlock endForEach = new StringBlock("{EndForEach}", StringFindOptions.CaseInsensitive, 12);
            StringBlock forEachSeparator = new StringBlock("{SeparateBy}", StringFindOptions.CaseInsensitive, 12);
            StringBlock ifStatement = new StringBlock("{If", StringFindOptions.CaseInsensitive, "}", StringFindOptions.CaseInsensitive);
            StringBlock elseIf = new StringBlock("{ElseIf", StringFindOptions.CaseInsensitive, "}", StringFindOptions.CaseInsensitive);
            StringBlock elseStatement = new StringBlock("{Else}", StringFindOptions.CaseInsensitive, 6);
            StringBlock endIf = new StringBlock("{EndIf}", StringFindOptions.CaseInsensitive, 7);
            codeBlockInRoot1.AddChildren(normalString, atSignString, interpolatedString, interpolatedAtSignString, commentBlock, inlineComment);
            codeBlockInRoot2.AddChildren(normalString, atSignString, interpolatedString, interpolatedAtSignString, commentBlock, inlineComment);
            forEach.AddChildren(normalString, atSignString, interpolatedString, interpolatedAtSignString, commentBlock, inlineComment);
            ifStatement.AddChildren(normalString, atSignString, interpolatedString, interpolatedAtSignString, commentBlock, inlineComment);
            elseIf.AddChildren(normalString, atSignString, interpolatedString, interpolatedAtSignString, commentBlock, inlineComment);

            var code = string.Concat(
                initialization ?? "",
                "string forEachSeparator=string.Empty;StringBuilder output = new StringBuilder(); output.Append($@\"",
                template.ReplaceBlocks((b, s) =>
                {
                    if (b == quotesInRoot)
                        return "\"\"";
                    else if (b == codeBlockInRoot1) //Code block that starts on a new line
                        return $"\");{"\n" + s.Substring(3, s.Length - 5 - (s.EndsWith("\n") ? 1 : 0)) + (s.EndsWith("\n") ? "\n" : "")}output.Append($@\"";
                    else if (b == codeBlockInRoot2) //Code block that does not start on a new line
                        return $"\");{s.Substring(2, s.Length - 4 - (s.EndsWith("\n") ? 1 : 0)) + (s.EndsWith("\n") ? "\n" : "")}output.Append($@\"";
                    else if (b == forEach)
                    {
                        var forEachInstruction = s.Substring("{FOREACH".Length, s.Length - "{FOREACH".Length - 1);
                        var loopVariable = forEachInstruction.SubstringBefore(" in ", StringFindOptions.CaseInsensitive);
                        var listExpression = forEachInstruction.SubstringAfter(" in ", StringFindOptions.CaseInsensitive);
                        return $"\");{{int {loopVariable}_counter=0;forEachSeparator=null;foreach(var {loopVariable} in {listExpression}){{{loopVariable}_counter++;if(forEachSeparator!=null)output.Append(forEachSeparator);output.Append($@\"";
                    }
                    else if (b == forEachSeparator)
                        return $"\");forEachSeparator=($@\"";
                    else if (b == endForEach)
                        return $"\");}}}}forEachSeparator=null;output.Append($@\"";
                    else if (b == ifStatement)
                    {
                        var condition = s.Substring("{If".Length, s.Length - "{If".Length - 1);
                        return $"\");if({condition}) {{output.Append($@\"";
                    }
                    else if (b == elseIf)
                    {
                        var condition = s.Substring("{ElseIf".Length, s.Length - "{ElseIf".Length - 1);
                        return $"\");}}else if({condition}) {{output.Append($@\"";
                    }
                    else if (b == elseStatement)
                        return $"\");}}else{{output.Append($@\"";
                    else if (b == endIf)
                        return $"\");}}output.Append($@\"";
                    else
                        return s;
                }, quotesInRoot, interpolationExpression, escapedStartAccolade, escapedEndAccolade, codeBlockInRoot1, codeBlockInRoot2, forEach, forEachSeparator, endForEach, ifStatement, elseStatement, elseIf, endIf),
                "\");\nreturn output.ToString();"
            );

            return CreateMethod<T>(code, GetNextTextStartLine(initialization) - 1, parameterNames);
        }

        public T CreateMethod<T>(string code, params string[] parameterNames)
        {
            return CreateMethod<T>(code, 0, parameterNames);
        }

        private T CreateMethod<T>(string code, int linesNonUserCode, params string[] parameterNames)
        {
            if (!typeof(MulticastDelegate).IsAssignableFrom(typeof(T).BaseType))
                throw new ArgumentException("Type 'T' must be a delegate type", "T");

            MethodInfo methodSignature = typeof(T).GetMethod("Invoke");
            int methodIndex = _methods.Count;

            code = $@"
        public {GetTypeCSharpText(methodSignature.ReturnType)} DoWork{methodIndex}({methodSignature.GetParameters().Select((p, i) => $"{GetTypeCSharpText(p.ParameterType)} {(parameterNames != null && parameterNames.Length > i ? parameterNames[i] : p.Name)}").JoinStrings(",")})
        {{
            {code}
        }}";

            string methodCall = $@"DoWork{methodIndex}({methodSignature.GetParameters().Select((p, i) => $"({GetTypeCSharpText(p.ParameterType)})parameters[{i}]").JoinStrings(",")})";

            //After .Compile(), Program will have the correct method but because it can be in another app domain 
            //we can't get to it directly - "program" is a MarshallByRefObject
            //We need an interface in order to use a MarshallByRefObject
            //so we have let Program implement IDynamicCodeClass which has a generic version of 
            //DoWork that executes the "real" DoWork.
            //Now we just need to build the delegate the caller expexts, and let it call the 
            //generic DoWork.
            //Note we need to wrap "program" because we can't build an expression that directly
            //calls the method on the MarshallByRefObject.
            var arguments = methodSignature.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToList();
            //Expression.Call(Expression.Constant(this), typeof(DynamicCode).GetMethod("GetProgram", BindingFlags.NonPublic | BindingFlags.Instance) Expression.Constant(new DynamicCodeClassWrapper(program))), //Get _program - only works after .Compile was used

            Expression resultBody =
                Expression.Call(
                    Expression.Call(Expression.Constant(this), typeof(DynamicCode).GetMethod("GetProgram", BindingFlags.NonPublic | BindingFlags.Instance)), //Get _program - only works after .Compile was used
                    typeof(IDynamicCodeClass).GetMethod("DoWork"),
                    Expression.Constant(methodIndex),
                    Expression.NewArrayInit(typeof(object), arguments.Select(a => Expression.Convert(a, typeof(object))))
                ); //Call IDynamicCodeClass.DoWork with all the arguments - as an object array. It will find the correct method and execute it
            if (methodSignature.ReturnType != null && methodSignature.ReturnType != typeof(void) && methodSignature.ReturnType != typeof(object))
                resultBody = Expression.Convert(resultBody, methodSignature.ReturnType); //Convert return type to match what the caller expexcts
                                                                                         //resultBody now matches the delegate the caller expects

            _methods.Add(Tuple.Create(code, methodCall, 3 + linesNonUserCode));

            return Expression.Lambda<T>(resultBody, arguments).Compile();
        }

        public static string GetTypeCSharpText(Type type)
        {
            if (type == null || type == typeof(void))
                return "void";

            if (type.IsGenericTypeDefinition)
                throw new ArgumentException("Generic type definitions are not supported as they cannot be used in code directly; please use a specific implementation of the generic type");

            //start with basic types
            if (type == typeof(object)) return "object";
            else if (type == typeof(string)) return "string";
            else if (type == typeof(sbyte)) return "sbyte";
            else if (type == typeof(short)) return "short";
            else if (type == typeof(int)) return "int";
            else if (type == typeof(long)) return "long";
            else if (type == typeof(byte)) return "byte";
            else if (type == typeof(ushort)) return "ushort";
            else if (type == typeof(uint)) return "uint";
            else if (type == typeof(ulong)) return "ulong";
            else if (type == typeof(float)) return "float";
            else if (type == typeof(double)) return "double";
            else if (type == typeof(bool)) return "bool";
            else if (type == typeof(char)) return "char";
            else if (type == typeof(decimal)) return "decimal";
            else if (Nullable.GetUnderlyingType(type) != null)
                return string.Concat(GetTypeCSharpText(Nullable.GetUnderlyingType(type)), "?");
            else if (type.GetGenericArguments().Length > 0)
                return string.Concat(
                    type.FullName.SubstringBefore("`"),
                    "<",
                    string.Join(",", type.GetGenericArguments().Select(a => GetTypeCSharpText(a))),
                    ">");
            else if (type.FullName.Contains(","))
                return type.FullName.SubstringBefore(",");
            else
                return type.FullName;
        }

        public void Dispose()
        {
            if (_appDomain != null)
            {
                try
                {
                    AppDomain.Unload(_appDomain);
                }
                catch { }
            }
            if (_tempFile != null)
            {
                try
                {
                    System.IO.File.Delete(_tempFile);
                }
                catch { }
            }
        }
    }
}
