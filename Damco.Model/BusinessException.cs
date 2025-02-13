
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Damco.Model
{
    /// <summary>
    /// Handles all the expected exceptions in the application.
    /// </summary>
    /// <remarks>
    /// To be used for validation failures etc. 
    /// The framework guarantees exceptions of this type are displayed to users in a friendly manner.
    /// The exceptions don't use a text as the message but use a TextPlaceHolder, so the exception can be 
    /// translated to a message in the user's own language.
    /// </remarks>
    //[DataContract(Namespace="http://damco.com/")]
    //[Serializable()]
    //[XmlType(Namespace="http://damco.com/")]
    [KnownType(typeof(TextPlaceHolder))]
    public class BusinessException : ApplicationException, ISerializable
    {
        /// <summary>
        /// Initializes the new instance of BusinessException.
        /// </summary>
        public BusinessException()
        {
        }

        protected BusinessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes the new instance of the BusinessException with the specified message of type TextPlaceHolder.
        /// </summary>
        /// <param name="messagePlaceHolder">The message of type TextPlaceHolder is passed for initialization.</param>
        public BusinessException(TextPlaceHolder messagePlaceHolder, bool canBeOverlooked = false, bool overlookRequiresAdministrator = false)
            : base(messagePlaceHolder.FixedText != null ? messagePlaceHolder.FixedText : string.Join(".", messagePlaceHolder.TextPath.ToArray()))
        {
            this.CanBeOverlooked = canBeOverlooked;
            this.OverlookRequiresAdministrator = overlookRequiresAdministrator;
            this.MessagePlaceHolder = messagePlaceHolder;
        }

        /// <summary>
        /// Gets or sets MessagePlaceHolder of type TextPlaceHolder.
        /// </summary>
        public virtual TextPlaceHolder MessagePlaceHolder { get; set; }

        public virtual bool CanBeOverlooked { get; set; }
        public virtual bool OverlookRequiresAdministrator { get; set; }

        public virtual bool ShouldPreignoreMessage => false;

        /// <summary>
        /// Method to Add value of MessageplaceHolder to SerializationInfo and to 
        /// serialize the value to the type of TextPlaceHolder.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Use the AddValue method to specify serialized values.
            info.AddValue(nameof(MessagePlaceHolder), this.MessagePlaceHolder, typeof(TextPlaceHolder));
            info.AddValue(nameof(CanBeOverlooked), this.CanBeOverlooked, typeof(bool));
            info.AddValue(nameof(OverlookRequiresAdministrator), this.OverlookRequiresAdministrator, typeof(bool));
        }

        /// <summary>
        /// Initializes a new instance of the BusinessException class with serialized data.
        /// </summary>
        /// <param name="info"> The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        public BusinessException(SerializationInfo info, StreamingContext context)
            : this(
                (TextPlaceHolder)info.GetValue(nameof(MessagePlaceHolder), typeof(TextPlaceHolder)),
                (bool)info.GetValue(nameof(CanBeOverlooked), typeof(bool)),
                (bool)info.GetValue(nameof(OverlookRequiresAdministrator), typeof(bool))
            )
        {
        }
    }

    public class AggregateBusinessException : BusinessException
    {
        private static IEnumerable<BusinessException> FlattenChildrenTree(IEnumerable<BusinessException> children)
        {
            foreach (var ex in children)
                if (ex is AggregateBusinessException)
                    foreach (var ex2 in FlattenChildrenTree(((AggregateBusinessException)ex).Children))
                        yield return ex2;
                else
                    yield return ex;
        }

        public List<BusinessException> Children { get; } = new List<Model.BusinessException>();

        public AggregateBusinessException(params BusinessException[] children)
            : this((IEnumerable<BusinessException>)children)
        {
        }

        public AggregateBusinessException(IEnumerable<BusinessException> children)
            : base(string.Join("\n", FlattenChildrenTree(children).Select(c => c.Message).ToArray()))
        {
            this.Children.AddRange(FlattenChildrenTree(children));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Use the AddValue method to specify serialized values.
            info.AddValue("Children", this.Children, typeof(List<BusinessException>));
        }

        public override TextPlaceHolder MessagePlaceHolder
        {
            get
            {
                //TODO: Real place holder
                return (TextPlaceHolder)this.Message;
            }
            set
            {

            }
        }

        public override bool CanBeOverlooked
        {
            get { return this.Children.All(x => x.CanBeOverlooked); }
            set { }
        }

        public override bool OverlookRequiresAdministrator
        {
            get { return this.Children.Any(x => x.OverlookRequiresAdministrator); }
            set { }
        }

        /// <summary>
        /// Initializes a new instance of the BusinessException class with serialized data.
        /// </summary>
        /// <param name="info"> The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        public AggregateBusinessException(SerializationInfo info, StreamingContext context)
            : this(
                (List<BusinessException>)info.GetValue("Children", typeof(List<BusinessException>))
            )
        {
        }
    }

    /// <summary>
    /// <see cref="BusinessException"/> that should preignore original inbound message.
    /// </summary>
    public class PreignoringBusinessException : BusinessException
    {
        /// <summary>
        /// Initializes the new instance of the PreignoringBusinessException with the specified message of type TextPlaceHolder.
        /// </summary>
        /// <param name="messagePlaceHolder">The message of type TextPlaceHolder is passed for initialization.</param>
        public PreignoringBusinessException(TextPlaceHolder messagePlaceHolder, bool canBeOverlooked = false, bool overlookRequiresAdministrator = false)
            : base(messagePlaceHolder, canBeOverlooked, overlookRequiresAdministrator)
        {
        }
        public override bool ShouldPreignoreMessage => true;
    }

}
