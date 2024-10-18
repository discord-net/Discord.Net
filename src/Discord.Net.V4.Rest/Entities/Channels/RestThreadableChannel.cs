// using Discord.Models;
// using Discord.Models.Json;
// using Discord.Rest;
// using Discord.Rest.Extensions;
//
// namespace Discord.Rest;
//
// [ExtendInterfaceDefaults]
// public partial class RestThreadableChannelActor :
//     RestGuildChannelActor,
//     IThreadableChannelActor,
//     IRestActor<RestThreadableChannelActor, ulong, RestThreadableChannel, IThreadableChannelModel>,
//     IRestGuildChannelInvitableTrait
// {
//     [SourceOfTruth]
//     public RestGuildThreadChannelActor.Indexable.WithNestedThreads.BackLink<RestThreadableChannelActor> Threads { get; }
//
//     [SourceOfTruth] internal override ThreadableChannelIdentity Identity { get; }
//
//     [TypeFactory]
//     public RestThreadableChannelActor(
//         DiscordRestClient client,
//         GuildIdentity guild,
//         ThreadableChannelIdentity channel
//     ) : base(client, guild, channel)
//     {
//         Identity = channel | this;
//
//         Threads = new(
//             this,
//             client,
//             RestActorProvider.GetOrCreate(
//                 client,
//                 Template.Of<GuildThreadIdentity>(),
//                 guild
//             ),
//             new(
//                 client,
//                 RestActorProvider.GetOrCreate(
//                     client,
//                     Template.Of<GuildThreadIdentity>(),
//                     guild
//                 ),
//                 new RestPagingProvider<
//                     IThreadChannelModel,
//                     ChannelThreads, 
//                     PagePublicArchivedThreadsParams,
//                     RestThreadChannel
//                 >(
//                     client,
//                     m => m.Threads,
//                     CreateThreadFromPaged,
//                     this
//                 )
//             ),
//             new(
//                 client,
//                 RestActorProvider.GetOrCreate(
//                     client,
//                     Template.Of<GuildThreadIdentity>(),
//                     guild
//                 ),
//                 new RestPagingProvider<
//                     IThreadChannelModel,
//                     ChannelThreads, 
//                     PagePrivateArchivedThreadsParams,
//                     RestThreadChannel
//                 >(
//                     client,
//                     m => m.Threads,
//                     CreateThreadFromPaged,
//                     this
//                 )
//             ),
//             new(
//                 client,
//                 RestActorProvider.GetOrCreate(
//                     client,
//                     Template.Of<GuildThreadIdentity>(),
//                     guild
//                 ),
//                 new RestPagingProvider<
//                     IThreadChannelModel,
//                     ChannelThreads, 
//                     PageJoinedPrivateArchivedThreadsParams,
//                     RestThreadChannel
//                 >(
//                     client,
//                     m => m.Threads,
//                     CreateThreadFromPaged,
//                     this
//                 )
//             )
//         );
//     }
//
//     private RestThreadChannel CreateThreadFromPaged(IThreadChannelModel model, ChannelThreads api)
//     {
//         var actor = Threads[model.Id];
//         
//         var memberModel = api.Members.FirstOrDefault(x => x.ThreadId == actor.Id);
//         
//         if(memberModel is not null)
//             actor.Members.Current.AddModelSource(memberModel);
//
//         return actor.CreateEntity(model);
//     }
//
//     [SourceOfTruth]
//     [CovariantOverride]
//     internal virtual RestThreadableChannel CreateEntity(IThreadableChannelModel model)
//         => RestThreadableChannel.Construct(Client, this, model);
// }
//
// public partial class RestThreadableChannel :
//     RestGuildChannel,
//     IThreadableChannel,
//     IRestNestedChannelTrait,
//     IRestConstructable<RestThreadableChannel, RestThreadableChannelActor, IThreadableChannelModel>
// {
//     public int? DefaultThreadSlowmode => Model.DefaultThreadRateLimitPerUser;
//
//     public ThreadArchiveDuration DefaultArchiveDuration => (ThreadArchiveDuration) Model.DefaultAutoArchiveDuration;
//
//     internal override IThreadableChannelModel Model => _model;
//
//     [ProxyInterface(
//         typeof(IThreadableChannelActor),
//         typeof(IEntityProvider<IThreadableChannel, IThreadableChannelModel>)
//     )]
//     internal override RestThreadableChannelActor Actor { get; }
//
//     private IThreadableChannelModel _model;
//
//     internal RestThreadableChannel(
//         DiscordRestClient client,
//         IThreadableChannelModel model,
//         RestThreadableChannelActor actor
//     ) : base(client, model, actor)
//     {
//         _model = model;
//         Actor = actor;
//     }
//
//     public static RestThreadableChannel Construct(
//         DiscordRestClient client,
//         RestThreadableChannelActor actor,
//         IThreadableChannelModel model)
//     {
//         switch (model)
//         {
//             case IGuildForumChannelModel forumModel:
//                 return RestForumChannel.Construct(
//                     client,
//                     actor as RestForumChannelActor ?? actor.Guild.Channels.Forum[model.Id],
//                     forumModel
//                 );
//             case IGuildMediaChannelModel mediaModel:
//                 return RestMediaChannel.Construct(
//                     client,
//                     actor as RestMediaChannelActor ?? actor.Guild.Channels.Media[model.Id],
//                     mediaModel
//                 );
//             case IGuildNewsChannelModel newsModel:
//                 return RestNewsChannel.Construct(
//                     client,
//                     actor as RestNewsChannelActor ?? actor.Guild.Channels.News[model.Id],
//                     newsModel
//                 );
//             case IGuildTextChannelModel textModel:
//                 return RestTextChannel.Construct(
//                     client,
//                     actor as RestTextChannelActor ?? actor.Guild.Channels.Text[model.Id],
//                     textModel
//                 );
//             default:
//                 return new(client, model, actor);
//         }
//     }
//
//     [CovariantOverride]
//     public virtual ValueTask UpdateAsync(IThreadableChannelModel model, CancellationToken token = default)
//     {
//         _model = model;
//
//         return base.UpdateAsync(model, token);
//     }
//
//     public override IThreadableChannelModel GetModel() => Model;
// }