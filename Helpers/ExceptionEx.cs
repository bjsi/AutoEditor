using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoEditor.Helpers
{
    /// <summary>
    /// Extensions for <see cref="Exception"/>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "<Pending>")]
    public static class ExceptionEx
    {
        #region Methods

        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if <paramref name="obj"/> is null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="errMsg"></param>
        public static void ThrowIfArgumentNull(this object obj, string errMsg)
        {
            if (obj == null)
                throw new ArgumentNullException(errMsg);
        }

        /// <summary>
        /// Throws a <see cref="NullReferenceException"/> if <paramref name="obj"/> is null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="errMsg"></param>
        public static void ThrowIfNull(this object obj, string errMsg)
        {
            if (obj == null)
                throw new NullReferenceException(errMsg);
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/> if <paramref name="str"/> is null or white space (blank, tab, line, ..)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="errMsg"></param>
        public static void ThrowIfNullOrWhitespace(this string str, string errMsg)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new InvalidOperationException(errMsg);
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/> if <paramref name="obj"/> is not null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="errMsg"></param>
        public static void ThrowIfNotNull(this object obj, string errMsg)
        {
            if (obj != null)
                throw new InvalidOperationException(errMsg);
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/> if <paramref name="dir"/> doesn't exist
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="errMsg"></param>
        public static void ThrowIfMissing(this DirectoryInfo dir, string errMsg)
        {
            if (dir.Exists == false)
                throw new InvalidOperationException(errMsg);
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/> if <paramref name="file"/> doesn't exist
        /// </summary>
        /// <param name="file"></param>
        /// <param name="errMsg"></param>
        public static void ThrowIfMissing(this FileInfo file, string errMsg)
        {
            if (file.Exists == false)
                throw new InvalidOperationException(errMsg);
        }

        #endregion
    }
}
