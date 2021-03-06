// Copyright © 2010-2017 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;

namespace CefSharp
{
    /// <summary>
    /// Event arguments for the ConsoleMessage event handler set up in IWebBrowser.
    /// </summary>
    public class ConsoleMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new ConsoleMessageEventArgs event argument.
        /// </summary>
        /// <param name="level">level</param>
        /// <param name="message">message</param>
        /// <param name="source">source</param>
        /// <param name="line">line number</param>
        public ConsoleMessageEventArgs(LogSeverity level, string message, string source, int line)
        {
            Level = level;
            Message = message;
            Source = source;
            Line = line;
        }

        /// <summary>
        /// Log level
        /// </summary>
        public LogSeverity Level { get; private set; }

        /// <summary>
        /// The message text of the console message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The source of the console message.
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// The line number that generated the console message.
        /// </summary>
        public int Line { get; private set; }
    }
}
