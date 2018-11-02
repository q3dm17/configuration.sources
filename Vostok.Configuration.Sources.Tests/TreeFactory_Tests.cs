using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.SettingsTree;

namespace Vostok.Configuration.Sources.Tests
{
    [TestFixture]
    internal class TreeFactory_Tests
    {
        [Test]
        public void Should_return_rootNode_when_value_is_rootNode()
        {
            var valueNode = new ValueNode("name", "value");
            TreeFactory.CreateTreeByMultiLevelKey("root", new string[0], valueNode)
                .Should().Be(valueNode);
        }

        [Test]
        public void Should_create_correct_tree_when_single_key()
        {
            var valueNode = new ValueNode("key", "value");
            TreeFactory.CreateTreeByMultiLevelKey("root", new []{"key"}, valueNode)
                .Should().Be(new ObjectNode("root", new Dictionary<string, ISettingsNode>
                {
                    ["key"] = valueNode
                }));
        }

        [Test]
        public void Should_create_correct_tree_when_multiLevel_key()
        {
            var valueNode = new ValueNode("key2", "value");
            TreeFactory.CreateTreeByMultiLevelKey("root", new []{"key1", "key2"}, valueNode)
                .Should().Be(new ObjectNode("root", new Dictionary<string, ISettingsNode>
                {
                    ["key1"] = new ObjectNode("key1", new Dictionary<string, ISettingsNode>
                    {
                        ["key2"] = valueNode
                    })
                }));
        }
    }
}