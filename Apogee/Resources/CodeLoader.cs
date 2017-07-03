using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;

namespace Apogee.Resources
{
    public class CodeLoader
    {
        public static object GetClass(string code, Type implments)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("MyCompilation", new[] { tree }, new[] { MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location) });
            var semanticModel = compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();

            var classSymbol = semanticModel.GetDeclaredSymbol(root.DescendantNodes().OfType<ClassDeclarationSyntax>().First());

            if (classSymbol.BaseType.Name == implments.Name)
            {
                var context = CSharpScript.RunAsync(code, 
                    ScriptOptions.Default.WithReferences(typeof(GameEngine).GetTypeInfo().Assembly)).Result;

                return context.ContinueWithAsync<object>($"return new {classSymbol.Name}();").Result.ReturnValue;
            }
            
            return null;
        }
    }
}