namespace Discord;

public partial interface ISkuActor :
    IActor<ulong, ISku>,
    IApplicationRelationship;