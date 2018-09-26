/*
 * Copyright (c) 2018 Filip Jansson
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Extend.Serializers
{
    public class IntegerFieldSerializer<T> : ISerializer where T : Fields.SimpleField<int?>
    {
        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The serialized value</returns>
        public string Serialize(object obj)
        {
            if (obj is T)
            {
                if (((T)obj).Value.HasValue)
                    return ((T)obj).Value.Value.ToString();
                return null;
            }
            throw new ArgumentException("The given object doesn't match the serialization type");
        }

        /// <summary>
        /// Deserializes the given string.
        /// </summary>
        /// <param name="str">The serialized value</param>
        /// <returns>The object</returns>
        public object Deserialize(string str)
        {
            var field = Activator.CreateInstance<T>();

            if (!string.IsNullOrWhiteSpace(str))
            {
                try
                {
                    field.Value = int.Parse(str);
                }
                catch { }
            }
            return field;
        }
    }
}
