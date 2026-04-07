// Knowledge Graph Export to Neo4j
// Generated: 2026-04-06 22:14:03
// Nodes: 47, Edges: 79

// Clear existing data (optional - uncomment if needed)
// MATCH (n) DETACH DELETE n;

// Create nodes

CREATE (nuserrepository_lock:Entity {id: "userrepository_lock", label: "lock()", community: 4, merge_count: "7", source_location: "L121"});
CREATE (nuserrepository:Entity {id: "userrepository", label: "UserRepository.cs", community: 4, source_location: "L1"});
CREATE (nuserrepository_argumentexception:Entity {id: "userrepository_argumentexception", label: "ArgumentException()", community: 4, merge_count: "2", source_location: "L71"});
CREATE (nuserrepository_argumentnullexception:Entity {id: "userrepository_argumentnullexception", label: "ArgumentNullException()", community: 4, merge_count: "2", source_location: "L68"});
CREATE (nuserrepository_invalidoperationexception:Entity {id: "userrepository_invalidoperationexception", label: "InvalidOperationException()", community: 4, merge_count: "3", source_location: "L93"});
CREATE (nservicecollectionextensions_minilibrary:Entity {id: "servicecollectionextensions_minilibrary", label: "MiniLibrary", community: 3, source_location: "L2"});
CREATE (nuserrepository_if:Entity {id: "userrepository_if", label: "if()", community: 4, merge_count: "7", source_location: "L92"});
CREATE (nuserservice_deleteuserasync:Entity {id: "userservice_deleteuserasync", label: "DeleteUserAsync()", community: 2, source_location: "L103"});
CREATE (nirepository_updateasync:Entity {id: "irepository_updateasync", label: "UpdateAsync()", community: 1, source_location: "L33"});
CREATE (nuserservice_deactivateuserasync:Entity {id: "userservice_deactivateuserasync", label: "DeactivateUserAsync()", community: 2, source_location: "L89"});
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237:File {id: "file:C:\\src\\graphify-dotnet\\samples\\mini-library\\src\\UserService.cs", label: "UserService.cs", community: 2, full_path: "C:\\src\\graphify-dotnet\\samples\\mini-library\\src\\UserService.cs"});
CREATE (nuserservice_createuserasync:Entity {id: "userservice_createuserasync", label: "CreateUserAsync()", community: 2, source_location: "L26"});
CREATE (nuserrepository_userrepository:Entity {id: "userrepository_userrepository", label: "UserRepository", community: 0, source_location: "L7"});
CREATE (nuserrepository_updateasync:Entity {id: "userrepository_updateasync", label: "UpdateAsync()", community: 0, source_location: "L65"});
CREATE (nuserrepository_minilibrary:Entity {id: "userrepository_minilibrary", label: "MiniLibrary", community: 0, source_location: "L1"});
CREATE (nuserrepository_getallasync:Entity {id: "userrepository_getallasync", label: "GetAllAsync()", community: 0, source_location: "L30"});
CREATE (nirepository_minilibrary:Entity {id: "irepository_minilibrary", label: "MiniLibrary", community: 1, source_location: "L1"});
CREATE (nuser_user:Entity {id: "user_user", label: "User", community: 5, source_location: "L6"});
CREATE (nuser:Entity {id: "user", label: "User.cs", community: 5, source_location: "L1"});
CREATE (nirepository:Entity {id: "irepository", label: "IRepository.cs", community: 1, source_location: "L1"});
CREATE (nuser_validate:Entity {id: "user_validate", label: "Validate()", community: 5, source_location: "L37"});
CREATE (nuserservice_invalidoperationexception:Entity {id: "userservice_invalidoperationexception", label: "InvalidOperationException()", community: 6, merge_count: "3", source_location: "L93"});
CREATE (nirepository_deleteasync:Entity {id: "irepository_deleteasync", label: "DeleteAsync()", community: 1, source_location: "L39"});
CREATE (nirepository_getallasync:Entity {id: "irepository_getallasync", label: "GetAllAsync()", community: 1, source_location: "L21"});
CREATE (nirepository_irepository:Entity {id: "irepository_irepository", label: "IRepository", community: 1, source_location: "L8"});
CREATE (nirepository_addasync:Entity {id: "irepository_addasync", label: "AddAsync()", community: 1, source_location: "L27"});
CREATE (nuser_minilibrary:Entity {id: "user_minilibrary", label: "MiniLibrary", community: 5, source_location: "L1"});
CREATE (nuserservice_if:Entity {id: "userservice_if", label: "if()", community: 6, merge_count: "6", source_location: "L92"});
CREATE (nuserservice_argumentexception:Entity {id: "userservice_argumentexception", label: "ArgumentException()", community: 6, merge_count: "3", source_location: "L62"});
CREATE (nuserservice:Entity {id: "userservice", label: "UserService.cs", community: 6, source_location: "L1"});
CREATE (nuserrepository_getactivecountasync:Entity {id: "userrepository_getactivecountasync", label: "GetActiveCountAsync()", community: 0, source_location: "L119"});
CREATE (nuserservice_minilibrary:Entity {id: "userservice_minilibrary", label: "MiniLibrary", community: 2, source_location: "L1"});
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735:File {id: "file:C:\\src\\graphify-dotnet\\samples\\mini-library\\src\\UserRepository.cs", label: "UserRepository.cs", community: 0, full_path: "C:\\src\\graphify-dotnet\\samples\\mini-library\\src\\UserRepository.cs"});
CREATE (nservicecollectionextensions_argumentnullexception:Entity {id: "servicecollectionextensions_argumentnullexception", label: "ArgumentNullException()", community: 3, merge_count: "2", source_location: "L42"});
CREATE (nuserrepository_deleteasync:Entity {id: "userrepository_deleteasync", label: "DeleteAsync()", community: 0, source_location: "L88"});
CREATE (nuserrepository_findbyemailasync:Entity {id: "userrepository_findbyemailasync", label: "FindByEmailAsync()", community: 0, source_location: "L104"});
CREATE (nuserrepository_addasync:Entity {id: "userrepository_addasync", label: "AddAsync()", community: 0, source_location: "L42"});
CREATE (nuserservice_isuseractiveasync:Entity {id: "userservice_isuseractiveasync", label: "IsUserActiveAsync()", community: 2, source_location: "L123"});
CREATE (nservicecollectionextensions_addminilibrary:Entity {id: "servicecollectionextensions_addminilibrary", label: "AddMiniLibrary()", community: 3, source_location: "L16"});
CREATE (nservicecollectionextensions:Entity {id: "servicecollectionextensions", label: "ServiceCollectionExtensions.cs", community: 3, source_location: "L1"});
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra377:File {id: "file:C:\\src\\graphify-dotnet\\samples\\mini-library\\src\\ServiceCollectionExtensions.cs", label: "ServiceCollectionExtensions.cs", community: 3, full_path: "C:\\src\\graphify-dotnet\\samples\\mini-library\\src\\ServiceCollectionExtensions.cs"});
CREATE (nuserservice_updateuserasync:Entity {id: "userservice_updateuserasync", label: "UpdateUserAsync()", community: 2, source_location: "L73"});
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra305:File {id: "file:C:\\src\\graphify-dotnet\\samples\\mini-library\\src\\User.cs", label: "User.cs", community: 5, full_path: "C:\\src\\graphify-dotnet\\samples\\mini-library\\src\\User.cs"});
CREATE (nuserservice_getactiveusersasync:Entity {id: "userservice_getactiveusersasync", label: "GetActiveUsersAsync()", community: 2, source_location: "L112"});
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra186:File {id: "file:C:\\src\\graphify-dotnet\\samples\\mini-library\\src\\IRepository.cs", label: "IRepository.cs", community: 1, full_path: "C:\\src\\graphify-dotnet\\samples\\mini-library\\src\\IRepository.cs"});
CREATE (nuserservice_userservice:Entity {id: "userservice_userservice", label: "UserService()", community: 6, merge_count: "2", source_location: "L15"});
CREATE (nservicecollectionextensions_if:Entity {id: "servicecollectionextensions_if", label: "if()", community: 3, merge_count: "2", source_location: "L41"});

