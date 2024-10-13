namespace Discord;

public partial interface ISkuActor :
    IActor<ulong, ISku>,
    IApplicationActor.CanonicalRelationship;