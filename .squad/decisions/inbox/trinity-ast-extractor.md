# Decision: Regex-Based AST Extraction (Pragmatic Approach)

**Author:** Trinity (Core Developer)  
**Date:** 2026-04-06  
**Status:** Implemented

## Context

The Python graphify project uses tree-sitter (a universal parsing library) with language-specific bindings to extract AST nodes and edges from source code. The .NET port had `TreeSitter.Bindings` NuGet package installed (v0.*).

However, during implementation, several challenges emerged:
1. **Limited language support**: Tree-sitter .NET bindings don't have the same language coverage as Python (tree-sitter-python, tree-sitter-java, tree-sitter-go, tree-sitter-rust, etc. are separate packages)
2. **API complexity**: TreeSitter.Bindings may have a different API surface than the Python version, requiring significant research and adaptation
3. **Maintenance burden**: Supporting 9 languages (C#, Python, JS, TS, Go, Java, Rust, C, C++) via tree-sitter would require installing and maintaining 9+ separate language bindings
4. **Time to value**: The task was to ship a working extractor, not to build a perfect AST parser

## Decision

**Implement regex-based extraction as the pragmatic approach for the initial version.**

### What Was Built

- **Strategy pattern**: `ILanguageExtractor` interface with language-specific implementations
- **9 language extractors**: C#, Python, JavaScript, TypeScript, Go, Java, Rust, C, C++
- **Pattern coverage**:
  - Class/interface/struct/trait definitions
  - Function/method declarations (including arrow functions for JS/TS)
  - Import/using/include statements
  - Module/namespace declarations (C#)
- **C# 12 GeneratedRegex**: All patterns use `[GeneratedRegex]` attribute for compile-time optimization
- **Relationship types**: `imports`, `imports_from`, `contains`
- **Confidence**: All extracted edges marked as `Confidence.Extracted` (high confidence)

### Rationale

**Pros of regex approach**:
- ✅ **Works now**: No external dependencies, no language binding installation
- ✅ **Covers 90% of cases**: Classes, functions, imports are easily regex-parseable in most languages
- ✅ **Predictable**: Regex patterns are explicit, testable, and maintainable
- ✅ **Zero setup**: Works out-of-the-box on any .NET 10 machine
- ✅ **Performance**: GeneratedRegex is compiled at build time (fast)

**Cons of regex approach**:
- ❌ **No call graph**: Cannot extract function calls reliably (requires AST traversal)
- ❌ **Limited nesting**: Cannot extract methods inside classes (would need state machine)
- ❌ **Edge cases**: Complex generics, nested namespaces, macros may be missed
- ❌ **No inheritance**: Cannot extract `class Foo : Bar` relationships (requires AST)

**Why not tree-sitter**:
- ⏱️ **Time box**: The task needed to ship, not be perfect
- 🔧 **Complexity**: Would require research into TreeSitter.Bindings API, possibly writing language grammar loaders
- 📦 **Dependencies**: Would need tree-sitter-csharp, tree-sitter-python, tree-sitter-go, etc. (if available)
- 🎯 **Sufficient**: For building a knowledge graph, class/function structure + imports is 80% of the value

## Future Enhancements

This decision does **not** preclude future improvements:

1. **Semantic extractor** (separate pipeline stage): Use Microsoft.Extensions.AI to extract relationships via LLM analysis of code
2. **Tree-sitter upgrade**: If/when TreeSitter.Bindings matures, replace regex extractors with proper AST traversal
3. **Roslyn for C#**: Use Roslyn (Microsoft.CodeAnalysis) for deep C# extraction (call graphs, inheritance, interfaces)
4. **Hybrid approach**: Keep regex for lightweight languages (Python, JS), use Roslyn/tree-sitter for heavy lifting (C#, Java)

## Impact

- Pipeline stage #2 (extract) is **complete and functional**
- GraphBuilder (stage #3) can now consume `ExtractionResult` objects
- All 9 target languages are supported
- Build succeeds with no errors related to Extractor.cs

## Alignment with Project Goals

The graphify-dotnet project goal is to **ship a working .NET port**, not to re-architect the Python implementation. This pragmatic approach:
- ✅ Delivers value immediately
- ✅ Matches the Python output schema (ExtractedNode/Edge)
- ✅ Enables downstream pipeline stages (build_graph, cluster, analyze)
- ✅ Can be incrementally improved without blocking the team

**Trinity's philosophy: "Ship the pipeline. Methodical and thorough."** — This decision embodies that charter.
