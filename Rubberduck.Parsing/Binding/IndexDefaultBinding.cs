﻿using Antlr4.Runtime;
using Rubberduck.Parsing.Symbols;
using System.Linq;

namespace Rubberduck.Parsing.Binding
{
    public sealed class IndexDefaultBinding : IExpressionBinding
    {
        private readonly DeclarationFinder _declarationFinder;
        private readonly Declaration _project;
        private readonly Declaration _module;
        private readonly Declaration _parent;
        private readonly VBAExpressionParser.IndexExpressionContext _indexExpression;
        private readonly VBAExpressionParser.IndexExprContext _indexExpr;
        private readonly ParserRuleContext _unknownOriginExpr;
        private readonly IExpressionBinding _lExpressionBinding;
        private readonly ArgumentList _argumentList;

        private const int DEFAULT_MEMBER_RECURSION_LIMIT = 32;
        private int _defaultMemberRecursionLimitCounter = 0;

        public IndexDefaultBinding(
            DeclarationFinder declarationFinder,
            Declaration project,
            Declaration module,
            Declaration parent,
            VBAExpressionParser.IndexExpressionContext expression,
            IExpressionBinding lExpressionBinding)
        {
            _declarationFinder = declarationFinder;
            _project = project;
            _module = module;
            _parent = parent;
            _indexExpression = expression;
            _lExpressionBinding = lExpressionBinding;
            _argumentList = ConvertContextToArgumentList(GetArgumentListContext());
        }

        public IndexDefaultBinding(
            DeclarationFinder declarationFinder,
            Declaration project,
            Declaration module,
            Declaration parent,
            VBAExpressionParser.IndexExprContext expression,
            IExpressionBinding lExpressionBinding)
        {
            _declarationFinder = declarationFinder;
            _project = project;
            _module = module;
            _parent = parent;
            _indexExpr = expression;
            _lExpressionBinding = lExpressionBinding;
            _argumentList = ConvertContextToArgumentList(GetArgumentListContext());
        }

        public IndexDefaultBinding(
            DeclarationFinder declarationFinder,
            Declaration project,
            Declaration module,
            Declaration parent,
            ParserRuleContext expression,
            IExpressionBinding lExpressionBinding,
            ArgumentList argumentList)
        {
            _declarationFinder = declarationFinder;
            _project = project;
            _module = module;
            _parent = parent;
            _unknownOriginExpr = expression;
            _lExpressionBinding = lExpressionBinding;
            _argumentList = argumentList;
        }

        private ParserRuleContext GetExpressionContext()
        {
            if (_indexExpression != null)
            {
                return _indexExpression;
            }
            if (_indexExpr != null)
            {
                return _indexExpr;
            }
            return _unknownOriginExpr;
        }

        private VBAExpressionParser.ArgumentListContext GetArgumentListContext()
        {
            if (_indexExpression != null)
            {
                return _indexExpression.argumentList();
            }
            return _indexExpr.argumentList();
        }

        private ArgumentList ConvertContextToArgumentList(VBAExpressionParser.ArgumentListContext argumentList)
        {
            var convertedList = new ArgumentList();
            var list = argumentList.positionalOrNamedArgumentList();
            if (list.positionalArgument() != null)
            {
                foreach (var expr in list.positionalArgument())
                {
                    convertedList.AddArgument(ArgumentListArgumentType.Positional);
                }
            }
            if (list.requiredPositionalArgument() != null)
            {
                convertedList.AddArgument(ArgumentListArgumentType.Positional);
            }
            if (list.namedArgumentList() != null)
            {
                foreach (var expr in list.namedArgumentList().namedArgument())
                {
                    convertedList.AddArgument(ArgumentListArgumentType.Named);
                }
            }
            return convertedList;
        }

        public IBoundExpression Resolve()
        {
            var lExpression = _lExpressionBinding.Resolve();
            return Resolve(lExpression);
        }

