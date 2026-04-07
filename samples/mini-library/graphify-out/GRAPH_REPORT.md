# Graph Report - mini-library  (2026-04-06)

## Summary
- 47 nodes · 79 edges · 7 communities detected
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS

## God Nodes (most connected - your core abstractions)
1. `MiniLibrary` - 2 edges
2. `UserRepository` - 2 edges
3. `MiniLibrary` - 2 edges
4. `MiniLibrary` - 2 edges
5. `User` - 2 edges
6. `IRepository` - 2 edges
7. `MiniLibrary` - 2 edges
8. `MiniLibrary` - 2 edges

## Surprising Connections (you probably didn't know these)
- None detected - all connections are within the same source files.

## Communities

### Community 0 - "Entity (Community 0)"
Cohesion: 0.22
Nodes (9): UserRepository.cs, UserRepository, UpdateAsync(), MiniLibrary, GetAllAsync(), GetActiveCountAsync(), DeleteAsync(), FindByEmailAsync() (+1 more)

### Community 2 - "Entity (Community 2)"
Cohesion: 0.25
Nodes (8): UserService.cs, DeleteUserAsync(), DeactivateUserAsync(), CreateUserAsync(), MiniLibrary, IsUserActiveAsync(), UpdateUserAsync(), GetActiveUsersAsync()

### Community 1 - "Entity (Community 1)"
Cohesion: 0.46
Nodes (8): IRepository.cs, IRepository.cs, UpdateAsync(), MiniLibrary, DeleteAsync(), GetAllAsync(), IRepository, AddAsync()

### Community 4 - "Entity (Community 4)"
Cohesion: 0.33
Nodes (6): UserRepository.cs, lock(), ArgumentException(), ArgumentNullException(), InvalidOperationException(), if()

### Community 3 - "Entity (Community 3)"
Cohesion: 0.60
Nodes (6): ServiceCollectionExtensions.cs, ServiceCollectionExtensions.cs, MiniLibrary, ArgumentNullException(), AddMiniLibrary(), if()

### Community 5 - "Entity (Community 5)"
Cohesion: 0.70
Nodes (5): User.cs, User.cs, User, Validate(), MiniLibrary

### Community 6 - "Entity (Community 6)"
Cohesion: 0.40
Nodes (5): UserService.cs, InvalidOperationException(), if(), ArgumentException(), UserService()

## Suggested Questions
_Not enough signal to generate questions. The graph has no ambiguous edges, no bridge nodes, and all communities are well-connected._

