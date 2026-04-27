using System.Text;
using System.Xml.Linq;

namespace BatchPay.Frontend.DevTools;

public static class DevIconSeeder
{
#if DEBUG
    public static void EnsureIconsExist()
    {
        var imagesDir = FindImagesDirectory();
        if (imagesDir is null)
            return; // fx Android/iOS runtime hvor repo ikke er på filsystemet

        Directory.CreateDirectory( imagesDir );

        var keys = new[]
        {
            "pizza","beer","trip","party","work","coffee","burger","sushi","taco","music","movie",
            "food","cafe","education","fun","gift","health","housing","other","shopping","transport","travel","utilities"
        };

        var bodies = GetBodies();
        var addedFiles = new List<string>();

        foreach (var key in keys)
        {
            var fileName = $"icon_{key}.svg";
            var fullPath = Path.Combine( imagesDir, fileName );

            if (File.Exists( fullPath ))
                continue;

            if (!bodies.TryGetValue( key, out var body ))
                body = @"<circle cx=""12"" cy=""12"" r=""9""/><path d=""M12 8v8""/><path d=""M8 12h8""/>";

            File.WriteAllText( fullPath, Svg( body ), new UTF8Encoding( encoderShouldEmitUTF8Identifier: false ) );
            addedFiles.Add(fileName);
        }

        if (addedFiles.Any())
        {
            UpdateProjectFile(addedFiles);
        }
    }

    private static void UpdateProjectFile(IEnumerable<string> addedFiles)
    {
        var projectDir = FindProjectDirectory();
        if (projectDir is null) return;

        var csprojPath = Path.Combine(projectDir.FullName, "BatchPay.Frontend.csproj");
        if (!File.Exists(csprojPath)) return;

        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;

        var itemGroup = doc.Root?.Elements(ns + "ItemGroup")
            .FirstOrDefault(ig => ig.Elements(ns + "MauiImage").Any());

        if (itemGroup is null)
        {
            itemGroup = new XElement(ns + "ItemGroup");
            doc.Root?.Add(itemGroup);
        }

        var existingImages = new HashSet<string>(
            itemGroup.Elements(ns + "MauiImage")
                     .Select(e => e.Attribute("Include")?.Value ?? "")
                     .Where(s => !string.IsNullOrEmpty(s))
        );

        foreach (var file in addedFiles)
        {
            var includePath = $@"Resources\Images\{file}";
            if (!existingImages.Contains(includePath))
            {
                itemGroup.Add(new XElement(ns + "MauiImage",
                    new XAttribute("Include", includePath)
                ));
            }
        }

        doc.Save(csprojPath);
    }

