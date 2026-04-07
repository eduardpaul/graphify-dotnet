# Decision: ConfigPersistence and ConfigurationFactory Test Coverage

**Author:** Tank (Tester)
**Date:** 2026-04-07
**Status:** Implemented

## Context

Trinity implemented an interactive configuration wizard (ConfigWizard, ConfigPersistence, ConfigurationFactory updates) for the CLI using Spectre.Console. Tests were needed for the file I/O and configuration layering logic.

## Decision

- **ConfigWizard is NOT unit-testable** in its current form — it uses static `AnsiConsole` methods that require a TTY. If we want wizard-level testing in the future, we'd need to inject `IAnsiConsole` instead.
- **ConfigPersistence** is fully tested via file round-trips. The `[Collection("ConfigFile")]` attribute is required on any test class that reads/writes `appsettings.local.json` in `AppContext.BaseDirectory`.
- **ConfigurationFactory** integration tests write temp files and verify layer priority (local file < CLI args).

## Impact

- 18 new tests covering all ConfigPersistence code paths and ConfigurationFactory local config loading.
- Any future test class that touches `appsettings.local.json` in the test output directory MUST use `[Collection("ConfigFile")]` to avoid race conditions.
