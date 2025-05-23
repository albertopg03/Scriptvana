namespace Scriptvana.Editor.Models
{
    /// <summary>
    /// Modelo de datos del formulario que rellena el usuario para poder crear un script.
    /// Aquí están todos los datos que compone la configuración de un script, para su posterior creación
    /// </summary>
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
    }
}
