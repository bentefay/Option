﻿using Functional.Option;
using NUnit.Framework;
using System;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class OptionTests
    {
        [Test]
        public void TestCreate()
        {
            var s = "string";
            Option<string> option = s;

            Assert.IsTrue(option.HasValue);
            Assert.AreEqual(s, option.Value);

            Option<object> objectOption = (object)null;

            Assert.IsFalse(objectOption.HasValue);
            Assert.IsTrue(Option.None == objectOption);

            Assert.Throws<ArgumentNullException>(() => Option.Some<object>(null));

            Option<int> intoption = ((int?)null).ToOption();
            intoption = Option.Create((int?)null);
            intoption = Option.Create<int>(null);
            intoption = Option.Create((int?)1);
            intoption = Option.Some((int?)1);
            intoption = Option.Some(1);
            intoption = 0.ToOption();

            Assert.Throws<ArgumentNullException>(() => Option.Some<int>(null));
        }

        [Test]
        public void TestValue()
        {
            Option<object> objectOption = Option.None;
            Assert.Throws(typeof(InvalidOperationException), () => { var v = objectOption.Value; });

            object o = new object();
            objectOption = o;
            object o2 = objectOption;
            Assert.AreEqual(o2, objectOption);
        }

        [Test]
        public void TestTryGetValue()
        {
            Option<object> objectOption = Option.None;
            Assert.IsFalse(objectOption.TryGetValue(out _));

            objectOption = new object();

            Assert.IsTrue(objectOption.TryGetValue(out _));
        }

        [Test]
        public void TestValueOrDefault()
        {
            Option<object> objectOption = Option.None;
            Assert.AreEqual(null, objectOption.ValueOrDefault);

            var o = new object();
            objectOption = o;

            Assert.AreEqual(o, objectOption.Value);

            var optionEqual = objectOption;
            var optionNotEqual = new object();
            Assert.IsTrue(objectOption.Equals(optionEqual));
            Assert.IsFalse(objectOption.Equals(optionNotEqual));
            Assert.IsFalse(objectOption.Equals(Option<object>.None));
        }

        [Test]
        public void TestValueOr()
        {
            Option<object> objectOption = Option.None;
            Assert.AreEqual(1, objectOption.ValueOr(1));
            Assert.AreEqual(1, objectOption.ValueOr(() => 1));

            var o = new object();
            objectOption = o;

            Assert.AreEqual(o, objectOption.ValueOr(1));
            Assert.AreEqual(o, objectOption.ValueOr(() => 1));
        }

        [Test]
        public void TestOptionMatchT()
        {
            Option<int> optionNone = Option.None;
            Option<int> optionSome = 10;

            var matchedNone = false;
            var matchedSome = false;

            void ResetMatchVars()
            {
                matchedNone = false;
                matchedSome = false;
            }

            void NoneAction() => matchedNone = true;
            void SomeAction(int x) => matchedSome = true;

            optionSome.Match(
                None: NoneAction,
                Some: SomeAction);
            Assert.IsFalse(matchedNone);
            Assert.IsTrue(matchedSome);

            ResetMatchVars();

            optionNone.Match(
                None: NoneAction,
                Some: SomeAction);
            Assert.IsTrue(matchedNone);
            Assert.IsFalse(matchedSome);
        }

        [Test]
        public void TestOptionMatchTInTOut()
        {
            Option<int> optionNone = Option.None;
            Option<int> optionSome = 10;

            Assert.AreEqual(0, optionNone.Match(
                None: () => 0,
                Some: x => 1));
            Assert.AreEqual(1, optionSome.Match(
                None: () => 0,
                Some: x => 1));
        }

        [Test]
        public void TestOptionPatternMatcherT()
        {
            Option<int> optionNone = Option.None;
            Option<int> optionSome = 10;
            Option<int> optionOne = 1;

            var matchedNone = false;
            var matchedSome = false;
            var matchedOne = false;

            void ResetMatchVars()
            {
                matchedNone = false;
                matchedSome = false;
                matchedOne = false;
            }

            var staticMatcher = Option<int>.PatternMatch()
                .None(() => matchedNone = true)
                .Some(1, () => matchedOne = true)
                .Some((x) => matchedSome = true);

            var matcher = optionSome.Match()
                .None(() => matchedNone = true)
                .Some(1, () => matchedOne = true)
                .Some((x) => matchedSome = true);

            staticMatcher.Result();
            
            Assert.IsTrue(matchedNone);
            Assert.IsFalse(matchedSome);
            Assert.IsFalse(matchedOne);

            ResetMatchVars();

            staticMatcher.Result(null);

            Assert.IsTrue(matchedNone);
            Assert.IsFalse(matchedSome);
            Assert.IsFalse(matchedOne);

            ResetMatchVars();

            matcher.Result();
            Assert.IsFalse(matchedNone);
            Assert.IsTrue(matchedSome);
            Assert.IsFalse(matchedOne);

            ResetMatchVars();

            matcher.Result(optionNone);
            Assert.IsTrue(matchedNone);
            Assert.IsFalse(matchedSome);
            Assert.IsFalse(matchedOne);

            ResetMatchVars();

            matcher.Result(optionSome);
            Assert.IsFalse(matchedNone);
            Assert.IsTrue(matchedSome);
            Assert.IsFalse(matchedOne);

            ResetMatchVars();

            matcher.Result(optionOne);
            Assert.IsFalse(matchedNone);
            Assert.IsFalse(matchedSome);
            Assert.IsTrue(matchedOne);

            Assert.Throws(typeof(InvalidOperationException), () => matcher.None(() => { }));
            Assert.Throws(typeof(InvalidOperationException), () => matcher.Some((x) => { }));
            Assert.Throws(typeof(InvalidOperationException), () => matcher.Some(1, () => { }));
        }

        [Test]
        public void TestOptionPatternMatcherTInTOut()
        {
            Option<int> optionNone = Option.None;
            Option<int> optionSome = 10;
            Option<int> optionOne = 1;

            var staticMatcher = Option<int>.PatternMatch<int>();

            var matcher =  optionSome.Match<int>()
                .None(() => 0)
                .Some(x => 1)
                .Some(1, () => 2);

            Assert.AreEqual(default(int), staticMatcher.Result());
            Assert.AreEqual(default(int), staticMatcher.Result(null));
            Assert.AreEqual(1, matcher.Result());
            Assert.AreEqual(0, matcher.Result(optionNone));
            Assert.AreEqual(1, matcher.Result(optionSome));
            Assert.AreEqual(2, matcher.Result(optionOne));

            Assert.Throws(typeof(InvalidOperationException), () => matcher.None(() => 0));
            Assert.Throws(typeof(InvalidOperationException), () => matcher.Some(x => 1));
            Assert.Throws(typeof(InvalidOperationException), () => matcher.Some(1, () => 2));
        }

        [Test]
        public void TestImplicitOperators()
        {
            Option<int?> o1 = 1;           // Some<int?>(1)
            Option<int?> o2 = (int?)null;  // None<int?>

            Assert.AreEqual(true, o1.HasValue);
            Assert.AreEqual(false, o2.HasValue);

            int ran = 0;
            Option<int> o = 1;
            var matcher = o.Match()
                | ((i) => { ran++; })
                | (() => { ran++; });

            Assert.AreEqual(1, ran);

            Func<string> m = () => o.Match<string>()
                //| (1, () => "One") // Not possible yet
                | ((i) => i.ToString())
                | (() => "None");

            Assert.AreEqual("1", m());

            Option<int> a = ((int?)1).ToOption();

        }

        [Test]
        public void TestEquals()
        {
            Option<int> o = 1;
            Option<int> oSame = 1;
            Option<bool> oDifferent = false;

            Assert.IsTrue(o.Equals((object)oSame));
            Assert.IsFalse(o.Equals(oDifferent));
            Assert.IsFalse(o.Equals(null));
        }

        [Test]
        public void TestEqualsOperators()
        {
            Option<int> o = 1;
            Option<int> oSame = 1;
            Option<int> oDifferent = 2;

            Option<int> oNull = null;

            Assert.IsTrue(o == oSame);
            Assert.IsFalse(o == oDifferent);
            Assert.IsTrue(o != oDifferent);
            Assert.IsTrue(oNull != o);
            Assert.IsTrue(null != o);
            Assert.IsTrue(oNull == null);
            Assert.IsTrue(null == oNull);
            Assert.IsFalse(oNull != null);
            Assert.IsFalse(null != oNull);
        }

        [Test]
        public void TestHashCode()
        {
            int i = 1;
            Option<int> o1 = i;
            Option<int> o2 = Option<int>.None;

            Assert.AreEqual(i.GetHashCode(), o1.GetHashCode());
            Assert.AreEqual(0, o2.GetHashCode());
        }

        [Test]
        public void TestSome()
        {
            Option<int> s = 1;

            Assert.AreEqual(1, s.Value);
        }

        [Test]
        public void TestIEnumerable()
        {
            Option<int> o = 1;

            TestIEnumerableHelper(o);
            TestIEnumerableHelper(Option.None);

            var now = DateTime.UtcNow;
            Option<DateTime> dto = now.AddHours(1);

            Option<int>[] oarray = { 1 };

            Assert.AreEqual(1, oarray.Flatten().Count());

            oarray = new Option<int>[] { Option.None };

            Assert.AreEqual(0, oarray.Flatten().Count());

            var aggr = dto
                .Aggregate(TimeSpan.Zero, (ts, dt) => dt - now + ts);
            Assert.AreEqual(TimeSpan.FromHours(1), aggr);
        }

        private static void TestIEnumerableHelper(Option<int> o)
        {
            var select = o.Select(x => x);
            Assert.AreEqual(o.HasValue ? 1 : 0, select.Count());

            select = o.Select((x, index) => x);
            Assert.AreEqual(o.HasValue ? 1 : 0, select.Count());

            var where = o.Where(x => true);
            Assert.AreEqual(o.HasValue ? 1 : 0, where.Count());

            where = o.Where((x, index) => true);
            Assert.AreEqual(o.HasValue ? 1 : 0, where.Count());

            var toArr = o.ToArray();
            Assert.AreEqual(o.HasValue ? 1 : 0, toArr.Length);


            if (o.HasValue)
            {
                var aggregate = o.Aggregate((acc, val) => acc + val);
                Assert.AreEqual(o.ValueOrDefault, aggregate);

                aggregate = o.Aggregate(0, (acc, val) => acc + val);
                Assert.AreEqual(o.ValueOrDefault, aggregate);
            }

            bool run = false;
            foreach (var obj in ((System.Collections.IEnumerable)o))
            {
                run = true;
                Assert.AreEqual(1, (int)obj);
            }

            Assert.AreEqual(run, o.HasValue);

            run = false;
            o.ForEach((val) => run = true);
            Assert.AreEqual(run, o.HasValue);
        }
    }
}
