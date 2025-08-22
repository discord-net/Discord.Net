// read the openapi doc

using System.Globalization;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

const string OPENAPI_PATH = "../../../../../discord-api-spec-3.0/openapi-3.0.yaml";
const string OUT_DIR = "../../../../../src/Discord.Net.Rest/Api/Routes/Generated";

if (!File.Exists(OPENAPI_PATH))
    throw new InvalidOperationException("Missing OpenAPI spec file");

using var streamReader = new StreamReader(OPENAPI_PATH);
var reader = new OpenApiStreamReader();
var document = reader.Read(streamReader.BaseStream, out var diagnostic);

var routeParameters = new HashSet<(string Name, string Type)>();

foreach (var (path, item) in document.Paths)
{
    foreach (var (operationType, operation) in item.Operations)
    {
        var operationName = ToPascalCase(operation.OperationId);

        var sb = new StringBuilder($"public sealed record {operationName}(");

        var pathParameters = item.Parameters
            .Where(x => x.In is ParameterLocation.Path)
            .ToArray();
        var queryParameters = item.Parameters
            .Where(x => x.In is ParameterLocation.Query)
            .Concat(operation.Parameters.Where(x => x.In is ParameterLocation.Query))
            .ToArray();

        if (pathParameters.Length > 0)
        {
            for (var i = 0; i < pathParameters.Length; i++)
            {
                if (i > 0) sb.Append(',');

                var parameter = pathParameters[i];
                var type = $"RouteParameters.{ToPascalCase(parameter.Name)}";

                routeParameters.Add((parameter.Name, GetDotnetName(parameter.Schema)));

                sb.AppendLine().Append("    ");
                sb.Append(type).Append(' ').Append(ToPascalCase(parameter.Name));
            }

            sb.AppendLine().Append(")");
        }
        else
        {
            sb.Append(')');
        }

        sb.AppendLine(" : IOperation")
            .AppendLine("{");

        if (queryParameters.Length > 0)
        {
            for (var i = 0; i < queryParameters.Length; i++)
            {
                var parameter = queryParameters[i];
                var type = GetDotnetName(parameter.Schema);

                sb.Append($"    public ");

                if (parameter.Required)
                    sb.Append("required ");
                else
                    type = $"Optional<{type}>";

                sb.Append(type)
                    .Append(' ')
                    .Append(ToPascalCase(parameter.Name))
                    .Append(" { get; init; }")
                    .AppendLine();
            }

            sb.AppendLine();
        }

        if (pathParameters.Length > 0)
        {
            sb.AppendLine(
                $"""
                     public static IReadOnlyList<Type> RouteParameterTypes
                         => [{string.Join(", ", pathParameters.Select(x => $"typeof(RouteParameters.{ToPascalCase(x.Name)})"))}];
                         
                     public IReadOnlyList<RouteParameters> RouteParameters
                         => [{string.Join(", ", pathParameters.Select(x => ToPascalCase(x.Name)))}];
                 """
            ).AppendLine();
        }

        var formatMethod = new StringBuilder();

        var stringIntpPath = path;

        foreach (var parameter in pathParameters)
        {
            stringIntpPath = stringIntpPath.Replace(parameter.Name, ToPascalCase(parameter.Name));
        }

        formatMethod.Append("$\"").Append(stringIntpPath);

        if (queryParameters.Length > 0)
        {
            formatMethod.Append("{QueryStrings.Build(");
            for (var i = 0; i < queryParameters.Length; i++)
            {
                if (i > 0) formatMethod.Append(", ");
                var parameter = queryParameters[i];
                formatMethod.Append(
                    $"(\"{parameter.Name}\", {ToPascalCase(parameter.Name)}{(!parameter.Required ? ".ToNullable()" : string.Empty)})");
            }

            formatMethod.Append(")}");
        }

        formatMethod.Append("\"");

        sb.Append(
            $$"""
                  public static string Path => @"{{path}}";
                  public static string OperationId => "{{operation.OperationId}}";
                  public static RequestMethod Method => RequestMethod.{{operationType}};
                  public static AuthenticationScheme AuthenticationScheme => {{(
                      operation.Security.Count is 0
                          ? "AuthenticationScheme.none"
                          : string.Join(
                              " | ",
                              operation.Security
                                  .SelectMany(x => x
                                      .Keys
                                      .Select(x => x
                                              .Type switch {
                                              SecuritySchemeType.ApiKey => "AuthenticationScheme.BotToken",
                                              SecuritySchemeType.OAuth2 => "AuthenticationScheme.BearerToken",
                                              _ => "None"
                                          }
                                      )
                                  )
                          )
                  )}};
                  
                  public string Format() => {{formatMethod}};
              }
              """
        );

        File.WriteAllText(
            Path.Combine(OUT_DIR, $"{operationName}.cs"),
            $$"""
              namespace Discord.Rest.Api;

              partial class Routes
              {
                 {{sb.ToString().ReplaceLineEndings($"{Environment.NewLine}    ")}}
              }
              """
        );
    }
}

// generate the route parameters
File.WriteAllText(
    Path.Combine(OUT_DIR, "RouteParameters.cs"),
    $$"""
      namespace Discord.Rest.Api;

      public abstract record RouteParameters
      {
          {{
              string.Join(
                  $"{Environment.NewLine}    ",
                  routeParameters
                      .Select(x =>
                      {
                          var name = ToPascalCase(x.Name);

                          return
                              $$"""
                                public sealed record {{name}}({{x.Type}} Value) : RouteParameters
                                {
                                    public static implicit operator {{x.Type}}({{name}} self) => self.Value;
                                    public static implicit operator {{name}}({{x.Type}} value) => new(value);
                                }
                                """.ReplaceLineEndings($"{Environment.NewLine}    ");
                      })
              )
          }}
      }
      """
);

string GetDotnetName(OpenApiSchema schema)
{
    return schema switch
    {
        {Format: "snowflake"} => "Snowflake",
        {Format: "int32"} => "int",
        {Type: "string"} => "string",
        {Type: "integer"} => "int",
        {Type: "boolean"} => "bool",
        {Type: "array", Items: not null} => $"{GetDotnetName(schema.Items)}[]",
        {OneOf.Count: not 0} => $"OneOf<{string.Join(", ", schema.OneOf.Select(GetDotnetName))}>",
        {Type: "null"} => "object?",
        _ => "object?"
    };
}

string ToPascalCase(string s)
{
    return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.Replace("_", " ")).Replace(" ", string.Empty);
}