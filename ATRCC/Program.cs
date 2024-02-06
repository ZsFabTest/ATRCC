using Mono.Cecil;

const string version = "alpha-0.0.1";
System.Console.WriteLine("=== AU Chinese Project Tool ===");
System.Console.WriteLine($"Version: {version}");
System.Console.WriteLine("Made by Lin\n");

System.Console.WriteLine("Input the address: ");
string path = System.Console.ReadLine()!;
System.Console.WriteLine("Input the language data address (The name of the file must be same as the origin file): ");
string resources = System.Console.ReadLine()!;
byte[] bytes;

System.Console.WriteLine("Please wait...");
try
{
    System.IO.FileStream filestream = new(resources, System.IO.FileMode.Open);
    bytes = new byte[filestream.Length];
    filestream.Read(bytes, 0, bytes.Length);
    filestream.Close();
    System.Console.WriteLine("Successfully read.");
}
catch(Exception e)
{
    System.Console.WriteLine($"Error: {e.Message}");
    System.Console.WriteLine(e.StackTrace);
    System.Console.WriteLine("Cannot open the file!");
    System.Console.WriteLine("Press Any Key To Continue...");
    System.Console.ReadKey();
    return;
}

try
{
    ResourcesOperation.ReplaceResource(path, System.IO.Path.GetFileName(resources), bytes);
    string newPath = $"{System.IO.Path.GetDirectoryName(path)}\\{System.IO.Path.GetFileNameWithoutExtension(path)}-new.dll";
    System.Console.WriteLine($"Successfully! {newPath}");
}
catch(Exception e)
{
    System.Console.WriteLine($"Error: {e.Message}");
    System.Console.WriteLine(e.StackTrace);
    System.Console.WriteLine("Cannot write to the file!");
    System.Console.WriteLine("Press Any Key To Continue...");
    System.Console.ReadKey();
}
System.Console.WriteLine("Press Any Key To Continue...");
System.Console.ReadKey();


public static class ResourcesOperation
{
    public static void ReplaceResource(string path, string resourceName, byte[] resource)
    {
        var definition =
            AssemblyDefinition.ReadAssembly(path);

        for (var i = 0; i < definition.MainModule.Resources.Count; i++)
            if (definition.MainModule.Resources[i].Name == resourceName)
            {
                definition.MainModule.Resources.RemoveAt(i);
                break;
            }

        var er = new EmbeddedResource(resourceName, ManifestResourceAttributes.Public, resource);
        definition.MainModule.Resources.Add(er);
        definition.Write($"{System.IO.Path.GetDirectoryName(path)}\\{System.IO.Path.GetFileNameWithoutExtension(path)}-new.dll");
    }

    public static void AddResource(string path, string resourceName, byte[] resource)
    {
        var definition =
            AssemblyDefinition.ReadAssembly(path);

        var er = new EmbeddedResource(resourceName, ManifestResourceAttributes.Public, resource);
        definition.MainModule.Resources.Add(er);
        definition.Write(path);
    }

    public static System.IO.MemoryStream GetResource(string path, string resourceName)
    {
        var definition =
            AssemblyDefinition.ReadAssembly(path);

        foreach (var resource in definition.MainModule.Resources)
            if (resource.Name == resourceName)
            {
                var embeddedResource = (EmbeddedResource)resource;
                var stream = embeddedResource.GetResourceStream();

                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                var memStream = new System.IO.MemoryStream();
                memStream.Write(bytes, 0, bytes.Length);
                memStream.Position = 0;
                return memStream;
            }

        return null!;
    }
}