    private static DirectoryInfo? FindProjectDirectory()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        for (var i = 0; i < 25 && dir is not null; i++)
        {
            if (dir.GetFiles("BatchPay.Frontend.csproj").Any())
            {
                return dir;
            }
            dir = dir.Parent;
        }
        return null;
    }


    /// <summary>
    /// Finder ...\BatchPay.Frontend\Resources\Images ved at gå opad og lede efter .sln eller csproj.
    /// </summary>
    private static string? FindImagesDirectory()
    {
        // Start fra base directory (bin/...)
        var dir = new DirectoryInfo( AppContext.BaseDirectory );

        for (var i = 0; i < 25 && dir is not null; i++)
        {
            // 1) Hvis vi står i solution root (har *.sln)
            var sln = dir.GetFiles( "*.sln" ).FirstOrDefault();
            if (sln is not null)
            {
                var candidate = Path.Combine( dir.FullName, "BatchPay.Frontend", "Resources", "Images" );
                return candidate;
            }

            // 2) Hvis vi står i BatchPay.Frontend projektmappen (har BatchPay.Frontend.csproj)
            var frontendProj = dir.GetFiles( "BatchPay.Frontend.csproj" ).FirstOrDefault();
            if (frontendProj is not null)
            {
                var candidate = Path.Combine( dir.FullName, "Resources", "Images" );
                return candidate;
            }

            // 3) Hvis vi kan se BatchPay.Frontend-mappen herfra
            var frontendFolder = Path.Combine( dir.FullName, "BatchPay.Frontend" );
            if (Directory.Exists( frontendFolder ))
            {
                var candidate = Path.Combine( frontendFolder, "Resources", "Images" );
                return candidate;
            }

            dir = dir.Parent;
        }

        return null;
    }

    private static string Svg( string body ) => $"""
<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"
  fill="none" stroke="#FFFFFF" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
{body}
</svg>
""";

    private static Dictionary<string, string> GetBodies() => new()
    {
        [ "pizza" ] = """
  <path d="M12 3a18 18 0 0 1 9 2c-2 8-6 14-9 16C9 19 5 13 3 5a18 18 0 0 1 9-2z"/>
  <path d="M12 8h0"/><path d="M9 11h0"/><path d="M15 12h0"/>
  <path d="M12 3v18"/>
""",
        [ "beer" ] = """
  <path d="M6 7h9v9a4 4 0 0 1-4 4H10a4 4 0 0 1-4-4V7z"/>
  <path d="M15 9h2a2.5 2.5 0 0 1 0 5h-2"/>
  <path d="M8 3c0 1 .6 1.6 1.2 2.2.6.6 1.2 1.2 1.2 2.3"/>
  <path d="M11 3c0 1 .6 1.6 1.2 2.2.6.6 1.2 1.2 1.2 2.3"/>
""",
        [ "trip" ] = """
  <rect x="6" y="7" width="12" height="14" rx="2"/>
  <path d="M8 7V6a4 4 0 0 1 8 0v1"/>
  <path d="M10 12h4"/><path d="M10 16h4"/>
""",
        [ "party" ] = """
  <path d="M4 20l6-6"/><path d="M7 17l3 3"/>
  <path d="M13 7l4-4"/><path d="M15 9l2-2"/>
  <path d="M9 15c-2-2-2-5 0-7l1-1 7 7-1 1c-2 2-5 2-7 0z"/>
""",
        [ "work" ] = """
  <rect x="4" y="7" width="16" height="13" rx="2"/>
  <path d="M9 7V5a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2"/>
  <path d="M4 12h16"/><path d="M10 12v2h4v-2"/>
""",
        [ "coffee" ] = """
  <path d="M5 8h10v6a4 4 0 0 1-4 4H9a4 4 0 0 1-4-4V8z"/>
  <path d="M15 9h2.5a2.5 2.5 0 0 1 0 5H15"/>
  <path d="M7 21h10"/>
  <path d="M9 6c0-1 .6-1.6 1.2-2.2.6-.6 1.2-1.2 1.2-2.3"/>
""",
        [ "burger" ] = """
  <path d="M5 10a7 7 0 0 1 14 0"/>
  <path d="M6 11h12"/><path d="M6 14h12"/>
  <path d="M7 16a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2"/>
  <path d="M8 9h0"/><path d="M12 9h0"/><path d="M16 9h0"/>
""",
        [ "sushi" ] = """
  <rect x="6" y="7" width="12" height="10" rx="3"/>
  <path d="M6 12h12"/><path d="M9 7v10"/><path d="M15 7v10"/>
  <path d="M8 17c.5 2 2.5 4 4 4s3.5-2 4-4"/>
""",
        [ "taco" ] = """
  <path d="M6 15a6 6 0 0 1 12 0v2a3 3 0 0 1-3 3H9a3 3 0 0 1-3-3v-2z"/>
  <path d="M7 15c2-3 8-3 10 0"/>
  <path d="M9 14h0"/><path d="M12 13h0"/><path d="M15 14h0"/>
""",
        [ "music" ] = """
  <path d="M11 20V6l10-2v14"/>
  <path d="M11 10l10-2"/>
  <path d="M9 18a2 2 0 1 0 0 4a2 2 0 0 0 0-4z"/>
  <path d="M17 16a2 2 0 1 0 0 4a2 2 0 0 0 0-4z"/>
""",
        [ "movie" ] = """
  <rect x="4" y="7" width="16" height="12" rx="2"/>
  <path d="M4 11h16"/>
  <path d="M8 7l2 4"/><path d="M12 7l2 4"/><path d="M16 7l2 4"/>
""",
        [ "food" ] = """
  <path d="M7 3v7"/><path d="M10 3v7"/><path d="M7 6h3"/>
  <path d="M9 10v11"/>
  <path d="M14 3v8a3 3 0 0 0 6 0V3"/>
  <path d="M17 11v10"/>
""",
        [ "cafe" ] = """
  <path d="M4 8h11v6a4 4 0 0 1-4 4H8a4 4 0 0 1-4-4V8z"/>
  <path d="M15 9h2.5a2.5 2.5 0 0 1 0 5H15"/>
  <path d="M6 21h8"/>
""",
        [ "education" ] = """
  <path d="M4 6h11a3 3 0 0 1 3 3v12H7a3 3 0 0 0-3 3V6z"/>
  <path d="M7 9h8"/><path d="M7 13h8"/><path d="M7 17h6"/>
""",
        [ "fun" ] = """
  <path d="M8 14l-3 3"/><path d="M10 16l-3 3"/>
  <path d="M14 10l3-3"/><path d="M16 12l3-3"/>
  <path d="M9 15c-2-2-2-5 0-7l1-1 7 7-1 1c-2 2-5 2-7 0z"/>
""",
        [ "gift" ] = """
  <path d="M20 12v9H4v-9"/><path d="M2 12h20"/>
  <path d="M12 12v9"/>
  <path d="M4 8h16v4H4z"/>
  <path d="M12 8c-2.2 0-3.6-1-3.6-2.4S9.6 3.2 12 6c2.4-2.8 3.6-1.8 3.6-.4S14.2 8 12 8z"/>
""",
        [ "health" ] = """
  <path d="M12 21s-7-4.4-9.5-9.2C.7 8.3 2.6 5.2 5.8 5c1.7-.1 3.3.8 4.2 2.1.9-1.3 2.5-2.2 4.2-2.1 3.2.2 5.1 3.3 3.3 6.8C19 16.6 12 21 12 21z"/>
""",
        [ "housing" ] = """
  <path d="M3 11l9-8 9 8"/>
  <path d="M5 10v11h14V10"/>
  <path d="M10 21v-6h4v6"/>
""",
        [ "other" ] = """
  <path d="M6 12h0"/><path d="M12 12h0"/><path d="M18 12h0"/>
  <path d="M6 6h0"/><path d="M12 6h0"/><path d="M18 6h0"/>
  <path d="M6 18h0"/><path d="M12 18h0"/><path d="M18 18h0"/>
""",
        [ "shopping" ] = """
  <path d="M7 8V7a5 5 0 0 1 10 0v1"/>
  <path d="M6 8h12l-1 13H7L6 8z"/>
  <path d="M9 11h0"/><path d="M15 11h0"/>
""",
        [ "transport" ] = """
  <path d="M6 17V8a3 3 0 0 1 3-3h6a3 3 0 0 1 3 3v9"/>
  <path d="M6 11h12"/><path d="M7 17h10"/>
  <path d="M8 17a2 2 0 1 0 0 4a2 2 0 0 0 0-4z"/>
  <path d="M16 17a2 2 0 1 0 0 4a2 2 0 0 0 0-4z"/>
""",
        [ "travel" ] = """
  <path d="M4 12l8-9 8 9"/>
  <path d="M2 12h20"/>
  <path d="M12 11v10"/><path d="M10 21l2-2 2 2"/>
  <path d="M7 12l-2 7"/><path d="M17 12l2 7"/>
""",
        [ "utilities" ] = """
  <path d="M13 2L3 14h7l-1 8 10-12h-7l1-8z"/>
""",
    };
#endif
}
