using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Persistence;

namespace Scriptvana.Editor.Services
{
    public static class ScriptConfigurationService
    {
        public static string NormalizeName(string input)
        {
            string sanitizedInput = (input ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(sanitizedInput))
            {
                return sanitizedInput;
            }

            return NormalizeNamePersistence.NormalizationMode switch
            {
                NameNormalizationMode.PascalCase => ToPascalCase(sanitizedInput, false),
                NameNormalizationMode.PascalCaseWithUnderscores => ToPascalCase(sanitizedInput, true),
                NameNormalizationMode.UppercaseFirstLetter => UppercaseFirstLetter(sanitizedInput),
                _ => sanitizedInput
            };
        }

        public static string NormalizeFolderPath(string path)
        {
            string normalizedPath = NormalizeFolderPathInternal(path);

            if (RoutePersistence.RestrictToBasePath)
            {
                string basePath = GetRestrictedBasePath();
                if (!IsPathInsideBasePath(normalizedPath, basePath))
                {
                    normalizedPath = basePath;
                }
            }

            return normalizedPath;
        }

        public static string GetDefaultScriptPath()
        {
            string defaultPath = NormalizeFolderPath(RoutePersistence.DefaultPath);
            string basePath = GetRestrictedBasePath();

            return RoutePersistence.RestrictToBasePath && !IsPathInsideBasePath(defaultPath, basePath)
                ? basePath
                : defaultPath;
        }

        public static string GetRestrictedBasePath()
        {
            return NormalizeFolderPathInternal(RoutePersistence.BasePath);
        }

        public static string ResolveNamespace(string explicitNamespace, string path, string scriptName = "")
        {
            if (!string.IsNullOrWhiteSpace(explicitNamespace))
            {
                return explicitNamespace.Trim();
            }

            return GetDefaultNamespace(path, scriptName);
        }

        public static string GetDefaultNamespace(string path, string scriptName = "")
        {
            if (NamespacePersistence.UseScriptNameAsDefaultNamespace)
            {
                return BuildNamespaceFromScriptName(scriptName);
            }

            return NamespacePersistence.DefaultMode switch
            {
                DefaultNamespaceMode.Fixed => NamespacePersistence.FixedNamespace.Trim(),
                DefaultNamespaceMode.FromPath => BuildNamespaceFromPath(path),
                _ => string.Empty
            };
        }

        public static string BuildNamespaceFromScriptName(string scriptName)
        {
            string normalizedScriptName = NormalizeName(scriptName);
            return SanitizeNamespaceSegment(normalizedScriptName);
        }

        public static string BuildNamespaceFromPath(string path)
        {
            string normalizedPath = NormalizeFolderPath(path);
            string relativePath = normalizedPath.StartsWith("Assets/")
                ? normalizedPath["Assets/".Length..]
                : string.Empty;

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return string.Empty;
            }

            string[] segments = relativePath
                .Split('/')
                .Select(SanitizeNamespaceSegment)
                .Where(segment => !string.IsNullOrWhiteSpace(segment))
                .ToArray();

            return string.Join(".", segments);
        }

        public static string BuildHeaderComment()
        {
            if (!TemplateDefaultsPersistence.IncludeHeaderComment)
            {
                return string.Empty;
            }

            string headerText = TemplateDefaultsPersistence.HeaderCommentText?.Trim();
            if (string.IsNullOrWhiteSpace(headerText))
            {
                return string.Empty;
            }

            string[] lines = headerText
                .Replace("\r\n", "\n")
                .Split('\n');

            StringBuilder builder = new StringBuilder();
            foreach (string line in lines)
            {
                builder.Append("// ");
                builder.AppendLine(line.TrimEnd());
            }

            return builder.ToString().TrimEnd();
        }

        public static bool IsPathInsideBasePath(string path, string basePath)
        {
            string normalizedPath = NormalizeFolderPathInternal(path);
            string normalizedBasePath = NormalizeFolderPathInternal(basePath);

            return normalizedPath == normalizedBasePath ||
                   normalizedPath.StartsWith(normalizedBasePath + "/");
        }

        private static string ToPascalCase(string input, bool allowUnderscores)
        {
            string workingInput = allowUnderscores
                ? Regex.Replace(input, @"[^A-Za-z0-9_\s]", " ")
                : Regex.Replace(input, @"[^A-Za-z0-9\s]", " ");

            string[] pieces = workingInput
                .Split(allowUnderscores ? new[] { ' ', '_' } : new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            string normalized = string.Concat(pieces.Select(UppercaseFirstLetter));
            if (!allowUnderscores)
            {
                return normalized;
            }

            List<string> underscoreBlocks = input
                .Split('_')
                .Select(block => string.Concat(Regex.Split(block, @"[^A-Za-z0-9]+")
                    .Where(part => !string.IsNullOrWhiteSpace(part))
                    .Select(UppercaseFirstLetter)))
                .Where(block => !string.IsNullOrWhiteSpace(block))
                .ToList();

            return underscoreBlocks.Count > 0
                ? string.Join("_", underscoreBlocks)
                : normalized;
        }

        private static string UppercaseFirstLetter(string input)
        {
            string cleanedInput = Regex.Replace(input ?? string.Empty, @"\s+", string.Empty);
            if (string.IsNullOrEmpty(cleanedInput))
            {
                return cleanedInput;
            }

            return char.ToUpper(cleanedInput[0]) + cleanedInput[1..];
        }

        private static string SanitizeNamespaceSegment(string segment)
        {
            if (string.IsNullOrWhiteSpace(segment))
            {
                return string.Empty;
            }

            string alphanumeric = Regex.Replace(segment, @"[^A-Za-z0-9_]", " ");
            string normalized = string.Concat(
                alphanumeric
                    .Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(UppercaseFirstLetter));

            if (string.IsNullOrEmpty(normalized))
            {
                return string.Empty;
            }

            return char.IsDigit(normalized[0]) ? "_" + normalized : normalized;
        }

        private static string NormalizeFolderPathInternal(string path)
        {
            string normalizedPath = (path ?? string.Empty).Trim().Replace('\\', '/');

            if (string.IsNullOrWhiteSpace(normalizedPath))
            {
                normalizedPath = "Assets/";
            }

            if (!normalizedPath.StartsWith("Assets"))
            {
                normalizedPath = "Assets/" + normalizedPath.TrimStart('/');
            }

            if (normalizedPath.Length > "Assets".Length && normalizedPath.EndsWith("/"))
            {
                normalizedPath = normalizedPath.TrimEnd('/');
            }

            return normalizedPath;
        }
    }
}
