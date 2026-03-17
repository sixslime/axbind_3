namespace SixSlime.AxBind3.TomlModel;

using System.Text.Json;
using System.Text.Json.Serialization;
using Tomlyn;
using Tomlyn.Serialization;

[TomlSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[TomlSerializable(typeof(ConfigFile))]
[TomlSerializable(typeof(MapFile))]
internal partial class AppTomlContext : TomlSerializerContext;