        private IBoundExpression Resolve(IBoundExpression lExpression)
        {
            IBoundExpression boundExpression = null;
            if (lExpression == null)
            {
                return null;
            }
            boundExpression = ResolveLExpressionIsVariablePropertyFunctionNoParameters(lExpression);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            boundExpression = ResolveLExpressionIsPropertyFunctionSubroutine(lExpression);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            boundExpression = ResolveLExpressionIsUnbound(lExpression);
            if (boundExpression != null)
            {
                return boundExpression;
            }
            return null;
        }

        private IBoundExpression ResolveLExpressionIsVariablePropertyFunctionNoParameters(IBoundExpression lExpression)
        {
            /*
             <l-expression> is classified as a variable, or <l-expression> is classified as a property or function 
                    with a parameter list that cannot accept any parameters and an <argument-list> that is not 
                    empty, and one of the following is true (see below):
             */
            bool isVariable = lExpression.Classification == ExpressionClassification.Variable;
            bool propertyWithParameters = lExpression.Classification == ExpressionClassification.Property && ((IDeclarationWithParameter)lExpression.ReferencedDeclaration).Parameters.Any();
            bool functionWithParameters = lExpression.Classification == ExpressionClassification.Function && ((IDeclarationWithParameter)lExpression.ReferencedDeclaration).Parameters.Any();
            if (isVariable ||
                ((!propertyWithParameters || !functionWithParameters)) && _argumentList.HasArguments)
            {
                IBoundExpression boundExpression = null;
                var asTypeName = lExpression.ReferencedDeclaration.AsTypeName;
                var asTypeDeclaration = lExpression.ReferencedDeclaration.AsTypeDeclaration;
                boundExpression = ResolveDefaultMember(lExpression, asTypeName, asTypeDeclaration);
                if (boundExpression != null)
                {
                    return boundExpression;
                }
                boundExpression = ResolveLExpressionDeclaredTypeIsArray(lExpression, asTypeDeclaration);
                if (boundExpression != null)
                {
                    return boundExpression;
                }
                return boundExpression;
            }
            return null;
        }

        private IBoundExpression ResolveDefaultMember(IBoundExpression lExpression, string asTypeName, Declaration asTypeDeclaration)
        {
            /*
                The declared type of <l-expression> is Object or Variant, and <argument-list> contains no 
                named arguments. In this case, the index expression is classified as an unbound member with 
                a declared type of Variant, referencing <l-expression> with no member name. 
             */
            if (asTypeName != null && (asTypeName.ToUpperInvariant() == "VARIANT" || asTypeName.ToUpperInvariant() == "OBJECT"))
            {
                return new IndexExpression(null, ExpressionClassification.Unbound, GetExpressionContext(), lExpression);
            }
            /*
                The declared type of <l-expression> is a specific class, which has a public default Property 
                Get, Property Let, function or subroutine, and one of the following is true:
            */
            bool hasDefaultMember = asTypeDeclaration != null
                && asTypeDeclaration.DeclarationType == DeclarationType.ClassModule
                && ((ClassModuleDeclaration)asTypeDeclaration).DefaultMember != null;
            if (hasDefaultMember)
            {
                ClassModuleDeclaration classModule = (ClassModuleDeclaration)asTypeDeclaration;
                Declaration defaultMember = classModule.DefaultMember;
                bool isPropertyGetLetFunctionProcedure =
                    defaultMember.DeclarationType == DeclarationType.PropertyGet
                    || defaultMember.DeclarationType == DeclarationType.PropertyLet
                    || defaultMember.DeclarationType == DeclarationType.Function
                    || defaultMember.DeclarationType == DeclarationType.Procedure;
                bool isPublic =
                    defaultMember.Accessibility == Accessibility.Global
                    || defaultMember.Accessibility == Accessibility.Implicit
                    || defaultMember.Accessibility == Accessibility.Public;
                if (isPropertyGetLetFunctionProcedure && isPublic)
                {
                    /**
                        This default member cannot accept any parameters. In this case, the static analysis restarts 
                        recursively, as if this default member was specified instead for <l-expression> with the 
                        same <argument-list>.
                    */
                    if (((IDeclarationWithParameter)defaultMember).Parameters.Count() == 0)
                    {
                        // Recursion limit reached, abort.
                        if (DEFAULT_MEMBER_RECURSION_LIMIT == _defaultMemberRecursionLimitCounter)
                        {
                            return null;
                        }
                        _defaultMemberRecursionLimitCounter++;
                        ExpressionClassification classification;
                        if (defaultMember.DeclarationType.HasFlag(DeclarationType.Property))
                        {
                            classification = ExpressionClassification.Property;
                        }
                        else if (defaultMember.DeclarationType == DeclarationType.Procedure)
                        {
                            classification = ExpressionClassification.Subroutine;
                        }
                        else
                        {
                            classification = ExpressionClassification.Function;
                        }
                        var defaultMemberAsLExpression = new SimpleNameExpression(defaultMember, classification, GetExpressionContext());
                        return Resolve(defaultMemberAsLExpression);
                    }
                    else
                    {
                        /*
                            This default member’s parameter list is compatible with <argument-list>. In this case, the 
                            index expression references this default member and takes on its classification and 
                            declared type.  

                            Note: To not have to deal with implementing parameter compatibility ourselves we simply assume
                            that they are compatible otherwise it wouldn't have compiled in the VBE.
                         */
                        return new IndexExpression(defaultMember, lExpression.Classification, GetExpressionContext(), lExpression);
                    }
                }
            }
            return null;
        }

