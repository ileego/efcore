﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.EntityFrameworkCore.TestModels.ManyToManyFieldsModel;

public class EntityRoot
{
    public int Id;
    public string Name;
    public ICollection<EntityThree> ThreeSkipShared;
    public ICollection<EntityCompositeKey> CompositeKeySkipShared;
}
