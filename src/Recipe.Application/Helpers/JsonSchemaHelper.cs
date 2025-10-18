using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace Recipe.Application.Helpers
{
    public static class JsonSchemaHelper
    {
        public static string GetJsonTemplate<T>()
        {
            var type = typeof(T);
            var obj = Activator.CreateInstance(type);

            // Set default values for lists/collections
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.PropertyType.IsInterface &&
                typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    // Create List<T> dynamically
                    var genericType = prop.PropertyType.GetGenericArguments().FirstOrDefault() ?? typeof(object);
                    var listType = typeof(List<>).MakeGenericType(genericType);
                    var listInstance = Activator.CreateInstance(listType);
                    prop.SetValue(obj, listInstance);
                }
            }

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(obj, jsonOptions);
        }
    }
}