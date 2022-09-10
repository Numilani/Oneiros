using System.Text.Json.Serialization;

namespace Oneiros;

public class CharacterStatusData
{
    public string Guid { get; set; }
    public string Name { get; set; }
    public int Hp { get; set; }
    public string[] Statuses { get; set; }
}