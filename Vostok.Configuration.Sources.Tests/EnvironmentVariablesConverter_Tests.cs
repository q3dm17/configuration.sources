using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.Environment;

namespace Vostok.Configuration.Sources.Tests
{
    [TestFixture]
    internal class EnvironmentVariablesConverter_Tests
    {
        [Test]
        public void Should_return_null_when_no_variables()
        {
            var vars = CreateVariablesDictionary(new Dictionary<string, string>());
            EnvironmentVariablesConverter.Convert(vars).Should().BeNull();
        }

        [Test]
        public void Should_parse_variables_to_single_objectNode_when_simple_keys()
        {
            var vars = CreateVariablesDictionary(
                new Dictionary<string, string>
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2"
                });
            EnvironmentVariablesConverter.Convert(vars)
                .Should()
                .Be(
                    new ObjectNode(
                        new[]
                        {
                            new ValueNode("key1", "value1"),
                            new ValueNode("key2", "value2")
                        }));
        }

        [TestCase(".")]
        [TestCase(":")]
        [TestCase("__")]
        public void Should_parse_variables_to_tree_when_multilevel_keys(string separator)
        {
            var vars = CreateVariablesDictionary(
                new Dictionary<string, string>
                {
                    [$"a{separator}b"] = "value1",
                    [$"a{separator}c"] = "value2"
                });
            EnvironmentVariablesConverter.Convert(vars)
                .Should()
                .Be(
                    new ObjectNode(
                        new[]
                        {
                            new ObjectNode(
                                "a",
                                new[]
                                {
                                    new ValueNode("b", "value1"),
                                    new ValueNode("c", "value2")
                                })
                        }));
        }

        [Test]
        public void Should_ignore_keys_case()
        {
            var vars = CreateVariablesDictionary(
                new Dictionary<string, string>
                {
                    ["PATH"] = "value"
                });
            EnvironmentVariablesConverter.Convert(vars)["path"].Should().Be(new ValueNode("PATH", "value"));
        }

        private static IDictionary CreateVariablesDictionary(Dictionary<string, string> variables)
        {
            var hashtable = new Hashtable();
            foreach (var keyValuePair in variables)
            {
                hashtable.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return hashtable;
        }
    }
}