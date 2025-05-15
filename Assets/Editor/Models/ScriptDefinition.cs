using UnityEngine;

public class ScriptDefinition
{
    public string Name { get; set; }
    public ScriptType Type { get; set; }
    public string NSpace { get; set; }
    public string Path { get; set; }
    public int Id { get; set; }

    public ScriptDefinition(int id, string name, ScriptType type, string nSpace, string path)
    {
        Id = id;
        Name = name;
        Type = type;
        NSpace = nSpace;
        Path = path;
    }
    
    public override string ToString()
    {
        return $"{Name}.cs ({Type}) in {Path}";
    }
}
