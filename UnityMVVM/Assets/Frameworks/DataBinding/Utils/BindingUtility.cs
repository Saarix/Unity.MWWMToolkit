using System;
using System.Collections;

namespace MVVMToolkit.DataBinding
{
    public static class BindingUtility
    {
        public const char PATH_SEPARATOR_CHAR = '.';
        public const char PATH_INDEX_START_CHAR = '[';
        public const char PATH_INDEX_END_CHAR = ']';

        public static string GetPropertyName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path is null");
            }

            string result = null;

            if (path.IndexOf(PATH_SEPARATOR_CHAR) == -1)
            {
                // Use source path as property name
                result = path;
            }
            else if (path.Length == 1 && path[0] == PATH_SEPARATOR_CHAR)
            {
                // Bind to source (Self)
                result = path;
            }
            else
            {
                int startIndex = path.LastIndexOf(PATH_SEPARATOR_CHAR) + 1;
                if (startIndex >= (path.Length - 1))
                {
                    throw new ArgumentException("Invalid path " + path);
                }

                // Get the last property name
                result = path.Substring(startIndex);
            }

            return result;
        }

        public static object GetBindingObject(object obj, string path, IBinder target)
        {
            if (obj == null)
                throw new ArgumentNullException("[BindingUtility.GetBindingObject] Argument obj is null");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("[BindingUtility.GetBindingObject] Argument path is null");

            // Check if path is nested path or if looking for self
            if (path.IndexOf(PATH_SEPARATOR_CHAR) == -1 || (path.Length == 1 && path[0] == PATH_SEPARATOR_CHAR))
                return obj;

            // Get nested object
            string[] names = path.Split(PATH_SEPARATOR_CHAR);

            Type type = obj.GetType();
            object nestedObject = obj;

            for (int i = 0; i < names.Length - 1; i++)
            {
                string propertyName = names[i];

                // Check if we are looking for an collection item
                if (propertyName.Contains(PATH_INDEX_START_CHAR))
                {
                    (Type type, object value) result = GetCollectionItem(propertyName, type, nestedObject);
                    nestedObject = result.value;
                    type = result.type;
                }
                else
                {
                    System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);

                    if (propertyInfo == null)
                        throw new Exception(string.Format("[BindingUtility.GetBindingObject] Failed to find property {0} in {1}", path, obj));

                    // Get value and type
                    nestedObject = propertyInfo.GetValue(nestedObject, null);
                    type = propertyInfo.PropertyType;
                }
            }

            // Return the binding object
            return nestedObject;
        }

        public static object GetPropertyValue(object obj, string propertyName, bool allowMissing = false)
        {
            if (obj == null)
                throw new ArgumentNullException("[BindingUtility.GetPropertyValue] Argument obj is null");

            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("[BindingUtility.GetPropertyValue] Argument propertyName is null");

            // When binding to Self return context
            if (propertyName.Length == 1 && propertyName[0] == PATH_SEPARATOR_CHAR)
                return obj;

            // When binding to an Item return item as an context
            if (propertyName.Contains(PATH_INDEX_START_CHAR) && propertyName.EndsWith(PATH_INDEX_END_CHAR))
            {
                (Type type, object value) result = GetCollectionItem(propertyName, obj.GetType(), obj);
                return result.value;
            }

            System.Reflection.PropertyInfo sourcePropertyInfo = obj.GetType().GetProperty(propertyName);
            if (sourcePropertyInfo == null)
            {
                if (!allowMissing)
                    UnityEngine.Debug.LogError(string.Format("[BindingUtility.GetPropertyValue] Failed to find property {0} in {1}", propertyName, obj.GetType()));

                return null;
            }

            object value = sourcePropertyInfo.GetValue(obj, null);
            return value;
        }

        public static (Type type, object value) GetCollectionItem(string propertyName, Type currentScopeType, object currentScope)
        {
            // Isolate collection property name and index of an item
            int startBracketIndex = propertyName.IndexOf(PATH_INDEX_START_CHAR);
            string collectionPath = propertyName.Substring(0, startBracketIndex);
            int itemIndex = int.Parse(propertyName.Substring(startBracketIndex + 1, propertyName.IndexOf(PATH_INDEX_END_CHAR) - startBracketIndex - 1));

            System.Reflection.PropertyInfo collectionProperty = currentScopeType.GetProperty(collectionPath);
            // Check if property that was found is a collection
            if (typeof(ICollection).IsAssignableFrom(collectionProperty.PropertyType) && collectionProperty.PropertyType != typeof(string))
            {
                // Get the value of the collection property from the object
                object value = collectionProperty.GetValue(currentScope, null);

                if (value is IEnumerable enumerable)
                {
                    int index = 0;
                    bool found = false;
                    foreach (object item in enumerable)
                    {
                        if (index == itemIndex)
                        {
                            found = true;
                            return (item.GetType(), item);
                        }

                        index++;
                    }

                    if (!found)
                        throw new Exception($"[BindingUtility.GetCollectionItem] Unable to find item on index: {itemIndex}");
                }
                else
                {
                    throw new Exception($"[BindingUtility.GetCollectionItem] The property is not a valid collection or is null. Type={value.GetType()}");
                }
            }
            else
            {
                throw new Exception($"[BindingUtility.GetCollectionItem] The property is not a valid collection or is null. Type={collectionProperty.PropertyType}");
            }

            return (null, null);
        }
    }
}