// Create relationships

CREATE (nuserrepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_addasync);
CREATE (nuserrepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_deleteasync);
CREATE (nuserrepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_findbyemailasync);
CREATE (nuserrepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_getactivecountasync);
CREATE (nuserrepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_getallasync);
CREATE (nuserrepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_minilibrary);
CREATE (nuserrepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_updateasync);
CREATE (nuserrepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_userrepository);
CREATE (nuserrepository)-[:CONTAINS {weight: 7.00, confidence: "EXTRACTED"}]->(nuserrepository_lock);
CREATE (nuserrepository)-[:CONTAINS {weight: 7.00, confidence: "EXTRACTED"}]->(nuserrepository_if);
CREATE (nuserrepository)-[:CONTAINS {weight: 2.00, confidence: "EXTRACTED"}]->(nuserrepository_argumentnullexception);
CREATE (nuserrepository)-[:CONTAINS {weight: 2.00, confidence: "EXTRACTED"}]->(nuserrepository_argumentexception);
CREATE (nuserrepository)-[:CONTAINS {weight: 3.00, confidence: "EXTRACTED"}]->(nuserrepository_invalidoperationexception);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_minilibrary);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_userservice);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_createuserasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_if);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_argumentexception);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_invalidoperationexception);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_updateuserasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_deactivateuserasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_deleteuserasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_getactiveusersasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra237)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_isuseractiveasync);
CREATE (nuser)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuser_minilibrary);
CREATE (nuser)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuser_user);
CREATE (nuser)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuser_validate);
CREATE (nirepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository_minilibrary);
CREATE (nirepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository_irepository);
CREATE (nirepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository_getallasync);
CREATE (nirepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository_addasync);
CREATE (nirepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository_updateasync);
CREATE (nirepository)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository_deleteasync);
CREATE (nuserservice)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_createuserasync);
CREATE (nuserservice)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_deactivateuserasync);
CREATE (nuserservice)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_deleteuserasync);
CREATE (nuserservice)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_getactiveusersasync);
CREATE (nuserservice)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_isuseractiveasync);
CREATE (nuserservice)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_minilibrary);
CREATE (nuserservice)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserservice_updateuserasync);
CREATE (nuserservice)-[:CONTAINS {weight: 2.00, confidence: "EXTRACTED"}]->(nuserservice_userservice);
CREATE (nuserservice)-[:CONTAINS {weight: 6.00, confidence: "EXTRACTED"}]->(nuserservice_if);
CREATE (nuserservice)-[:CONTAINS {weight: 3.00, confidence: "EXTRACTED"}]->(nuserservice_argumentexception);
CREATE (nuserservice)-[:CONTAINS {weight: 3.00, confidence: "EXTRACTED"}]->(nuserservice_invalidoperationexception);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_minilibrary);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_userrepository);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_lock);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_getallasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_addasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_if);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_argumentnullexception);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_argumentexception);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_invalidoperationexception);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_updateasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_deleteasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_findbyemailasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra735)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuserrepository_getactivecountasync);
CREATE (nservicecollectionextensions)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nservicecollectionextensions_minilibrary);
CREATE (nservicecollectionextensions)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nservicecollectionextensions_addminilibrary);
CREATE (nservicecollectionextensions)-[:CONTAINS {weight: 2.00, confidence: "EXTRACTED"}]->(nservicecollectionextensions_if);
CREATE (nservicecollectionextensions)-[:CONTAINS {weight: 2.00, confidence: "EXTRACTED"}]->(nservicecollectionextensions_argumentnullexception);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra377)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nservicecollectionextensions);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra377)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nservicecollectionextensions_minilibrary);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra377)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nservicecollectionextensions_addminilibrary);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra377)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nservicecollectionextensions_if);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra377)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nservicecollectionextensions_argumentnullexception);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra305)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuser);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra305)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuser_minilibrary);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra305)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuser_user);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra305)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nuser_validate);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra186)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra186)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository_minilibrary);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra186)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository_irepository);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra186)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository_getallasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra186)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository_addasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra186)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository_updateasync);
CREATE (nfile_C__src_graphify_dotnet_samples_mini_libra186)-[:CONTAINS {weight: 1.00, confidence: "EXTRACTED"}]->(nirepository_deleteasync);

// Create indexes for better query performance

CREATE INDEX IF NOT EXISTS FOR (n:Entity) ON (n.id);
CREATE INDEX IF NOT EXISTS FOR (n:Entity) ON (n.label);
CREATE INDEX IF NOT EXISTS FOR (n:File) ON (n.id);
CREATE INDEX IF NOT EXISTS FOR (n:File) ON (n.label);

// Index for community-based queries
CREATE INDEX IF NOT EXISTS FOR (n:Entity) ON (n.community);
CREATE INDEX IF NOT EXISTS FOR (n:File) ON (n.community);

// Query examples:
// - Find all nodes: MATCH (n) RETURN n LIMIT 25;
// - Find nodes by type: MATCH (n:Class) RETURN n LIMIT 25;
// - Find nodes in a community: MATCH (n) WHERE n.community = 1 RETURN n;
// - Find highly connected nodes: MATCH (n) RETURN n, size((n)--()) as degree ORDER BY degree DESC LIMIT 10;
// - Find paths: MATCH p=shortestPath((a)-[*]-(b)) WHERE a.id='Node1' AND b.id='Node2' RETURN p;
