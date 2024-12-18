﻿using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using YamlDotNet.Core;

namespace XProxy.Serialization
{
    public static class ConsoleYamlParser
    {
        public static ISerializer Serializer { get; } = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .DisableAliases()
            .IgnoreFields()
            .WithDefaultScalarStyle(ScalarStyle.SingleQuoted)
            .Build();

        public static IDeserializer Deserializer { get; } = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .IgnoreFields()
            .Build();
    }
}
