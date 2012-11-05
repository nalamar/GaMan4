using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolLibrary.Attributes
{
    /// <summary>
    /// This optional attribute can be used to display author information in the 
    /// settings pane. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AuthorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorAttribute"/> class.
        /// </summary>
        /// <param name="author">The author.</param>
        /// <param name="email">The email - optional, validated with regex.</param>
        /// <param name="institute">The institute - optional.</param>
        /// <param name="url">The URL - optional, validated with regex.</param>
        public AuthorAttribute(string author, string email, string institute, string url)
        {
            _author = author;
            _email = email;
            _institute = institute;
            _url = url;
        }

        /// <summary>
        /// The author
        /// </summary>
        public readonly string _author;

        /// <summary>
        /// The email of the author
        /// </summary>
        public readonly string _email;

        /// <summary>
        /// The institute the author works for
        /// </summary>
        public readonly string _institute;

        /// <summary>
        /// The url of the institute or the author
        /// </summary>
        public readonly string _url;
    }
}