﻿using NUnit.Framework;
using Rubberduck.RegexAssistant.Atoms;
using Rubberduck.RegexAssistant.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.RegexAssistant.Tests
{
    [TestFixture]
    public class RegularExpressionTests
    {
        [Category("RegexAssistant")]
        [Test]
        public void ParseSingleLiteralGroupAsAtomWorks()
        {
            var pattern = "(g){2,4}";
            var expression = VBRegexParser.Parse(pattern);
            Assert.IsInstanceOf(typeof(SingleAtomExpression), expression);
            Assert.AreEqual(new Group(new SingleAtomExpression(new Literal("g", Quantifier.None)), "(g)", new Quantifier("{2,4}")), (expression as SingleAtomExpression).Atom);
        }

        [Category("RegexAssistant")]
        [Test]
        public void ParseCharacterClassAsAtomWorks()
        {
            var pattern = "[abcd]*";
            var expression = VBRegexParser.Parse(pattern);
            Assert.IsInstanceOf(typeof(SingleAtomExpression), expression);
            Assert.AreEqual(new CharacterClass("[abcd]", new Quantifier("*")), (expression as SingleAtomExpression).Atom);
        }

        [Category("RegexAssistant")]
        [Test]
        public void ParseLiteralAsAtomWorks()
        {
            var pattern = "a";
            var expression = VBRegexParser.Parse(pattern);
            Assert.IsInstanceOf(typeof(SingleAtomExpression), expression);
            Assert.AreEqual(new Literal("a", Quantifier.None), (expression as SingleAtomExpression).Atom);
        }

        [Category("RegexAssistant")]
        [Test]
        public void ParseUnicodeEscapeAsAtomWorks()
        {
            var pattern = "\\u1234+";
            var expression = VBRegexParser.Parse(pattern);
            Assert.IsInstanceOf(typeof(SingleAtomExpression), expression);
            Assert.AreEqual(new Literal("\\u1234", new Quantifier("+")), (expression as SingleAtomExpression).Atom);
        }

        [Category("RegexAssistant")]
        [Test]
        public void ParseHexEscapeSequenceAsAtomWorks()
        {
            var pattern = "\\x12?";
            var expression = VBRegexParser.Parse(pattern);
            Assert.IsInstanceOf(typeof(SingleAtomExpression), expression);
            Assert.AreEqual(new Literal("\\x12", new Quantifier("?")), (expression as SingleAtomExpression).Atom);
        }

        [Category("RegexAssistant")]
        [Test]
        public void ParseOctalEscapeSequenceAsAtomWorks()
        {
            var pattern = "\\712{2}";
            var expression = VBRegexParser.Parse(pattern);
            Assert.IsInstanceOf(typeof(SingleAtomExpression), expression);
            Assert.AreEqual(new Literal("\\712", new Quantifier("{2}")), (expression as SingleAtomExpression).Atom);
        }

        [Category("RegexAssistant")]
        [Test]
        public void ParseEscapedLiteralAsAtomWorks()
        {
            var pattern = "\\)";
            var expression = VBRegexParser.Parse(pattern);
            Assert.IsInstanceOf(typeof(SingleAtomExpression), expression);
            Assert.AreEqual(new Literal("\\)", Quantifier.None), (expression as SingleAtomExpression).Atom);
        }

        [Category("RegexAssistant")]
        [Test]
        public void ParseUnescapedSpecialCharAsAtomFails()
        {
            foreach (var paren in "()[]{}*?+".ToCharArray().Select(c => "" + c))
            {
                var hack = paren;
                var expression = VBRegexParser.Parse(hack);
                Assert.IsAssignableFrom(typeof(ErrorExpression), expression);
            }
        }

        [Category("RegexAssistant")]
        [Test]
        public void ParseSimpleLiteralConcatenationAsConcatenatedExpression()
        {
            var expected = new List<IRegularExpression>
            {
                new SingleAtomExpression(new Literal("a", Quantifier.None)),
                new SingleAtomExpression(new Literal("b", Quantifier.None))
            };

            var expression = VBRegexParser.Parse("ab");
            Assert.IsInstanceOf(typeof(ConcatenatedExpression), expression);
            var subexpressions = (expression as ConcatenatedExpression).Subexpressions;
            Assert.AreEqual(expected.Count, subexpressions.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], subexpressions[i]);
            }
        }


        [Category("RegexAssistant")]
        [Test]
        public void ParseGroupConcatenationAsConcatenatedExpression()
        {
            var expected = new List<IRegularExpression>
            {
                new SingleAtomExpression(new Group(new SingleAtomExpression(new Literal("a", Quantifier.None)), "(a)", Quantifier.None)),
                new SingleAtomExpression(new Group(new SingleAtomExpression(new Literal("b", Quantifier.None)), "(b)", Quantifier.None))
            };
            var expression = VBRegexParser.Parse("(a)(b)");
            Assert.IsInstanceOf(typeof(ConcatenatedExpression), expression);
            var subexpressions = (expression as ConcatenatedExpression).Subexpressions;
            Assert.AreEqual(expected.Count, subexpressions.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], subexpressions[i]);
            }
        }

        [Category("RegexAssistant")]
        [Test]
        public void ParseNestedConcatenatedGroupsCorrectly()
        {
            var expected = new List<IRegularExpression>
            {
                new SingleAtomExpression(new Literal("a", Quantifier.None)),
                new SingleAtomExpression(new Group(new ConcatenatedExpression(new IRegularExpression[]{
                    new SingleAtomExpression(new Literal("f", Quantifier.None)),
                    new SingleAtomExpression(new Literal("o", Quantifier.None)),
                    new SingleAtomExpression(new Literal("o", Quantifier.None)),
                }.ToList()), "(foo)", Quantifier.None)),
                new SingleAtomExpression(new Group(new ConcatenatedExpression(new IRegularExpression[]{
                    new SingleAtomExpression(new Literal("b", Quantifier.None)),
                    new SingleAtomExpression(new Literal("a", Quantifier.None)),
                    new SingleAtomExpression(new Literal("r", Quantifier.None)),
                }.ToList()), "(bar)", Quantifier.None)),
                new SingleAtomExpression(new Literal("b", Quantifier.None))
            };
            var expression = VBRegexParser.Parse("(a(foo)(bar)b)");
            Assert.IsInstanceOf(typeof(SingleAtomExpression), expression);
            Assert.IsInstanceOf(typeof(Group), ((SingleAtomExpression)expression).Atom);
            var containedGroup = (Group)((SingleAtomExpression)expression).Atom;
            var subexpressions = containedGroup.Subexpression.Subexpressions;
            Assert.AreEqual(expected.Count, subexpressions.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], subexpressions[i]);
            }
        }

        [Category("RegexAssistant")]
        [Test]
        public void ParseSimplisticGroupConcatenationAsConcatenatedExpression()
        {
            var expected = new List<IRegularExpression>
            {
                new SingleAtomExpression(new Literal("a", Quantifier.None)),
                new SingleAtomExpression(new Group(new ConcatenatedExpression(new IRegularExpression[]{
                    new SingleAtomExpression(new Literal("a", Quantifier.None)),
                    new SingleAtomExpression(new Literal("b", Quantifier.None)),
                    new SingleAtomExpression(new Literal("c", Quantifier.None)),
                }.ToList()),"(abc)", new Quantifier("{1,4}"))),
                new SingleAtomExpression(new Literal("b", Quantifier.None))
            };

            var expression = VBRegexParser.Parse("a(abc){1,4}b");
            Assert.IsInstanceOf(typeof(ConcatenatedExpression), expression);
            var subexpressions = (expression as ConcatenatedExpression).Subexpressions;
            Assert.AreEqual(expected.Count, subexpressions.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], subexpressions[i]);
            }
        }

        [Category("RegexAssistant")]
        [Test]
        public void ParseSimplisticCharacterClassConcatenationAsConcatenatedExpression()
        {
            var expected = new List<IRegularExpression>
            {
                new SingleAtomExpression(new Literal("a", Quantifier.None)),
                new SingleAtomExpression(new CharacterClass("[abc]", new Quantifier("*"))),
                new SingleAtomExpression(new Literal("b", Quantifier.None))
            };

            var expression = VBRegexParser.Parse("a[abc]*b");
            Assert.IsInstanceOf(typeof(ConcatenatedExpression), expression);
            var subexpressions = (expression as ConcatenatedExpression).Subexpressions;
            Assert.AreEqual(expected.Count, subexpressions.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], subexpressions[i]);
            }
        }

        [Category("RegexAssistant")]
        [Test]
        public void ParseSimplisticAlternativesExpression()
        {
            var expected = new List<IRegularExpression>
            {
                new SingleAtomExpression(new Literal("a", Quantifier.None)),
                new SingleAtomExpression(new Literal("b", Quantifier.None))
            };

            var expression = VBRegexParser.Parse("a|b");
            Assert.IsInstanceOf(typeof(AlternativesExpression), expression);
            var subexpressions = (expression as AlternativesExpression).Subexpressions;
            Assert.AreEqual(expected.Count, subexpressions.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], subexpressions[i]);
            }
        }

        [Category("RegexAssistant")]
        [Test]
        public void CharacterClassIsNotAnAlternativesExpression()
        {
            var expression = VBRegexParser.Parse("[a|b]");
            Assert.IsInstanceOf(typeof(SingleAtomExpression), expression);
            Assert.AreEqual(new CharacterClass("[a|b]", Quantifier.None), (expression as SingleAtomExpression).Atom);
        }

        [Category("RegexAssistant")]
        [Test]
        public void GroupIsNotAnAlternativesExpression()
        {
            var expression = VBRegexParser.Parse("(a|b)");
            Assert.IsInstanceOf(typeof(SingleAtomExpression), expression);
            Assert.AreEqual(new Group(new AlternativesExpression(new IRegularExpression[]{
                    new SingleAtomExpression(new Literal("a", Quantifier.None)),
                    new SingleAtomExpression(new Literal("b", Quantifier.None)),
                }.ToList()), "(a|b)", Quantifier.None), (expression as SingleAtomExpression).Atom);
        }
    }
}
