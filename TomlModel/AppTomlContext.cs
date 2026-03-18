namespace SixSlime.AxBind3.TomlModel;

using System.Text.Json;
using System.Text.Json.Serialization;
using Tomlyn;
using Tomlyn.Serialization;

[TomlSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[TomlSerializable(typeof(ProfileFile))]
[TomlSerializable(typeof(MapFile))]
public partial class AppTomlContext : TomlSerializerContext;