        private IBoundExpression ResolveLExpressionDeclaredTypeIsArray(IBoundExpression lExpression, Declaration asTypeDeclaration)
        {
            // TODO: Test this as soon as parser has as type fixed.

            /*
                 The declared type of <l-expression> is an array type, an empty argument list has not already 
                 been specified for it, and one of the following is true:  
             */
            if (asTypeDeclaration != null && asTypeDeclaration.IsArray())
            {
                /*
                    <argument-list> represents an empty argument list. In this case, the index expression 
                    takes on the classification and declared type of <l-expression> and references the same 
                    array.  
                 */
                if (!_argumentList.HasArguments)
                {
                    return new IndexExpression(asTypeDeclaration, lExpression.Classification, GetExpressionContext(), lExpression);
                }
                else
                {
                    /*
                        <argument-list> represents an argument list with a number of positional arguments equal 
                        to the rank of the array, and with no named arguments. In this case, the index expression 
                        references an individual element of the array, is classified as a variable and has the 
                        declared type of the array’s element type.  

                        Note: We assume this is the case without checking, enfored by the VBE.
                     */
                    if (!_argumentList.HasNamedArguments)
                    {
                        return new IndexExpression(asTypeDeclaration, ExpressionClassification.Variable, GetExpressionContext(), lExpression);
                    }
                }
            }
            return null;
        }

        private IBoundExpression ResolveLExpressionIsPropertyFunctionSubroutine(IBoundExpression lExpression)
        {
            /*
                    <l-expression> is classified as a property or function and its parameter list is compatible with 
                    <argument-list>. In this case, the index expression references <l-expression> and takes on its 
                    classification and declared type. 

                    <l-expression> is classified as a subroutine and its parameter list is compatible with <argument-
                    list>. In this case, the index expression references <l-expression> and takes on its classification 
                    and declared type.   

                    Note: We assume compatibility through enforcement by the VBE.
             */
            if (lExpression.Classification == ExpressionClassification.Property
               || lExpression.Classification == ExpressionClassification.Function
               || lExpression.Classification == ExpressionClassification.Subroutine)
            {
                return new IndexExpression(lExpression.ReferencedDeclaration, lExpression.Classification, GetExpressionContext(), lExpression);
            }
            return null;
        }

        private IBoundExpression ResolveLExpressionIsUnbound(IBoundExpression lExpression)
        {
            /*
                 <l-expression> is classified as an unbound member. In this case, the index expression references 
                 <l-expression>, is classified as an unbound member and its declared type is Variant.  
            */
            if (lExpression.Classification == ExpressionClassification.Unbound)
            {
                return new IndexExpression(lExpression.ReferencedDeclaration, ExpressionClassification.Unbound, GetExpressionContext(), lExpression);
            }
            return null;
        }
    }
}