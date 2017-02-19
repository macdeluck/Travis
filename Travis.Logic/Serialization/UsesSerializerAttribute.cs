using System;

namespace Travis.Logic.Serialization
{
    /// <summary>
    /// Annotates that class may be serialized using specified serializer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UsesSerializerAttribute : Attribute
    {
        /// <summary>
        /// Type of serializer.
        /// </summary>
        public Type SerializerType { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="UsesSerializerAttribute"/>.
        /// </summary>
        /// <param name="serializerType">Type of serializer used to serialize game.</param>
        public UsesSerializerAttribute(Type serializerType)
        {
            SerializerType = serializerType;
        }
    }
}
