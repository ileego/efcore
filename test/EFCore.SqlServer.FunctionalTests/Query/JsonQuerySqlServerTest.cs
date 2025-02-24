﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.EntityFrameworkCore.Query;

public class JsonQuerySqlServerTest : JsonQueryTestBase<JsonQuerySqlServerFixture>
{
    public JsonQuerySqlServerTest(JsonQuerySqlServerFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override async Task Basic_json_projection_owner_entity(bool async)
    {
        await base.Basic_json_projection_owner_entity(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Name], JSON_QUERY([j].[OwnedCollectionRoot],'$'), JSON_QUERY([j].[OwnedReferenceRoot],'$')
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Basic_json_projection_owned_reference_root(bool async)
    {
        await base.Basic_json_projection_owned_reference_root(async);

        AssertSql(
            @"SELECT JSON_QUERY([j].[OwnedReferenceRoot],'$'), [j].[Id]
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Basic_json_projection_owned_reference_duplicated(bool async)
    {
        await base.Basic_json_projection_owned_reference_duplicated(async);

        AssertSql(
            @"SELECT JSON_QUERY([j].[OwnedReferenceRoot],'$'), [j].[Id]
FROM [JsonEntitiesBasic] AS [j]
ORDER BY [j].[Id]");
    }

    public override async Task Basic_json_projection_owned_collection_root(bool async)
    {
        await base.Basic_json_projection_owned_collection_root(async);

        AssertSql(
            @"SELECT JSON_QUERY([j].[OwnedCollectionRoot],'$'), [j].[Id]
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Basic_json_projection_owned_reference_branch(bool async)
    {
        await base.Basic_json_projection_owned_reference_branch(async);

        AssertSql(
            @"SELECT JSON_QUERY([j].[OwnedReferenceRoot],'$.OwnedReferenceBranch'), [j].[Id]
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Basic_json_projection_owned_collection_branch(bool async)
    {
        await base.Basic_json_projection_owned_collection_branch(async);

        AssertSql(
            @"SELECT JSON_QUERY([j].[OwnedReferenceRoot],'$.OwnedCollectionBranch'), [j].[Id]
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Basic_json_projection_owned_reference_leaf(bool async)
    {
        await base.Basic_json_projection_owned_reference_leaf(async);

        AssertSql(
            @"SELECT JSON_QUERY([j].[OwnedReferenceRoot],'$.OwnedReferenceBranch.OwnedReferenceLeaf'), [j].[Id]
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Basic_json_projection_owned_collection_leaf(bool async)
    {
        await base.Basic_json_projection_owned_collection_leaf(async);

        AssertSql(
            @"SELECT JSON_QUERY([j].[OwnedReferenceRoot],'$.OwnedReferenceBranch.OwnedCollectionLeaf'), [j].[Id]
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Basic_json_projection_scalar(bool async)
    {
        await base.Basic_json_projection_scalar(async);

        AssertSql(
            @"SELECT CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.Name') AS nvarchar(max))
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Json_scalar_length(bool async)
    {
        await base.Json_scalar_length(async);

        AssertSql(
            @"SELECT [j].[Name]
FROM [JsonEntitiesBasic] AS [j]
WHERE CAST(LEN(CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.Name') AS nvarchar(max))) AS int) > 2");
    }


    public override async Task Basic_json_projection_enum_inside_json_entity(bool async)
    {
        await base.Basic_json_projection_enum_inside_json_entity(async);

        AssertSql(
            @"SELECT [j].[Id], CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.OwnedReferenceBranch.Enum') AS nvarchar(max)) AS [Enum]
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Json_projection_enum_with_custom_conversion(bool async)
    {
        await base.Json_projection_enum_with_custom_conversion(async);

        AssertSql(
            @"SELECT [j].[Id], CAST(JSON_VALUE([j].[json_reference_custom_naming],'$.CustomEnum') AS int) AS [Enum]
FROM [JsonEntitiesCustomNaming] AS [j]");
    }

    public override async Task Json_projection_with_deduplication(bool async)
    {
        await base.Json_projection_with_deduplication(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Name], JSON_QUERY([j].[OwnedCollectionRoot],'$'), JSON_QUERY([j].[OwnedReferenceRoot],'$'), CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.OwnedReferenceBranch.OwnedReferenceLeaf.SomethingSomething') AS nvarchar(max))
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Json_projection_with_deduplication_reverse_order(bool async)
    {
        await base.Json_projection_with_deduplication_reverse_order(async);

        AssertSql(
            @"SELECT JSON_QUERY([j].[OwnedReferenceRoot],'$'), [j].[Id], [j].[Name], JSON_QUERY([j].[OwnedCollectionRoot],'$')
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Json_property_in_predicate(bool async)
    {
        await base.Json_property_in_predicate(async);

        AssertSql(
            @"SELECT [j].[Id]
FROM [JsonEntitiesBasic] AS [j]
WHERE CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.OwnedReferenceBranch.Fraction') AS decimal(18,2)) < 20.5");
    }

    public override async Task Json_subquery_property_pushdown_length(bool async)
    {
        await base.Json_subquery_property_pushdown_length(async);

        AssertSql(
            @"@__p_0='3'

SELECT CAST(LEN([t0].[c]) AS int)
FROM (
    SELECT DISTINCT [t].[c]
    FROM (
        SELECT TOP(@__p_0) CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.OwnedReferenceBranch.OwnedReferenceLeaf.SomethingSomething') AS nvarchar(max)) AS [c]
        FROM [JsonEntitiesBasic] AS [j]
        ORDER BY [j].[Id]
    ) AS [t]
) AS [t0]");
    }

    public override async Task Json_subquery_reference_pushdown_reference(bool async)
    {
        await base.Json_subquery_reference_pushdown_reference(async);

        AssertSql(
            @"@__p_0='10'

SELECT JSON_QUERY([t0].[c],'$.OwnedReferenceBranch'), [t0].[Id]
FROM (
    SELECT DISTINCT JSON_QUERY([t].[c],'$') AS [c], [t].[Id]
    FROM (
        SELECT TOP(@__p_0) JSON_QUERY([j].[OwnedReferenceRoot],'$') AS [c], [j].[Id]
        FROM [JsonEntitiesBasic] AS [j]
        ORDER BY [j].[Id]
    ) AS [t]
) AS [t0]");
    }

    public override async Task Json_subquery_reference_pushdown_reference_anonymous_projection(bool async)
    {
        await base.Json_subquery_reference_pushdown_reference_anonymous_projection(async);

        AssertSql(
            @"@__p_0='10'

SELECT JSON_QUERY([t0].[c],'$.OwnedReferenceSharedBranch'), [t0].[Id], CAST(LEN([t0].[c0]) AS int)
FROM (
    SELECT DISTINCT JSON_QUERY([t].[c],'$') AS [c], [t].[Id], [t].[c0]
    FROM (
        SELECT TOP(@__p_0) JSON_QUERY([j].[json_reference_shared],'$') AS [c], [j].[Id], CAST(JSON_VALUE([j].[json_reference_shared],'$.OwnedReferenceSharedBranch.OwnedReferenceSharedLeaf.SomethingSomething') AS nvarchar(max)) AS [c0]
        FROM [JsonEntitiesBasic] AS [j]
        ORDER BY [j].[Id]
    ) AS [t]
) AS [t0]");
    }

    public override async Task Json_subquery_reference_pushdown_reference_pushdown_anonymous_projection(bool async)
    {
        await base.Json_subquery_reference_pushdown_reference_pushdown_anonymous_projection(async);

        AssertSql(
            @"@__p_0='10'

SELECT JSON_QUERY([t2].[c],'$.OwnedReferenceSharedLeaf'), [t2].[Id], JSON_QUERY([t2].[c],'$.OwnedCollectionSharedLeaf'), [t2].[Length]
FROM (
    SELECT DISTINCT JSON_QUERY([t1].[c],'$') AS [c], [t1].[Id], [t1].[Length]
    FROM (
        SELECT TOP(@__p_0) JSON_QUERY([t0].[c],'$.OwnedReferenceSharedBranch') AS [c], [t0].[Id], CAST(LEN([t0].[Scalar]) AS int) AS [Length]
        FROM (
            SELECT DISTINCT JSON_QUERY([t].[c],'$') AS [c], [t].[Id], [t].[Scalar]
            FROM (
                SELECT TOP(@__p_0) JSON_QUERY([j].[json_reference_shared],'$') AS [c], [j].[Id], CAST(JSON_VALUE([j].[json_reference_shared],'$.OwnedReferenceSharedBranch.OwnedReferenceSharedLeaf.SomethingSomething') AS nvarchar(max)) AS [Scalar]
                FROM [JsonEntitiesBasic] AS [j]
                ORDER BY [j].[Id]
            ) AS [t]
        ) AS [t0]
        ORDER BY CAST(LEN([t0].[Scalar]) AS int)
    ) AS [t1]
) AS [t2]");
    }

    public override async Task Json_subquery_reference_pushdown_reference_pushdown_reference(bool async)
    {
        await base.Json_subquery_reference_pushdown_reference_pushdown_reference(async);

        AssertSql(
            @"@__p_0='10'

SELECT JSON_QUERY([t2].[c],'$.OwnedReferenceLeaf'), [t2].[Id]
FROM (
    SELECT DISTINCT JSON_QUERY([t1].[c],'$') AS [c], [t1].[Id]
    FROM (
        SELECT TOP(@__p_0) JSON_QUERY([t0].[c],'$.OwnedReferenceBranch') AS [c], [t0].[Id]
        FROM (
            SELECT DISTINCT JSON_QUERY([t].[c],'$') AS [c], [t].[Id], [t].[c] AS [c0]
            FROM (
                SELECT TOP(@__p_0) JSON_QUERY([j].[OwnedReferenceRoot],'$') AS [c], [j].[Id]
                FROM [JsonEntitiesBasic] AS [j]
                ORDER BY [j].[Id]
            ) AS [t]
        ) AS [t0]
        ORDER BY CAST(JSON_VALUE([t0].[c0],'$.Name') AS nvarchar(max))
    ) AS [t1]
) AS [t2]");
    }

    public override async Task Json_subquery_reference_pushdown_reference_pushdown_collection(bool async)
    {
        await base.Json_subquery_reference_pushdown_reference_pushdown_collection(async);

        AssertSql(
            @"@__p_0='10'

SELECT JSON_QUERY([t2].[c],'$.OwnedCollectionLeaf'), [t2].[Id]
FROM (
    SELECT DISTINCT JSON_QUERY([t1].[c],'$') AS [c], [t1].[Id]
    FROM (
        SELECT TOP(@__p_0) JSON_QUERY([t0].[c],'$.OwnedReferenceBranch') AS [c], [t0].[Id]
        FROM (
            SELECT DISTINCT JSON_QUERY([t].[c],'$') AS [c], [t].[Id], [t].[c] AS [c0]
            FROM (
                SELECT TOP(@__p_0) JSON_QUERY([j].[OwnedReferenceRoot],'$') AS [c], [j].[Id]
                FROM [JsonEntitiesBasic] AS [j]
                ORDER BY [j].[Id]
            ) AS [t]
        ) AS [t0]
        ORDER BY CAST(JSON_VALUE([t0].[c0],'$.Name') AS nvarchar(max))
    ) AS [t1]
) AS [t2]");
    }

    public override async Task Json_subquery_reference_pushdown_property(bool async)
    {
        await base.Json_subquery_reference_pushdown_property(async);

        AssertSql(
            @"@__p_0='10'

SELECT CAST(JSON_VALUE([t0].[c],'$.SomethingSomething') AS nvarchar(max))
FROM (
    SELECT DISTINCT JSON_QUERY([t].[c],'$') AS [c], [t].[Id]
    FROM (
        SELECT TOP(@__p_0) JSON_QUERY([j].[OwnedReferenceRoot],'$.OwnedReferenceBranch.OwnedReferenceLeaf') AS [c], [j].[Id]
        FROM [JsonEntitiesBasic] AS [j]
        ORDER BY [j].[Id]
    ) AS [t]
) AS [t0]");
    }

    public override async Task Custom_naming_projection_owner_entity(bool async)
    {
        await base.Custom_naming_projection_owner_entity(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Title], JSON_QUERY([j].[json_collection_custom_naming],'$'), JSON_QUERY([j].[json_reference_custom_naming],'$')
FROM [JsonEntitiesCustomNaming] AS [j]");
    }

    public override async Task Custom_naming_projection_owned_reference(bool async)
    {
        await base.Custom_naming_projection_owned_reference(async);

        AssertSql(
            @"SELECT JSON_QUERY([j].[json_reference_custom_naming],'$.CustomOwnedReferenceBranch'), [j].[Id]
FROM [JsonEntitiesCustomNaming] AS [j]");
    }

    public override async Task Custom_naming_projection_owned_collection(bool async)
    {
        await base.Custom_naming_projection_owned_collection(async);

        AssertSql(
            @"SELECT JSON_QUERY([j].[json_collection_custom_naming],'$'), [j].[Id]
FROM [JsonEntitiesCustomNaming] AS [j]
ORDER BY [j].[Id]");
    }

    public override async Task Custom_naming_projection_owned_scalar(bool async)
    {
        await base.Custom_naming_projection_owned_scalar(async);

        AssertSql(
            @"SELECT CAST(JSON_VALUE([j].[json_reference_custom_naming],'$.CustomOwnedReferenceBranch.CustomFraction') AS float)
FROM [JsonEntitiesCustomNaming] AS [j]");
    }

    public override async Task Custom_naming_projection_everything(bool async)
    {
        await base.Custom_naming_projection_everything(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Title], JSON_QUERY([j].[json_collection_custom_naming],'$'), JSON_QUERY([j].[json_reference_custom_naming],'$'), CAST(JSON_VALUE([j].[json_reference_custom_naming],'$.CustomName') AS nvarchar(max)), CAST(JSON_VALUE([j].[json_reference_custom_naming],'$.CustomOwnedReferenceBranch.CustomFraction') AS float)
FROM [JsonEntitiesCustomNaming] AS [j]");
    }

    public override async Task Project_entity_with_single_owned(bool async)
    {
        await base.Project_entity_with_single_owned(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Name], JSON_QUERY([j].[OwnedCollection],'$')
FROM [JsonEntitiesSingleOwned] AS [j]");
    }

    public override async Task Left_join_json_entities(bool async)
    {
        await base.Left_join_json_entities(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Name], JSON_QUERY([j].[OwnedCollection],'$'), [j0].[Id], [j0].[Name], JSON_QUERY([j0].[OwnedCollectionRoot],'$'), JSON_QUERY([j0].[OwnedReferenceRoot],'$')
FROM [JsonEntitiesSingleOwned] AS [j]
LEFT JOIN [JsonEntitiesBasic] AS [j0] ON [j].[Id] = [j0].[Id]");
    }

    public override async Task Left_join_json_entities_complex_projection(bool async)
    {
        await base.Left_join_json_entities_complex_projection(async);

        AssertSql(
            @"SELECT [j].[Id], [j0].[Id], [j0].[Name], JSON_QUERY([j0].[OwnedCollectionRoot],'$'), JSON_QUERY([j0].[OwnedReferenceRoot],'$')
FROM [JsonEntitiesSingleOwned] AS [j]
LEFT JOIN [JsonEntitiesBasic] AS [j0] ON [j].[Id] = [j0].[Id]");
    }

    public override async Task Project_json_entity_FirstOrDefault_subquery(bool async)
    {
        await base.Project_json_entity_FirstOrDefault_subquery(async);

        AssertSql(
            @"SELECT JSON_QUERY([t].[c],'$'), [t].[Id]
FROM [JsonEntitiesBasic] AS [j]
OUTER APPLY (
    SELECT TOP(1) JSON_QUERY([j0].[OwnedReferenceRoot],'$.OwnedReferenceBranch') AS [c], [j0].[Id]
    FROM [JsonEntitiesBasic] AS [j0]
    ORDER BY [j0].[Id]
) AS [t]
ORDER BY [j].[Id]");
    }

    public override async Task Project_json_entity_FirstOrDefault_subquery_with_binding_on_top(bool async)
    {
        await base.Project_json_entity_FirstOrDefault_subquery_with_binding_on_top(async);

        AssertSql(
            @"SELECT (
    SELECT TOP(1) CAST(JSON_VALUE([j0].[OwnedReferenceRoot],'$.OwnedReferenceBranch.Date') AS datetime2)
    FROM [JsonEntitiesBasic] AS [j0]
    ORDER BY [j0].[Id])
FROM [JsonEntitiesBasic] AS [j]
ORDER BY [j].[Id]");
    }

    public override async Task Project_json_entity_FirstOrDefault_subquery_with_entity_comparison_on_top(bool async)
    {
        await base.Project_json_entity_FirstOrDefault_subquery_with_entity_comparison_on_top(async);

        AssertSql(
            @"");
    }

    public override async Task Project_json_entity_FirstOrDefault_subquery_deduplication(bool async)
    {
        await base.Project_json_entity_FirstOrDefault_subquery_deduplication(async);

        AssertSql(
            @"SELECT JSON_QUERY([t].[c],'$'), [t].[Id], JSON_QUERY([t].[c0],'$'), [t].[Id0], JSON_QUERY([t].[c1],'$'), [t].[c2], [t].[c3], [t].[c4]
FROM [JsonEntitiesBasic] AS [j]
OUTER APPLY (
    SELECT TOP(1) JSON_QUERY([j].[OwnedReferenceRoot],'$.OwnedCollectionBranch') AS [c], [j].[Id], JSON_QUERY([j0].[OwnedReferenceRoot],'$') AS [c0], [j0].[Id] AS [Id0], JSON_QUERY([j0].[OwnedReferenceRoot],'$.OwnedReferenceBranch') AS [c1], CAST(JSON_VALUE([j0].[OwnedReferenceRoot],'$.Name') AS nvarchar(max)) AS [c2], CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.OwnedReferenceBranch.Enum') AS nvarchar(max)) AS [c3], 1 AS [c4]
    FROM [JsonEntitiesBasic] AS [j0]
    ORDER BY [j0].[Id]
) AS [t]
ORDER BY [j].[Id]");
    }


    public override async Task Project_json_entity_FirstOrDefault_subquery_deduplication_and_outer_reference(bool async)
    {
        await base.Project_json_entity_FirstOrDefault_subquery_deduplication_and_outer_reference(async);

        AssertSql(
            @"SELECT JSON_QUERY([t].[c],'$'), [t].[Id], JSON_QUERY([t].[c0],'$'), [t].[Id0], JSON_QUERY([t].[c1],'$'), [t].[c2], [t].[c3], [t].[c4]
FROM [JsonEntitiesBasic] AS [j]
OUTER APPLY (
    SELECT TOP(1) JSON_QUERY([j].[OwnedReferenceRoot],'$.OwnedCollectionBranch') AS [c], [j].[Id], JSON_QUERY([j0].[OwnedReferenceRoot],'$') AS [c0], [j0].[Id] AS [Id0], JSON_QUERY([j0].[OwnedReferenceRoot],'$.OwnedReferenceBranch') AS [c1], CAST(JSON_VALUE([j0].[OwnedReferenceRoot],'$.Name') AS nvarchar(max)) AS [c2], CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.OwnedReferenceBranch.Enum') AS nvarchar(max)) AS [c3], 1 AS [c4]
    FROM [JsonEntitiesBasic] AS [j0]
    ORDER BY [j0].[Id]
) AS [t]
ORDER BY [j].[Id]");
    }

    public override async Task Project_json_entity_FirstOrDefault_subquery_deduplication_outer_reference_and_pruning(bool async)
    {
        await base.Project_json_entity_FirstOrDefault_subquery_deduplication_outer_reference_and_pruning(async);

        AssertSql(
            @"SELECT JSON_QUERY([t].[c],'$'), [t].[Id], [t].[c0]
FROM [JsonEntitiesBasic] AS [j]
OUTER APPLY (
    SELECT TOP(1) JSON_QUERY([j].[OwnedReferenceRoot],'$.OwnedCollectionBranch') AS [c], [j].[Id], 1 AS [c0]
    FROM [JsonEntitiesBasic] AS [j0]
    ORDER BY [j0].[Id]
) AS [t]
ORDER BY [j].[Id]");
    }


    public override async Task Json_entity_with_inheritance_basic_projection(bool async)
    {
        await base.Json_entity_with_inheritance_basic_projection(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Discriminator], [j].[Name], [j].[Fraction], JSON_QUERY([j].[CollectionOnBase],'$'), JSON_QUERY([j].[ReferenceOnBase],'$'), JSON_QUERY([j].[CollectionOnDerived],'$'), JSON_QUERY([j].[ReferenceOnDerived],'$')
FROM [JsonEntitiesInheritance] AS [j]");
    }

    public override async Task Json_entity_with_inheritance_project_derived(bool async)
    {
        await base.Json_entity_with_inheritance_project_derived(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Discriminator], [j].[Name], [j].[Fraction], JSON_QUERY([j].[CollectionOnBase],'$'), JSON_QUERY([j].[ReferenceOnBase],'$'), JSON_QUERY([j].[CollectionOnDerived],'$'), JSON_QUERY([j].[ReferenceOnDerived],'$')
FROM [JsonEntitiesInheritance] AS [j]
WHERE [j].[Discriminator] = N'JsonEntityInheritanceDerived'");
    }

    public override async Task Json_entity_with_inheritance_project_navigations(bool async)
    {
        await base.Json_entity_with_inheritance_project_navigations(async);

        AssertSql(
            @"SELECT [j].[Id], JSON_QUERY([j].[ReferenceOnBase],'$'), JSON_QUERY([j].[CollectionOnBase],'$')
FROM [JsonEntitiesInheritance] AS [j]");
    }

    public override async Task Json_entity_with_inheritance_project_navigations_on_derived(bool async)
    {
        await base.Json_entity_with_inheritance_project_navigations_on_derived(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Discriminator], [j].[Name], [j].[Fraction], JSON_QUERY([j].[CollectionOnBase],'$'), JSON_QUERY([j].[ReferenceOnBase],'$'), JSON_QUERY([j].[CollectionOnDerived],'$'), JSON_QUERY([j].[ReferenceOnDerived],'$')
FROM [JsonEntitiesInheritance] AS [j]
WHERE [j].[Discriminator] = N'JsonEntityInheritanceDerived'");
    }

    public override async Task Json_entity_backtracking(bool async)
    {
        await base.Json_entity_backtracking(async);

        AssertSql(
            @"");
    }

    public override async Task Json_collection_element_access_in_projection_basic(bool async)
    {
        await base.Json_collection_element_access_in_projection_basic(async);

        // array element access in projection is currently done on the client - issue 28648
        AssertSql(
            @"SELECT JSON_QUERY([j].[OwnedCollectionRoot],'$'), [j].[Id]
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Json_collection_element_access_in_predicate(bool async)
    {
        await base.Json_collection_element_access_in_predicate(async);

        AssertSql(
            @"");
    }

    public override async Task Json_scalar_required_null_semantics(bool async)
    {
        await base.Json_scalar_required_null_semantics(async);

        AssertSql(
            @"SELECT [j].[Name]
FROM [JsonEntitiesBasic] AS [j]
WHERE CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.Number') AS int) <> CAST(LEN(CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.Name') AS nvarchar(max))) AS int) OR CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.Name') AS nvarchar(max)) IS NULL");
    }

    public override async Task Json_scalar_optional_null_semantics(bool async)
    {
        await base.Json_scalar_optional_null_semantics(async);

        AssertSql(
            @"SELECT [j].[Name]
FROM [JsonEntitiesBasic] AS [j]
WHERE CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.Name') AS nvarchar(max)) = CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.OwnedReferenceBranch.OwnedReferenceLeaf.SomethingSomething') AS nvarchar(max)) OR (CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.Name') AS nvarchar(max)) IS NULL AND CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.OwnedReferenceBranch.OwnedReferenceLeaf.SomethingSomething') AS nvarchar(max)) IS NULL)");
    }

    public override async Task Group_by_on_json_scalar(bool async)
    {
        await base.Group_by_on_json_scalar(async);

        AssertSql(
            @"SELECT [t].[Key], COUNT(*) AS [Count]
FROM (
    SELECT CAST(JSON_VALUE([j].[OwnedReferenceRoot],'$.Name') AS nvarchar(max)) AS [Key]
    FROM [JsonEntitiesBasic] AS [j]
) AS [t]
GROUP BY [t].[Key]");
    }

    public override async Task Json_with_include_on_json_entity(bool async)
    {
        await base.Json_with_include_on_json_entity(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Name], JSON_QUERY([j].[OwnedCollectionRoot],'$'), JSON_QUERY([j].[OwnedReferenceRoot],'$')
FROM [JsonEntitiesBasic] AS [j]");
    }

    public override async Task Json_with_include_on_entity_reference(bool async)
    {
        await base.Json_with_include_on_entity_reference(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Name], JSON_QUERY([j].[OwnedCollectionRoot],'$'), JSON_QUERY([j].[OwnedReferenceRoot],'$'), [j0].[Id], [j0].[Name], [j0].[ParentId]
FROM [JsonEntitiesBasic] AS [j]
LEFT JOIN [JsonEntitiesBasicForReference] AS [j0] ON [j].[Id] = [j0].[ParentId]");
    }

    public override async Task Json_with_include_on_entity_collection(bool async)
    {
        await base.Json_with_include_on_entity_collection(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Name], JSON_QUERY([j].[OwnedCollectionRoot],'$'), JSON_QUERY([j].[OwnedReferenceRoot],'$'), [j0].[Id], [j0].[Name], [j0].[ParentId]
FROM [JsonEntitiesBasic] AS [j]
LEFT JOIN [JsonEntitiesBasicForCollection] AS [j0] ON [j].[Id] = [j0].[ParentId]
ORDER BY [j].[Id]");
    }

    public override async Task Json_with_include_on_entity_collection_and_reference(bool async)
    {
        await base.Json_with_include_on_entity_collection_and_reference(async);

        AssertSql(
            @"SELECT [j].[Id], [j].[Name], JSON_QUERY([j].[OwnedCollectionRoot],'$'), JSON_QUERY([j].[OwnedReferenceRoot],'$'), [j0].[Id], [j0].[Name], [j0].[ParentId], [j1].[Id], [j1].[Name], [j1].[ParentId]
FROM [JsonEntitiesBasic] AS [j]
LEFT JOIN [JsonEntitiesBasicForReference] AS [j0] ON [j].[Id] = [j0].[ParentId]
LEFT JOIN [JsonEntitiesBasicForCollection] AS [j1] ON [j].[Id] = [j1].[ParentId]
ORDER BY [j].[Id], [j0].[Id]");
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
