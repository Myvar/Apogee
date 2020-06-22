using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Apogee.Generators
{
    [Generator]
    public class HelloWorldGenerator : ISourceGenerator
    {
        public void Execute(SourceGeneratorContext context)
        {
            // begin creating the source we'll inject into the users compilation
            var sourceBuilder = new StringBuilder(@"
using System;
namespace HelloWorldGenerated
{
    public static class HelloWorld
    {
        public static void SayHello() 
        {
            Console.WriteLine(""Hello from generated code!"");
            Console.WriteLine(""The following syntax trees existed in the compilation that created this program:"");
");

            // using the context, get a list of syntax trees in the users compilation
            var syntaxTrees = context.Compilation.SyntaxTrees;


            // add the filepath of each tree to the class we're building
            foreach (CSharpSyntaxTree tree in syntaxTrees)
            {
                sourceBuilder.AppendLine($@"Console.WriteLine(@"" - {tree.FilePath}"");");
                sourceBuilder.AppendLine($@"Console.WriteLine(@"" - {tree.GetRoot().ChildNodes().Count()}"");");
                var root = tree.GetRoot();
                foreach (var childNode in root.ChildNodes())
                {
                    if (childNode is NamespaceDeclarationSyntax nds)
                    {
                        foreach (var node in nds.ChildNodes())
                        {
                            if (node is ClassDeclarationSyntax cds)
                            {
                                var isShader = cds.GetText().ToString().Split('{')[0].ToString().Trim().Split(":")
                                    .Last().Trim() == "Shader";

                                sourceBuilder.AppendLine(
                                    $@"Console.WriteLine(""{cds.Identifier}.isShader = {isShader}"");");
                                foreach (var syntaxNode in node.ChildNodes().ToArray())
                                {
                                    if (syntaxNode is PropertyDeclarationSyntax pds)
                                    {
                                        sourceBuilder.AppendLine($@"Console.WriteLine(""{pds.Identifier}"");");
                                        SyntaxToken newId = Identifier("Bob");
                                        root = root.ReplaceNode(pds, pds.WithIdentifier(newId));
                                    }
                                }
                            }
                        }
                    }
                }

             
            }

            // finish creating the source to inject
            sourceBuilder.Append(@"
        }
    }
}");

            // inject the created source into the users compilation
            context.AddSource("helloWorldGenerator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        public void Initialize(InitializationContext context)
        {
            // No initialization required for this one
        }
    }
}