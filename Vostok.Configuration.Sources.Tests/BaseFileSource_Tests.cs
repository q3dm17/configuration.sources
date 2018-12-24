using System;
using System.IO;
using System.Reactive.Subjects;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using Vostok.Commons.Testing;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.File;
using Vostok.Configuration.Sources.Tests.Helpers;

namespace Vostok.Configuration.Sources.Tests
{
    [TestFixture]
    public class BaseFileSource_Tests
    {
        private ReplaySubject<(string, Exception)> subject;
        private ValueNode settings;

        [SetUp]
        public void SetUp()
        {
            subject = new ReplaySubject<(string, Exception)>();
            settings = new ValueNode("value");
        }
        
        [Test]
        public void Should_push_parsed_settings_when_no_errors()
        {
            var source = new BaseFileSource(
                () => subject,
                content => settings);
            
            subject.OnNext(("settings", null));

            source.Observe().WaitFirstValue(100.Milliseconds())
                .Should()
                .Be((settings, null));
        }
        
        [Test]
        public void Should_push_error_from_fileObserver()
        {
            var parseCalls = 0;
            var source = new BaseFileSource(
                () => subject,
                content =>
                {
                    parseCalls++;
                    return settings;
                });
            
            var error = new IOException();
            subject.OnNext(("settings", error));

            source.Observe().WaitFirstValue(100.Milliseconds())
                .Should()
                .Be((null, error));

            parseCalls.Should().Be(0);
        }

        [Test]
        public void Should_push_parsing_error_when_failed_to_parse()
        {
            var error = new IOException();
            var source = new BaseFileSource(
                () => subject,
                content => throw error);
            
            subject.OnNext(("settings", null));

            source.Observe().WaitFirstValue(100.Milliseconds())
                .Should()
                .Be((null, error));
        }

        [Test]
        public void Should_not_parse_same_content_twice([Values]bool parserThrows)
        {
            var parseCalls = 0;
            var source = new BaseFileSource(
                () => subject,
                content =>
                {
                    parseCalls++;
                    if (parserThrows)
                        throw new FormatException();
                    return settings;
                });

            using (source.Observe().Subscribe(_ => {}))
            {
                Action assertion = () => parseCalls.Should().Be(1);
            
                subject.OnNext(("settings", null));
            
                assertion.ShouldPassIn(1.Seconds());
            
                subject.OnNext(("settings", null));
            
                assertion.ShouldNotFailIn(500.Milliseconds());
            }
        }
    }
}