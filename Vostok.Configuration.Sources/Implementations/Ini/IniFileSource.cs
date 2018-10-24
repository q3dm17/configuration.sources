﻿using System;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.Implementations.File;

namespace Vostok.Configuration.Sources.Implementations.Ini
{
    /// <inheritdoc />
    /// <summary>
    /// Ini converter to <see cref="ISettingsNode"/> tree from file
    /// </summary>
    public class IniFileSource : BaseFileSource
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="T:Vostok.Configuration.Sources.Implementations.Ini.IniFileSource" /> instance using given parameter <paramref name="filePath" />
        /// </summary>
        /// <param name="filePath">File name with settings</param>
        /// <param name="settings">File parsing settings</param>
        public IniFileSource([NotNull] string filePath, FileSourceSettings settings = null)
            : base(filePath, settings, ParseSettings)
        {
        }

        internal IniFileSource([NotNull] string filePath, Func<string, FileSourceSettings, IObservable<(string, Exception)>> fileWatcherCreator, FileSourceSettings settings = null)
            : base(filePath, settings, ParseSettings, fileWatcherCreator)
        {
        }

        private static ISettingsNode ParseSettings(string str) => new IniStringSource(str).Get();
    }
}