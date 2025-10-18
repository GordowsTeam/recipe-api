using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipe.Application.Helpers
{
    public static class DeserializerHelper
    {
        public static T? DeserializeSafe<T>(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return default;
            // Trim whitespace
            input = input.Trim();
            // Remove enclosing quotes if present
            if ((input.StartsWith("\"") && input.EndsWith("\"")) ||
                (input.StartsWith("'") && input.EndsWith("'")))
            {
                input = input[1..^1].Trim();
            }
            // Remove markdown code block if present
            if (input.StartsWith("```") && input.EndsWith("```"))
            {
                var lines = input.Split('\n');
                if (lines.Length > 2)
                {
                    // Remove the first and last lines (the ``` markers)
                    input = string.Join('\n', lines[1..^1]).Trim();
                }
            }
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(input);
            }
            catch
            {
                // Log deserialization error if needed
                return default;
            }
        }
    }
}
