using System;
using System.Collections.Generic;

namespace Discord.Interactions.Entities;

public interface ISelectMenuModel<T>
{
    IList<T> Values { get; }

    IEnumerable<SelectMenuOption> Options(IModal modal, IInteractionContext context, IServiceProvider services);
}
