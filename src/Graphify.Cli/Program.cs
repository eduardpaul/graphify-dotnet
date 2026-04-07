using System.CommandLine;
using System.CommandLine.Invocation;
using Graphify.Pipeline;

var rootCommand = new RootCommand("graphify-dotnet: AI-powered knowledge graph builder for codebases");

// ── Shared option/argument factory helpers ───────────────────────────────
static Argument<string> PathArg(string description)
{
    var arg = new Argument<string>("path", description);
    arg.SetDefaultValue(".");
    return arg;
}

static void AddPipelineOptions(Command cmd,
    out Option<string> outputOpt, out Option<string> formatOpt,
    out Option<bool> verboseOpt, out Option<string?> providerOpt,
    out Option<string?> endpointOpt, out Option<string?> apiKeyOpt,
    out Option<string?> modelOpt, out Option<string?> deploymentOpt)
{
    outputOpt = new Option<string>("--output", "Output directory");
    outputOpt.AddAlias("-o");
    outputOpt.SetDefaultValue("graphify-out");

    formatOpt = new Option<string>("--format", "Export formats (comma-separated)");
    formatOpt.AddAlias("-f");
    formatOpt.SetDefaultValue("json,html");

    verboseOpt = new Option<bool>("--verbose", "Enable verbose output");
    verboseOpt.AddAlias("-v");

    providerOpt = new Option<string?>("--provider", "AI provider: azureopenai, ollama");
    providerOpt.AddAlias("-p");

    endpointOpt = new Option<string?>("--endpoint", "AI service endpoint URL");
    apiKeyOpt = new Option<string?>("--api-key", "API key for the AI provider");
    modelOpt = new Option<string?>("--model", "Model ID (e.g., gpt-4o, llama3.2)");
    deploymentOpt = new Option<string?>("--deployment", "Azure OpenAI deployment name");

    cmd.AddOption(outputOpt);
    cmd.AddOption(formatOpt);
    cmd.AddOption(verboseOpt);
    cmd.AddOption(providerOpt);
    cmd.AddOption(endpointOpt);
    cmd.AddOption(apiKeyOpt);
    cmd.AddOption(modelOpt);
    cmd.AddOption(deploymentOpt);
}

// ── run command ──────────────────────────────────────────────────────────
var runPathArg = PathArg("Path to the project to analyze");

var runCommand = new Command("run", "Run the full extraction and graph-building pipeline");
runCommand.AddArgument(runPathArg);
AddPipelineOptions(runCommand,
    out var runOutputOpt, out var runFormatOpt, out var runVerboseOpt,
    out var runProviderOpt, out var runEndpointOpt, out var runApiKeyOpt,
    out var runModelOpt, out var runDeploymentOpt);

runCommand.SetHandler(async (InvocationContext context) =>
{
    var path = context.ParseResult.GetValueForArgument(runPathArg);
    var output = context.ParseResult.GetValueForOption(runOutputOpt)!;
    var format = context.ParseResult.GetValueForOption(runFormatOpt)!;
    var verbose = context.ParseResult.GetValueForOption(runVerboseOpt);
    var provider = context.ParseResult.GetValueForOption(runProviderOpt);

    var formats = format.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    if (!string.IsNullOrEmpty(provider))
    {
        Console.WriteLine($"ℹ AI provider '{provider}' configured. Semantic extraction will be available after configuration setup.");
    }

    var runner = new Graphify.Cli.PipelineRunner(Console.Out, verbose);
    var graph = await runner.RunAsync(path, output, formats, useCache: true, context.GetCancellationToken());
    context.ExitCode = graph != null ? 0 : 1;
});

rootCommand.AddCommand(runCommand);

// ── watch command ────────────────────────────────────────────────────────
var watchPathArg = PathArg("Path to the project to watch");

var watchCommand = new Command("watch", "Watch for changes and re-process");
watchCommand.AddArgument(watchPathArg);
AddPipelineOptions(watchCommand,
    out var watchOutputOpt, out var watchFormatOpt, out var watchVerboseOpt,
    out var watchProviderOpt, out var watchEndpointOpt, out var watchApiKeyOpt,
    out var watchModelOpt, out var watchDeploymentOpt);

watchCommand.SetHandler(async (InvocationContext context) =>
{
    var path = context.ParseResult.GetValueForArgument(watchPathArg);
    var output = context.ParseResult.GetValueForOption(watchOutputOpt)!;
    var format = context.ParseResult.GetValueForOption(watchFormatOpt)!;
    var verbose = context.ParseResult.GetValueForOption(watchVerboseOpt);
    var provider = context.ParseResult.GetValueForOption(watchProviderOpt);
    var ct = context.GetCancellationToken();

    var formats = format.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    if (!string.IsNullOrEmpty(provider))
    {
        Console.WriteLine($"ℹ AI provider '{provider}' configured. Semantic extraction will be available after configuration setup.");
    }

    Console.WriteLine("Running initial pipeline...");
    Console.WriteLine();
    var runner = new Graphify.Cli.PipelineRunner(Console.Out, verbose);
    var graph = await runner.RunAsync(path, output, formats, useCache: true, ct);

    if (graph is null)
    {
        Console.WriteLine("Initial pipeline failed. Aborting watch.");
        context.ExitCode = 1;
        return;
    }

    Console.WriteLine();
    using var watchMode = new WatchMode(Console.Out, verbose);
    watchMode.SetInitialGraph(graph);
    await watchMode.WatchAsync(path, output, formats, ct);
    context.ExitCode = 0;
});

rootCommand.AddCommand(watchCommand);

// ── benchmark command ────────────────────────────────────────────────────
var benchmarkPathArg = new Argument<string>("graph-path", "Path to the graph JSON file");
benchmarkPathArg.SetDefaultValue("graphify-out/graph.json");

var benchmarkCommand = new Command("benchmark", "Measure token reduction");
benchmarkCommand.AddArgument(benchmarkPathArg);

benchmarkCommand.SetHandler(async (InvocationContext context) =>
{
    var graphPath = context.ParseResult.GetValueForArgument(benchmarkPathArg);
    var result = await BenchmarkRunner.RunAsync(graphPath, corpusWords: null);
    BenchmarkRunner.PrintBenchmark(result, Console.Out);
    context.ExitCode = string.IsNullOrEmpty(result.Error) ? 0 : 1;
});

rootCommand.AddCommand(benchmarkCommand);

// ── config show command (stub) ───────────────────────────────────────────
var configCommand = new Command("config", "Configuration management");
var configShowCommand = new Command("show", "Display resolved provider settings");

configShowCommand.SetHandler(() =>
{
    Console.WriteLine("Configuration display coming soon.");
});

configCommand.AddCommand(configShowCommand);
rootCommand.AddCommand(configCommand);

// ── invoke ───────────────────────────────────────────────────────────────
return await rootCommand.InvokeAsync(args);
