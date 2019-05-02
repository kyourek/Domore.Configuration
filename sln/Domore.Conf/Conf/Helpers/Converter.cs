﻿using System;
using System.ComponentModel;

namespace Domore.Conf.Helpers {
    class Converter {
        public static object Convert(Type type, string value, TypeConverter typeConverter = null) {
            typeConverter = typeConverter ?? TypeDescriptor.GetConverter(type);
            try {
                return typeConverter.ConvertFrom(value);
            }
            catch {
                if (type == typeof(Type)) {
                    return Type.GetType(value, throwOnError: true, ignoreCase: true);
                }
                var valueType = Type.GetType(value, throwOnError: false, ignoreCase: true);
                if (valueType != null) {
                    return Activator.CreateInstance(valueType);
                }
                throw;
            }
        }

        public static object Convert(Type type, IConfBlock block, string key) {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == block) throw new ArgumentNullException(nameof(block));

            var conv = TypeDescriptor.GetConverter(type);
            if (conv is ConfTypeConverter conf) {
                conf.Conf = block;
            }

            if (block.ItemExists(key, out var item)) {
                return Convert(type, item.OriginalValue, conv);
            }

            try {
                return conv.ConvertFrom(key);
            }
            catch {
                var constructor = type.GetConstructor(new Type[] { });
                if (constructor != null) {
                    return constructor.Invoke(new object[] { });
                }
                throw;
            }
        }
    }
}