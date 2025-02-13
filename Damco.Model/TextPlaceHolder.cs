using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{
    /// <summary>
    /// Represents a specific text that can be retrieved via a text provider.
    /// </summary>
    /// <remarks>
    /// Text to be displayed on UI is retrieved from database via a text provider.
    /// </remarks>
    [Serializable()]
    [KnownType(typeof(object[]))]
    [KnownType(typeof(string[]))]
    public class TextPlaceHolder
    {
        public TextPlaceHolder()
        {
        }

        /// <summary>
        /// Initializes the new instance of TextPlaceHolder with given Text.
        /// </summary>
        /// <param name="fixedText">Text to be displayed via TextPlaceHolder.</param>
        public TextPlaceHolder(string fixedText)
        {
            this.FixedText = fixedText;
        }

        /// <summary>
        /// Initializes the new instance of TextPlaceHolder at the specified path.
        /// </summary>
        /// <param name="textPath">Path where the text is to be placed on the UI.</param>
        /// <param name="parameters">Parameters to be set to the TextPlaceHolder.</param>
        public TextPlaceHolder(string[] textPath, params object[] parameters)
        {
            this.TextPath = textPath;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Text to be displayed.
        /// </summary>
        public virtual string FixedText { get; set; }

        /// <summary>
        /// Path of the Text on the UI.
        /// </summary>
        public virtual string[] TextPath { get; set; }

        /// <summary>
        /// Parameters of the text.
        /// </summary>
        public virtual object[] Parameters { get; set; }

        /// <summary>
        /// Gets the TextPlaceHolder type converted Text.
        /// </summary>
        /// <param name="fixedText">Text retrieved and displayed in UI.</param>
        public static explicit operator TextPlaceHolder(string fixedText)
        {
            return new TextPlaceHolder(fixedText);
        }

        public override string ToString()
        {
            if (this.FixedText != null)
                return this.FixedText;
            else if (this.TextPath != null)
                return $"{string.Join(".", this.TextPath)}({string.Join(",", (this.Parameters ?? new object[] { }).Select(x => x?.ToString() ?? "").ToArray())}";
            else
                return null;
        }
    }
}
