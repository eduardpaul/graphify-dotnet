// Simple CLI stub for graphify-dotnet
// Full System.CommandLine implementation pending proper API documentation
// For now, provide basic usage info and manual command handling

if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
{
    Console.WriteLine("graphify-dotnet: Transform codebases into knowledge graphs");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  graphify run [path] [options]     Run the full pipeline");
    Console.WriteLine("  graphify benchmark [graph.json]   Measure token reduction");
    Console.WriteLine();
    Console.WriteLine("Run Command Options:");
    Console.WriteLine("  --output, -o <path>     Output directory (default: graphify-out)");
    Console.WriteLine("  --format, -f <formats>  Export formats: json,html (default: json,html)");
    Console.WriteLine("  --verbose, -v           Enable verbose output");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  graphify run .                          # Analyze current directory");
    Console.WriteLine("  graphify run ./src --output ./docs     # Custom output");
    Console.WriteLine("  graphify benchmark graphify-out/graph.json");
    Console.WriteLine();
    return 0;
}

var command = args[0].ToLowerInvariant();

if (command == "run")
{
    var path = args.Length > 1 && !args[1].StartsWith("--") ? args[1] : ".";
    var output = "graphify-out";
    var formats = new[] { "json", "html" };
    var verbose = args.Contains("--verbose") || args.Contains("-v");
    
    // Parse options
    for (int i = 1; i < args.Length; i++)
    {
        if (args[i] == "--output" || args[i] == "-o")
        {
            if (i + 1 < args.Length) output = args[++i];
        }
        else if (args[i] == "--format" || args[i] == "-f")
        {
            if (i + 1 < args.Length) formats = args[++i].Split(',');
        }
    }
    
    var runner = new Graphify.Cli.PipelineRunner(Console.Out, verbose);
    var graph = await runner.RunAsync(path, output, formats, useCache: true, CancellationToken.None);
    return graph != null ? 0 : 1;
}
else if (command == "benchmark")
{
    var graphPath = args.Length > 1 ? args[1] : "graphify-out/graph.json";
    var result = await Graphify.Pipeline.BenchmarkRunner.RunAsync(graphPath, corpusWords: null);
    Graphify.Pipeline.BenchmarkRunner.PrintBenchmark(result, Console.Out);
    return string.IsNullOrEmpty(result.Error) ? 0 : 1;
}
else
{
    Console.WriteLine($"Unknown command: {command}");
    Console.WriteLine("Run 'graphify --help' for usage.");
    return 1;
}
