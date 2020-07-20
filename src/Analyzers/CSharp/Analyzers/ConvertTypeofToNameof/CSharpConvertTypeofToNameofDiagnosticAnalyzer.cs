﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Microsoft.CodeAnalysis.CodeStyle;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.ConvertTypeOfToNameOf;
using Microsoft.CodeAnalysis.LanguageServices;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Shared.Extensions;

namespace Microsoft.CodeAnalysis.CSharp.ConvertTypeOfToNameOf
{
    /// <summary>
    /// Finds code like typeof(someType).Name and determines whether it can be changed to nameof(someType), if yes then it offers a diagnostic
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class CSharpConvertTypeOfToNameOfDiagnosticAnalyzer : AbstractConvertTypeOfToNameOfDiagnosticAnalyzer
    {
        public CSharpConvertTypeOfToNameOfDiagnosticAnalyzer()
        {
        }

        protected override bool IsValidTypeofAction(OperationAnalysisContext context)
        {
            var node = context.Operation.Syntax;
            var syntaxTree = node.SyntaxTree;
            // nameof was added in CSharp 6.0, so don't offer it for any languages before that time
            if (((CSharpParseOptions)syntaxTree.Options).LanguageVersion < LanguageVersion.CSharp6)
            {
                return false;
            }

            // Make sure that the syntax that we're looking at is actually a typeof expression and that
            // the parent syntax is a member access expression otherwise the syntax is not the kind of
            // expression that we want to analyze
            return node is TypeOfExpressionSyntax { Parent: MemberAccessExpressionSyntax _ };
        }

        protected override Diagnostic LanguageReportDiagnostic(Location location, DiagnosticDescriptor cSharpDescriptor, DiagnosticDescriptor visualBasicDescriptor)
        {
            return Diagnostic.Create(cSharpDescriptor, location);
        }
    }
}
